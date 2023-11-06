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

        private PlayerUnit playerUnit;

        private ScreenShake screenShake;

        protected override void Awake()
        {
            base.Awake();

            playerUnit = GetComponent<PlayerUnit>();
            pool = new ComponentPool<RocketProjectile>(projectilePrefab, 5);
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
            yield return new WaitForSeconds(reloadTime);
            ReloadCompleted();
        }

        protected override void Fire()
        {
            screenShake.Shake(0);

            RocketProjectile launched = pool.Get();
            launched.Expired += RocketExpired;
            launched.transform.position = launchPosition.position;
            launched.transform.rotation = launchPosition.rotation;
            launched.Launch(bulletDamage, screenShake);
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
