using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

namespace TeamBobFPS
{
    public class Weapon_Rocketlauncher : WeaponBase
    {
        [SerializeField]
        private RocketProjectile projectilePrefab;

        [SerializeField]
        private Transform launchPosition;

        [SerializeField]
        private float reloadTime = 0.5f;

        private ComponentPool<RocketProjectile> pool;

        private Coroutine reloadRoutine = null;

        protected override void Awake()
        {
            base.Awake();

            pool = new ComponentPool<RocketProjectile>(projectilePrefab, 5);
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
            RocketProjectile launched = pool.Get();
            launched.Expired += RocketExpired;
            launched.transform.position = launchPosition.position;
            launched.transform.rotation = launchPosition.rotation;
            launched.Launch(bulletDamage);
        }

        private void RocketExpired(RocketProjectile rocket)
        {
            rocket.Expired -= RocketExpired;

            if (!pool.Return(rocket))
            {
                Debug.LogError("Can't return rocket to pool!");
            }
        }
    }
}
