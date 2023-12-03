using System.Collections;
using System.Collections.Generic;
using TeamBobFPS.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace TeamBobFPS
{
    public enum WeaponType
    {
        Minigun = 0,
        Shotgun = 1,
        RocketLauncher = 2,
        Railgun = 3,
        Pistol = 4,
    }

    /// <summary>
    /// Base class for player weapons
    /// </summary>
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
        protected float falloffDistance;

        [SerializeField]
        protected GameObject hitEffect;

        [SerializeField]
        protected WeaponType weaponType;

        [SerializeField]
        protected BulletTracer bulletTrail;

        [SerializeField]
        protected Transform bulletOrigin;

        [SerializeField]
        protected Animator viewmodelAnimator;

        protected ComponentPool<Transform> hitEffectPool;

        protected ComponentPool<BulletTracer> bulletTrailPool;

        protected CameraRecoil recoil;

        public virtual int CurrentReserveAmmo
        {
            get;
            protected set;
        }

        public virtual int MaxReserveAmmo
        {
            get { return maxReserveAmmo; }
        }

        protected int currentMagAmmoCount;

        protected float timer = 0;

        protected bool readyToFire = true;

        protected bool reloading = false;

        protected bool active = false;

        public WeaponType WeaponType
        {
            get { return weaponType; }
        }

        protected override void Awake()
        {
            base.Awake();

            CurrentReserveAmmo = startingReserveAmmo;
            currentMagAmmoCount = magSize;
            hitEffectPool = new ComponentPool<Transform>(hitEffect.transform, 10);
            bulletTrailPool = new ComponentPool<BulletTracer>(bulletTrail, 10);

            recoil = GetComponentInChildren<CameraRecoil>();
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (readyToFire == false)
            {
                timer -= deltaTime;
                if (timer <= 0)
                {
                    readyToFire = true;
                    if (currentMagAmmoCount == 0) BeginReload();
                }
            }
        }

        /// <summary>
        /// Activate / deactivate this weapon. All weapons scripts are connected to player prefab
        /// </summary>
        /// <param name="state">true to activate, false to deactivate</param>
        public virtual void Activate(bool state)
        {
            if (state)
            {
                StartCoroutine(OnActivate());
            }
            else
            {
                active = false;
            }
        }

        /// <summary>
        /// Updates the ammo counters on hud
        /// </summary>
        public virtual void UpdateHudAmmo()
        {

            InGameHudCanvas inGameHudCanvas = GameInstance.Instance.GetInGameHudCanvas();

            inGameHudCanvas.UpdateMagCount(currentMagAmmoCount);
            inGameHudCanvas.UpdateReserveCount(CurrentReserveAmmo);
        }

        /// <summary>
        /// Can be used by weapons that need to do something when button is held
        /// </summary>
        /// <param name="state">True if held, false if not</param>
        public virtual void FireButtonHeld(bool state)
        {
        }

        /// <summary>
        /// Checks if weapon is ready to fire. If it is, updates ammo counts, sets firerate timer and calls Fire method where shooting is done.
        /// </summary>
        public virtual void Shoot()
        {
            if (currentMagAmmoCount == 0 || !readyToFire || reloading || !active) return;

            currentMagAmmoCount -= shotAmmoCost;
            Fire();
            UpdateHudAmmo();
            readyToFire = false;
            if (fireRate != 0) timer = 1 / fireRate;
            else timer = 0;

            if (viewmodelAnimator != null)
            {
                viewmodelAnimator.SetTrigger("Fire");
            }
        }
        
        // Methods for controlling animation states

        public virtual void SetWalking(bool state)
        {
            if (viewmodelAnimator != null)
            {
                viewmodelAnimator.SetBool("Walking", state);
            }
        }

        public virtual void Jump()
        {
            if (viewmodelAnimator != null)
            {
                viewmodelAnimator.ResetTrigger("Land");
                viewmodelAnimator.SetTrigger("Jump");
            }
        }

        public virtual void Land()
        {
            if (viewmodelAnimator != null)
            {
                viewmodelAnimator.ResetTrigger("Jump");
                viewmodelAnimator.SetTrigger("Land");
            }
        }

        /// <summary>
        /// Called after reload animation / wait time
        /// </summary>
        protected virtual void ReloadCompleted()
        {
            int reloadAmount = magSize - currentMagAmmoCount;
            if (CurrentReserveAmmo < reloadAmount) reloadAmount = CurrentReserveAmmo;

            CurrentReserveAmmo -= reloadAmount;
            currentMagAmmoCount += reloadAmount;
            if (currentMagAmmoCount > magSize) currentMagAmmoCount = magSize;

            UpdateHudAmmo();
            reloading = false;
        }

        /// <summary>
        /// Returns bullet tracer effect to pool
        /// </summary>
        /// <param name="tracer">Tracer to be returned</param>
        protected void RecycleTracer(BulletTracer tracer)
        {
            if (!bulletTrailPool.Return(tracer))
            {
                Debug.LogError("Can not return tracer to pool!");
            }
            else
            {
                tracer.Expired -= RecycleTracer;
            }
        }

        /// <summary>
        /// Adds ammo to reserves
        /// </summary>
        /// <param name="amount">Amount to add</param>
        public virtual void AddAmmo(int amount)
        {
            CurrentReserveAmmo += amount;
            if (CurrentReserveAmmo > maxReserveAmmo)
            {
                CurrentReserveAmmo = maxReserveAmmo;
            }
        }

        private IEnumerator OnActivate()
        {
            float timer = 0.2f;
            while (timer > 0)
            {
                timer -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            active = true;
        }

        public abstract void AbortReload();

        public abstract void BeginReload();

        protected abstract void Fire();
    }
}
