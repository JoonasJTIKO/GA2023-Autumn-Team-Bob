using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Weapon_Minigun : WeaponBase
    {
        [SerializeField]
        private float knockbackStrength = 2f;

        [SerializeField]
        private float spreadAngle = 0.5f;

        [SerializeField]
        private LayerMask environmentLayers;

        [SerializeField]
        private LayerMask enemyLayers;

        [SerializeField]
        private ParticleSystem muzzleFlash;

        private PlayerUnit playerUnit;

        private Rigidbody rb;

        private Transform[] activeHitEffects = new Transform[8];

        private int index = 0;

        private bool shootAudioPlaying = false;

        private AudioSource shootAudio;

        public override int CurrentReserveAmmo
        {
            get { return currentMagAmmoCount; }
        }

        public override int MaxReserveAmmo
        {
            get { return magSize; }
        }

        private ScreenShake screenShake;

        protected override void Awake()
        {
            base.Awake();

            playerUnit = GetComponent<PlayerUnit>();
            rb = GetComponent<Rigidbody>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            FireButtonHeld(false);
        }

        private void Start()
        {
            screenShake = playerUnit.PlayerCam.GetComponent<ScreenShake>();
        }

        public override void AbortReload()
        {
        }

        public override void BeginReload()
        {

        }

        protected override void Fire()
        {
            FireButtonHeld(true);
            screenShake.Shake(1);
            recoil.DoRecoil(-0.3f);
            muzzleFlash.Play();

            RaycastHit hit;
            Vector3 angle = playerUnit.PlayerCam.transform.TransformDirection(Vector3.forward);
            angle = new(angle.x + Random.Range(-spreadAngle, spreadAngle),
                angle.y + Random.Range(-spreadAngle, spreadAngle),
                angle.z + Random.Range(-spreadAngle, spreadAngle));

            BulletTracer bullet = bulletTrailPool.Get();
            bullet.Expired += RecycleTracer;
            bullet.transform.position = bulletOrigin.transform.position;
            bullet.Launch(angle);

            if (Physics.Raycast(playerUnit.PlayerCam.transform.position,
                angle, out hit, Mathf.Infinity, enemyLayers))
            {
                RaycastHit rHit;
                if (Physics.Raycast(playerUnit.PlayerCam.transform.position,
                angle, out rHit, hit.distance, environmentLayers))
                {
                    return;
                }

                if (hit.collider.gameObject.tag == "EnemyRagdoll") return;

                float damage = bulletDamage;
                if (hit.collider.gameObject.tag == "EnemyHead")
                {
                    damage *= 1.5f;
                }

                EnemyGibbing.DeathType deathType = EnemyGibbing.DeathType.Normal;
                switch (hit.collider.gameObject.tag)
                {
                    case "EnemyBody":
                        deathType = EnemyGibbing.DeathType.Normal;
                        break;
                    case "EnemyHead":
                        deathType = EnemyGibbing.DeathType.Head;
                        break;
                    case "EnemyArmR":
                        deathType = EnemyGibbing.DeathType.RightArm;
                        break;
                    case "EnemyArmL":
                        deathType = EnemyGibbing.DeathType.LeftArm;
                        break;
                    case "EnemyLegR":
                        deathType = EnemyGibbing.DeathType.RightLeg;
                        break;
                    case "EnemyLegL":
                        deathType = EnemyGibbing.DeathType.LeftLeg;
                        break;
                }

                hit.collider.gameObject.GetComponentInParent<UnitHealth>().RemoveHealth(damage, deathType);

                if (activeHitEffects[index] != null)
                {
                    hitEffectPool.Return(activeHitEffects[index]);
                }
                activeHitEffects[index] = hitEffectPool.Get();
                activeHitEffects[index].position = hit.point;
                activeHitEffects[index].transform.up = -angle;
                index++;
                if (index >= activeHitEffects.Length) index = 0;
            }
            //else if (Physics.Raycast(playerUnit.PlayerCam.transform.position,
            //    angle, out hit, Mathf.Infinity, environmentLayers))
            //{
            //    if (activeHitEffects[index] != null)
            //    {
            //        hitEffectPool.Return(activeHitEffects[index]);
            //    }
            //    activeHitEffects[index] = hitEffectPool.Get();
            //    activeHitEffects[index].position = hit.point;
            //    index++;
            //    if (index >= activeHitEffects.Length) index = 0;
            //}

            if (!playerUnit.IsGrounded)
            {
                if (rb.velocity.y < 0 && playerUnit.PlayerCam.transform.TransformDirection(transform.forward).y < 0) rb.velocity = new(rb.velocity.x, 0, rb.velocity.z);

                rb.AddForce(-playerUnit.PlayerCam.transform.TransformDirection(Vector3.forward) * knockbackStrength, ForceMode.Impulse);
            }
        }

        public override void AddAmmo(int amount)
        {
            currentMagAmmoCount += amount;
            if (currentMagAmmoCount > magSize)
            {
                currentMagAmmoCount = magSize;
            }
        }

        public override void Activate(bool state)
        {
            base.Activate(state);
            viewmodelAnimator.SetTrigger("Equip");
            bulletOrigin.transform.position = muzzleFlash.transform.position;

            if (!state)
            {
                FireButtonHeld(false);
            }
        }

        public override void FireButtonHeld(bool state)
        {
            viewmodelAnimator.SetBool("Firing", state);
            if (state && !shootAudioPlaying)
            {
                if (shootAudio != null)
                {
                    GameInstance.Instance.GetAudioManager().StopLoopingAudio(shootAudio);
                }

                shootAudio = GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_MINIGUN_SHOOT, transform.position, volume: 0.3f, loop: true, make2D: true);
                shootAudioPlaying = true;
            }
            else if (!state && shootAudioPlaying)
            {
                GameInstance.Instance.GetAudioManager().StopLoopingAudio(shootAudio);
                shootAudio = null;
                shootAudioPlaying = false;
            }
        }
    }
}
