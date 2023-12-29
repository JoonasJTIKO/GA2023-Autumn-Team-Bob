using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Weapon_Railgun : WeaponBase
    {
        [SerializeField]
        private float spreadAngle = 0.5f;

        [SerializeField]
        private LayerMask environmentLayers;

        [SerializeField]
        private LayerMask enemyLayers;

        [SerializeField]
        private float reloadTime = 0.5f;

        [SerializeField]
        private float chargeTime = 0.75f;

        [SerializeField]
        private float shotSize = 0.5f;

        [SerializeField]
        private ParticleSystem muzzleFlash;

        private PlayerUnit playerUnit;

        private Transform[] activeHitEffects = new Transform[8];

        private int index = 0;

        private Coroutine reloadRoutine = null;

        private ScreenShake screenShake;

        private bool buttonHeld = false;

        private bool fireAnimStarted = false;

        private float chargeTimer = 0f;

        protected override void Awake()
        {
            base.Awake();

            playerUnit = GetComponent<PlayerUnit>();
        }

        private void Start()
        {
            screenShake = playerUnit.PlayerCam.GetComponent<ScreenShake>();
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (buttonHeld)
            {
                if (!fireAnimStarted)
                {
                    if (viewmodelAnimator != null)
                    {
                        viewmodelAnimator.SetTrigger("Shoot");
                        fireAnimStarted = true;
                    }
                }

                chargeTimer += deltaTime;

                if (chargeTimer >= chargeTime)
                {
                    ChargeCompleted();
                    fireAnimStarted = false;
                }
            }
            else
            {
                chargeTimer = 0;
            }
        }

        public override void Shoot()
        {
            FireButtonHeld(true);
        }

        public override void AbortReload()
        {
            if (reloadRoutine != null && reloading)
            {
                StopCoroutine(reloadRoutine);
                reloading = false;
            }
        }

        public override void BeginReload()
        {
            if (currentMagAmmoCount == magSize || reloading) return;

            reloading = true;

            reloadRoutine = StartCoroutine(ReloadAfterDelay());
            GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_RAILGUN_RELOAD, transform.position, make2D: true);
        }

        private IEnumerator ReloadAfterDelay()
        {
            float timer = 0;
            while (timer < reloadTime)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            ReloadCompleted();
        }

        private void ChargeCompleted()
        {
            currentMagAmmoCount -= shotAmmoCost;
            UpdateHudAmmo();

            readyToFire = false;
            if (fireRate != 0) timer = 1 / fireRate;
            else timer = 0;

            screenShake.Shake(0);
            GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_RAILGUN_SHOOT, transform.position, volume: 0.5f, make2D: true);

            Vector3 angle = playerUnit.PlayerCam.transform.TransformDirection(Vector3.forward);
            angle = new(angle.x + Random.Range(-spreadAngle, spreadAngle),
                angle.y + Random.Range(-spreadAngle, spreadAngle),
                angle.z + Random.Range(-spreadAngle, spreadAngle));

            BulletTracer bullet = bulletTrailPool.Get();
            if (bullet != null)
            {
                bullet.Expired += RecycleTracer;
                bullet.transform.position = bulletOrigin.transform.position;
                bullet.Launch(angle);
            }

            RaycastHit[] hits = Physics.SphereCastAll(playerUnit.PlayerCam.transform.position,
                shotSize, angle, Mathf.Infinity, enemyLayers);

            if (hits.Length > 0)
            {
                RaycastHit rHit;
                if (Physics.Raycast(playerUnit.PlayerCam.transform.position,
                angle, out rHit, hits[0].distance, environmentLayers))
                {
                    return;
                }

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.tag == "EnemyRagdoll") return;

                    float damage = bulletDamage;
                    if (hit.collider.gameObject.tag == "EnemyHead")
                    {
                        damage *= 1.5f;
                    }

                    EnemyGibbing.DeathType deathType = EnemyGibbing.DeathType.Explode;
                    //switch (hit.collider.gameObject.tag)
                    //{
                    //    case "EnemyBody":
                    //        deathType = EnemyGibbing.DeathType.Normal;
                    //        break;
                    //    case "EnemyHead":
                    //        deathType = EnemyGibbing.DeathType.Head;
                    //        break;
                    //    case "EnemyArmR":
                    //        deathType = EnemyGibbing.DeathType.RightArm;
                    //        break;
                    //    case "EnemyArmL":
                    //        deathType = EnemyGibbing.DeathType.LeftArm;
                    //        break;
                    //    case "EnemyLegR":
                    //        deathType = EnemyGibbing.DeathType.RightLeg;
                    //        break;
                    //    case "EnemyLegL":
                    //        deathType = EnemyGibbing.DeathType.LeftLeg;
                    //        break;
                    //}

                    hit.collider.gameObject.GetComponentInParent<UnitHealth>().RemoveHealth(damage, deathType);

                    if (activeHitEffects[index] != null)
                    {
                        hitEffectPool.Return(activeHitEffects[index]);
                    }
                    activeHitEffects[index] = hitEffectPool.Get();
                    activeHitEffects[index].position = hit.point;
                    index++;
                    if (index >= activeHitEffects.Length) index = 0;
                }
            }
            //else if (Physics.Raycast(playerUnit.PlayerCam.transform.position,
            //    angle, out rHit, Mathf.Infinity, environmentLayers))
            //{
            //    if (activeHitEffects[index] != null)
            //    {
            //        hitEffectPool.Return(activeHitEffects[index]);
            //    }
            //    activeHitEffects[index] = hitEffectPool.Get();
            //    activeHitEffects[index].position = rHit.point;
            //    index++;
            //    if (index >= activeHitEffects.Length) index = 0;
            //}
        }

        protected override void Fire()
        {
        }

        public override void FireButtonHeld(bool state)
        {
            if (currentMagAmmoCount == 0 || !readyToFire || reloading || !active)
            {
                buttonHeld = false;
            }
            else
            {
                buttonHeld = state;
            }
        }

        public override void Activate(bool state)
        {
            base.Activate(state);

            viewmodelAnimator.SetTrigger("Equip");
            bulletOrigin.transform.position = muzzleFlash.transform.position;
        }
    }
}
