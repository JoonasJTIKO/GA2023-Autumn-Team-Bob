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

        [SerializeField]
        protected WeaponType weaponType;

        [SerializeField]
        protected BulletTracer bulletTrail;

        [SerializeField]
        protected Transform bulletOrigin;

        [SerializeField]
        private Animator viewmodelAnimator;

        protected ComponentPool<Transform> hitEffectPool;

        protected ComponentPool<BulletTracer> bulletTrailPool;

        public int CurrentReserveAmmo
        {
            get;
            protected set;
        }

        public int MaxReserveAmmo
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

        public virtual void UpdateHudAmmo()
        {

            InGameHudCanvas inGameHudCanvas = GameInstance.Instance.GetInGameHudCanvas();

            inGameHudCanvas.UpdateMagCount(currentMagAmmoCount);
            inGameHudCanvas.UpdateReserveCount(CurrentReserveAmmo);
        }

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
