using System.Collections;
using System.Collections.Generic;
using TeamBobFPS.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace TeamBobFPS
{
    public abstract class WeaponBase : BaseUpdateListener
    {
        [SerializeField]
        protected float bulletDamage;

        [SerializeField]
        protected float fireRate;

        [SerializeField]
        protected int maxReserveAmmo;

        [SerializeField]
        protected int startingReserveAmmo;

        [SerializeField]
        protected int magSize;

        [SerializeField]
        protected int shotAmmoCost;

        [SerializeField]
        protected GameObject hitEffect;

        protected ComponentPool<Transform> hitEffectPool;

        protected int currentReserveAmmo;

        protected int currentMagAmmoCount;

        protected float timer = 0;

        protected bool readyToFire = true;

        protected bool reloading = false;

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

        public virtual void UpdateHudAmmo()
        {
            InGameHudCanvas inGameHudCanvas = GameInstance.Instance.GetInGameHudCanvas();

            inGameHudCanvas.UpdateMagCount(currentMagAmmoCount);
            inGameHudCanvas.UpdateReserveCount(currentReserveAmmo);
        }

        public virtual void Shoot()
        {
            if (currentMagAmmoCount == 0 || !readyToFire || reloading) return;

            currentMagAmmoCount -= shotAmmoCost;
            Fire();
            UpdateHudAmmo();
            readyToFire = false;
            timer = 1 / fireRate;
        }

        protected virtual void ReloadCompleted()
        {
            int reloadAmount = magSize - currentMagAmmoCount;
            if (currentReserveAmmo < reloadAmount) reloadAmount = currentReserveAmmo;

            currentReserveAmmo -= reloadAmount;
            currentMagAmmoCount += reloadAmount;
            if (currentMagAmmoCount > magSize) currentMagAmmoCount = magSize;

            UpdateHudAmmo();
            reloading = false;
        }

        public abstract void BeginReload();

        protected abstract void Fire();
    }
}
