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

        public WeaponBase ActiveWeapon
        {
            get;
            private set;
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
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (GameInstance.Instance == null) return;

            playerUnit.Inputs.Weapons.Disable();
            swapAction.performed -= SwapWeapon;
            reloadAction.performed -= ReloadActiveWeapon;

            WeaponLoadout.WeaponEquipped -= EquipWeapon;
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (shootAction.phase == InputActionPhase.Performed)
            {
                ShootActiveWeapon();
            }
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
            }


            ActivateWeapon(activeWeaponIndex);
        }

        private void SwapWeapon(InputAction.CallbackContext context)
        {
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

            switch (weaponType)
            {
                case WeaponType.Minigun:
                    if (dashAction != null) dashAction.performed -= ShotgunDash;
                    break;
                case WeaponType.Shotgun:
                    if (dashAction != null) dashAction.performed += ShotgunDash;
                    break;
                case WeaponType.RocketLauncher:
                    if (dashAction != null) dashAction.performed -= ShotgunDash;
                    break;
            }

            activeWeapon = equippedWeapons[index];
            activeWeapon.UpdateHudAmmo();
            ActivateViewmodel(weaponType);
            playerUnit.CurrentWeaponSlot = activeWeaponIndex;
        }

        private void ActivateViewmodel(WeaponType weapon)
        {
            switch (weapon)
            {
                case WeaponType.Minigun:
                    viewmodels[0].SetActive(true);
                    viewmodels[1].SetActive(false);
                    viewmodels[2].SetActive(false);
                    break;
                case WeaponType.Shotgun:
                    viewmodels[0].SetActive(false);
                    viewmodels[1].SetActive(true);
                    viewmodels[2].SetActive(false);
                    break;
                case WeaponType.RocketLauncher:
                    viewmodels[0].SetActive(false);
                    viewmodels[1].SetActive(false);
                    viewmodels[2].SetActive(true);
                    break;
            }
        }

        private void ShootActiveWeapon()
        {
            activeWeapon.Shoot();
        }

        private void ReloadActiveWeapon(InputAction.CallbackContext context)
        {
            activeWeapon.BeginReload();
        }

        private void ShotgunDash(InputAction.CallbackContext context)
        {
            playerDash.Dash();
        }
    }
}
