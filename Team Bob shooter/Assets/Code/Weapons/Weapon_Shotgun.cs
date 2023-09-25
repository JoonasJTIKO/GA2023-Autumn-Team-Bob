using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Weapon_Shotgun : WeaponBase
    {
        [SerializeField]
        private float spreadAngle = 0.5f;

        [SerializeField]
        private LayerMask environmentLayers;

        [SerializeField]
        private LayerMask enemyLayers;

        [SerializeField]
        private int pelletCount = 5;

        [SerializeField]
        private float reloadTime = 0.5f;

        private PlayerUnit playerUnit;

        private Transform[] activeHitEffects = new Transform[8];

        private int index = 0;

        private Coroutine reloadRoutine = null;

        protected override void Awake()
        {
            base.Awake();

            playerUnit = GetComponent<PlayerUnit>();
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
            yield return new WaitForSeconds(reloadTime);
            ReloadCompleted();
        }

        protected override void Fire()
        {
            Dictionary<UnitHealth, float> damages = new Dictionary<UnitHealth, float>();

            for (int i = 0; i < pelletCount; i++)
            {
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
                    float damage = bulletDamage;
                    if (hit.collider.gameObject.layer == 16)
                    {
                        damage *= 1.5f;
                    }

                    UnitHealth component = hit.collider.GetComponent<UnitHealth>();
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
                else if (Physics.Raycast(playerUnit.PlayerCam.transform.position,
                    angle, out hit, Mathf.Infinity, environmentLayers))
                {
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

            foreach (KeyValuePair<UnitHealth, float> item in damages)
            {
                item.Key.RemoveHealth(item.Value);
            }
        }
    }
}
