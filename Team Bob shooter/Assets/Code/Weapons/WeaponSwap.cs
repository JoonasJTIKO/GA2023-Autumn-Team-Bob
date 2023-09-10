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

        protected override void OnEnable()
        {
            base.OnEnable();
            if (GameInstance.Instance == null) return;

            swapAction = playerUnit.Inputs.Weapons.Swap;
            shootAction = playerUnit.Inputs.Weapons.Shoot;
            reloadAction = playerUnit.Inputs.Weapons.Reload;
            dashAction = playerUnit.Inputs.Weapons.ShotgunDash;

            playerUnit.Inputs.Weapons.Enable();
            swapAction.performed += SwapWeapon;
            reloadAction.performed += ReloadActiveWeapon;

            ActivateWeapon(activeWeaponIndex);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (GameInstance.Instance == null) return;

            playerUnit.Inputs.Weapons.Disable();
            swapAction.performed -= SwapWeapon;
            reloadAction.performed -= ReloadActiveWeapon;
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
            }
        }

        private void SwapWeapon(InputAction.CallbackContext context)
        {
            viewmodels[activeWeaponIndex].SetActive(false);

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
            switch (index)
            {
                case 0:
                    dashAction.performed -= ShotgunDash;
                    break;
                case 1:
                    dashAction.performed += ShotgunDash;
                    break;
            }

            activeWeapon = equippedWeapons[index];
            activeWeapon.UpdateHudAmmo();
            viewmodels[index].SetActive(true);
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
