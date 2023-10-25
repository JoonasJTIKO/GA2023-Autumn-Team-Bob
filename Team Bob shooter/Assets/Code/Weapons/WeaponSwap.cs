using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamBobFPS
{
    public class WeaponSwap : BaseUpdateListener
    {
        [SerializeField]
        private List<WeaponBase> weapons;

        [SerializeField]
        private List<GameObject> viewmodels;

        private WeaponBase[] equippedWeapons;

        private PlayerUnit playerUnit;

        private PlayerDash playerDash;

        private InputAction swapAction, shootAction, reloadAction, dashAction;

        private int activeWeaponIndex = 0;

        private WeaponBase activeWeapon;

        private bool firstInitialize = true;

        private bool playerDead = false;

        private bool fullAutoMode = true;

        private bool readyToFire = true;

        private bool lockInputs = false;

        public WeaponBase ActiveWeapon
        {
            get;
            private set;
        }

        public bool CurrentReserveAmmoFull
        {
            get { return activeWeapon.CurrentReserveAmmo == activeWeapon.MaxReserveAmmo; }
        }

        protected override void Awake()
        {
            base.Awake();
            if (GameInstance.Instance == null) return;

            playerUnit = GetComponent<PlayerUnit>();
            playerDash = GetComponent<PlayerDash>();
            equippedWeapons = new WeaponBase[GameInstance.Instance.GetWeaponLoadout().EquippedWeapons.Length];

            int slot = 0;
            foreach (EquippableWeapon weapon in GameInstance.Instance.GetWeaponLoadout().EquippedWeapons)
            {
                EquipWeapon(weapon.WeaponType, slot);
                slot++;
            }
        }

        private void Start()
        {
            swapAction = playerUnit.Inputs.Weapons.Swap;
            shootAction = playerUnit.Inputs.Weapons.Shoot;
            reloadAction = playerUnit.Inputs.Weapons.Reload;
            dashAction = playerUnit.Inputs.Weapons.ShotgunDash;

            if (firstInitialize)
            {
                playerUnit.Inputs.Weapons.Enable();
                swapAction.performed += SwapWeapon;
                reloadAction.performed += ReloadActiveWeapon;

                firstInitialize = false;
            }

            ActivateWeapon(activeWeaponIndex);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (GameInstance.Instance == null) return;


            if (!firstInitialize)
            {
                playerUnit.Inputs.Weapons.Enable();
                swapAction.performed += SwapWeapon;
                reloadAction.performed += ReloadActiveWeapon;
            }

            WeaponLoadout.WeaponEquipped += EquipWeapon;
            playerUnit.OnPlayerDied += OnPlayerDied;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (GameInstance.Instance == null) return;

            playerUnit.Inputs.Weapons.Disable();
            swapAction.performed -= SwapWeapon;
            reloadAction.performed -= ReloadActiveWeapon;

            WeaponLoadout.WeaponEquipped -= EquipWeapon;
            playerUnit.OnPlayerDied -= OnPlayerDied;

            playerUnit.GravityScale = 1f;
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (shootAction.phase == InputActionPhase.Waiting)
            {
                activeWeapon.FireButtonHeld(false);
                readyToFire = true;
            }

            if (shootAction.phase == InputActionPhase.Performed && !playerDead && readyToFire && !lockInputs)
            {
                activeWeapon.FireButtonHeld(true);
                ShootActiveWeapon();
                if (!fullAutoMode)
                {
                    readyToFire = false;
                }
            }
        }

        public void LockInputs(bool state)
        {
            lockInputs = state;
        }

        public void AddAmmo(int amount)
        {
            activeWeapon.AddAmmo(amount);

            activeWeapon.UpdateHudAmmo();
        }

        public void SetCurrentWeaponWalking(bool state)
        {
            activeWeapon.SetWalking(state);
        }

        public void CurrentWeaponJump()
        {
            activeWeapon.Jump();
        }

        public void CurrentWeaponLand()
        {
            activeWeapon.Land();
        }

        private void OnPlayerDied()
        {
            swapAction.performed -= SwapWeapon;
            reloadAction.performed -= ReloadActiveWeapon;

            playerDead = true;
        }

        private void EquipWeapon(EquippableWeapon.Weapon weapon, int slot)
        {
            switch (weapon)
            {
                case EquippableWeapon.Weapon.Minigun:
                    equippedWeapons[slot] = weapons[0];
                    break;
                case EquippableWeapon.Weapon.Shotgun:
                    equippedWeapons[slot] = weapons[1];
                    break;
                case EquippableWeapon.Weapon.RocketLauncher:
                    equippedWeapons[slot] = weapons[2];
                    break;
                case EquippableWeapon.Weapon.Railgun:
                    equippedWeapons[slot] = weapons[3];
                    break;
                case EquippableWeapon.Weapon.Pistol:
                    equippedWeapons[slot] = weapons[4];
                    break;
            }


            ActivateWeapon(activeWeaponIndex);
        }

        private void SwapWeapon(InputAction.CallbackContext context)
        {
            if (lockInputs) return;

            equippedWeapons[activeWeaponIndex].AbortReload();
            equippedWeapons[activeWeaponIndex].Activate(false);

            if (context.ReadValue<float>() > 0)
            {
                activeWeaponIndex++;
                if (activeWeaponIndex >= equippedWeapons.Length) activeWeaponIndex = 0;
            }
            else if (context.ReadValue<float>() < 0)
            {
                activeWeaponIndex--;
                if (activeWeaponIndex < 0) activeWeaponIndex = equippedWeapons.Length - 1;
            }

            ActivateWeapon(activeWeaponIndex);
        }

        private void ActivateWeapon(int index)
        {
            WeaponType weaponType = equippedWeapons[index].WeaponType;

            playerUnit.GravityScale = 1f;
            if (dashAction != null) dashAction.performed -= ShotgunDash;
            if (playerUnit != null)
            {
                playerUnit.MoveSpeedModifier = 1f;
                playerUnit.ResetSpeed();
                playerUnit.EnableDoubleJump = false;
            }
            fullAutoMode = true;

            switch (weaponType)
            {
                case WeaponType.Minigun:
                    break;
                case WeaponType.Shotgun:
                    if (dashAction != null) dashAction.performed += ShotgunDash;
                    break;
                case WeaponType.RocketLauncher:
                    break;
                case WeaponType.Railgun:
                    playerUnit.GravityScale = 0.3f;
                    break;
                case WeaponType.Pistol:
                    if (playerUnit != null)
                    {
                        playerUnit.MoveSpeedModifier = 1.5f;
                        playerUnit.ResetSpeed();
                        playerUnit.EnableDoubleJump = true;
                    }
                    fullAutoMode = false;
                    break;
            }

            activeWeapon = equippedWeapons[index];
            activeWeapon.Activate(true);
            activeWeapon.UpdateHudAmmo();
            ActivateViewmodel(weaponType);
            playerUnit.CurrentWeaponSlot = activeWeaponIndex;
        }

        private void ActivateViewmodel(WeaponType weapon)
        {
            viewmodels[0].SetActive(false);
            viewmodels[1].SetActive(false);
            viewmodels[2].SetActive(false);
            viewmodels[3].SetActive(false);
            viewmodels[4].SetActive(false);

            switch (weapon)
            {
                case WeaponType.Minigun:
                    viewmodels[0].SetActive(true);
                    break;
                case WeaponType.Shotgun:
                    viewmodels[1].SetActive(true);
                    break;
                case WeaponType.RocketLauncher:
                    viewmodels[2].SetActive(true);
                    break;
                case WeaponType.Railgun:
                    viewmodels[3].SetActive(true);
                    break;
                case WeaponType.Pistol:
                    viewmodels[4].SetActive(true);
                    break;
            }
        }

        private void ShootActiveWeapon()
        {
            if (lockInputs) return;

            activeWeapon.Shoot();
        }

        private void ReloadActiveWeapon(InputAction.CallbackContext context)
        {
            if (lockInputs) return;

            activeWeapon.BeginReload();
        }

        private void ShotgunDash(InputAction.CallbackContext context)
        {
            if (lockInputs) return;

            playerDash.Dash();
        }
    }
}
