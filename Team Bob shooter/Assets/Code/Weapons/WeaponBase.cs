using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TeamBobFPS
{
    public abstract class WeaponBase : BaseUpdateListener
    {
        [SerializeField]
        private float bulletDamage;

        [SerializeField]
        private float fireRate;

        [SerializeField]
        private float maxReserveAmmo;

        [SerializeField]
        private float startingReserveAmmo;

        [SerializeField]
        private float magSize;

        [SerializeField]
        private float shotAmmoCost;

        [SerializeField]
        private GameObject hitEffect;

        protected ComponentPool<Transform> hitEffectPool;

        private float currentReserveAmmo;

        private float currentMagAmmoCount;

        private float timer = 0;

        private bool readyToFire = true;

        protected override void Awake()
        {
            base.Awake();

            currentReserveAmmo = startingReserveAmmo;
            currentMagAmmoCount = magSize;
            hitEffectPool = new ComponentPool<Transform>(hitEffect.transform, 10);
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (readyToFire == false)
            {
                timer -= deltaTime;
                if (timer <= 0) readyToFire = true;
            }
        }

        public virtual void Shoot()
        {
            if (currentMagAmmoCount == 0 || !readyToFire) return;

            currentMagAmmoCount -= shotAmmoCost;
            Fire();
            readyToFire = false;
            timer = 1 / fireRate;
        }

        protected virtual void ReloadCompleted()
        {
            float reloadAmount;
            if (currentReserveAmmo < magSize) reloadAmount = currentReserveAmmo;
            else reloadAmount = magSize;

            currentReserveAmmo -= reloadAmount;
            currentMagAmmoCount += reloadAmount;
        }

        protected abstract void BeginReload();

        protected abstract void Fire();
    }
}
