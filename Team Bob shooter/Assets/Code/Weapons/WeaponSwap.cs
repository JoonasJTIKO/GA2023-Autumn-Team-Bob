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

        private InputAction swapAction, shootAction, reloadAction;

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
            equippedWeapons = new WeaponBase[GameInstance.Instance.GetWeaponLoadout().EquippedWeapons.Length];

            int slot = 0;
            foreach (EquippableWeapon weapon in GameInstance.Instance.GetWeaponLoadout().EquippedWeapons)
            {
                EquipWeapon(weapon.WeaponType, slot);
                slot++;
            }
            activeWeapon = equippedWeapons[activeWeaponIndex];
            activeWeapon.UpdateHudAmmo();
            viewmodels[activeWeaponIndex].SetActive(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (GameInstance.Instance == null) return;

            playerUnit.Inputs.Weapons.Enable();
            swapAction = playerUnit.Inputs.Weapons.Swap;
            shootAction = playerUnit.Inputs.Weapons.Shoot;
            reloadAction = playerUnit.Inputs.Weapons.Reload;
            swapAction.performed += SwapWeapon;
            reloadAction.performed += ReloadActiveWeapon;
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
            activeWeapon = equippedWeapons[activeWeaponIndex];
            activeWeapon.UpdateHudAmmo();
            viewmodels[activeWeaponIndex].SetActive(true);
        }

        private void ShootActiveWeapon()
        {
            activeWeapon.Shoot();
        }

        private void ReloadActiveWeapon(InputAction.CallbackContext context)
        {
            activeWeapon.BeginReload();
        }
    }
}
