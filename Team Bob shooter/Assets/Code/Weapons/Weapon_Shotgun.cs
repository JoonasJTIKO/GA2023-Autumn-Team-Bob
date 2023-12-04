using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Weapon_Shotgun : WeaponBase
    {
        [SerializeField]
        private float spreadAngleX = 0.5f;

        [SerializeField]
        private float spreadAngleY = 0.5f;

        [SerializeField]
        private LayerMask environmentLayers;

        [SerializeField]
        private LayerMask enemyLayers;

        [SerializeField]
        private int pelletCount = 5;

        [SerializeField]
        private float reloadTime = 0.5f;

        [SerializeField]
        private ParticleSystem muzzleFlash;

        private PlayerUnit playerUnit;

        private Transform[] activeHitEffects = new Transform[8];

        private int index = 0;

        private Coroutine reloadRoutine = null;

        private ScreenShake screenShake;

        protected override void Awake()
        {
            base.Awake();

            playerUnit = GetComponent<PlayerUnit>();
        }

        private void Start()
        {
            screenShake = playerUnit.PlayerCam.GetComponent<ScreenShake>();
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
        }

        private IEnumerator ReloadAfterDelay()
        {
            float timer = 0;
            while (timer < reloadTime)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_SHOTGUN_RELOAD, transform.position, volume: 0.5f, make2D: true);
            ReloadCompleted();
        }

        protected override void Fire()
        {
            screenShake.Shake(0);
            recoil.DoRecoil(-4);
            muzzleFlash.Play();
            GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_SHOTGUN_SHOOT, transform.position, volume: 0.25f, make2D: true);

            Dictionary<UnitHealth, float> damages = new Dictionary<UnitHealth, float>();

            for (int i = 0; i < pelletCount; i++)
            {
                RaycastHit hit;
                Vector3 angle = playerUnit.PlayerCam.transform.TransformDirection(Vector3.forward);
                angle = new(angle.x + Random.Range(-spreadAngleX, spreadAngleX),
                    angle.y + Random.Range(-spreadAngleY, spreadAngleY),
                    angle.z + Random.Range(-spreadAngleX, spreadAngleX));

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

                    if (hit.collider.gameObject.tag == "EnemyRagdoll") continue;

                    float damage = bulletDamage;
                    if (hit.collider.gameObject.tag == "EnemyHead")
                    {
                        damage *= 1.5f;
                    }

                    if (Vector3.Distance(transform.position, hit.point) > falloffDistance)
                    {
                        damage *= 0.5f;
                    }

                    UnitHealth component = hit.collider.GetComponentInParent<UnitHealth>();
                    if (damages.ContainsKey(component))
                    {
                        damages[component] += damage;
                    }
                    else
                    {
                        damages.Add(component, damage);
                    }

                    if (activeHitEffects[index] != null)
                    {
                        hitEffectPool.Return(activeHitEffects[index]);
                    }
                    activeHitEffects[index] = hitEffectPool.Get();
                    activeHitEffects[index].position = hit.point;
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
            }

            foreach (KeyValuePair<UnitHealth, float> item in damages)
            {
                item.Key.RemoveHealth(item.Value, EnemyGibbing.DeathType.Explode);
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
