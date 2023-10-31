using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class WeaponLoadout : MonoBehaviour
    {
        private EquippableWeapon[] equippedWeapons = new EquippableWeapon[2];

        public EquippableWeapon[] EquippedWeapons
        {
            get { return equippedWeapons; }
        }

        [SerializeField]
        private List<EquippableWeapon> startingLoadout;

        public static event Action<EquippableWeapon> WeaponUnequipped;

        public static event Action<EquippableWeapon.Weapon, int> WeaponEquipped;

        private void Awake()
        {
            int slot = 0;
            foreach (EquippableWeapon weapon in startingLoadout)
            {
                EquipWeapon(weapon, slot);
                slot++;
            }
        }

        /// <summary>
        /// Equips weapon to slot
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="slot"></param>
        public void EquipWeapon(EquippableWeapon weapon, int slot)
        {
            WeaponUnequipped?.Invoke(equippedWeapons[slot]);

            equippedWeapons[slot] = weapon;

            WeaponEquipped?.Invoke(equippedWeapons[slot].WeaponType, slot);
        }

        /// <summary>
        /// Checks if a weapon is currently equipped
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
        public bool CheckEquipStatus(EquippableWeapon weapon)
        {
            foreach (EquippableWeapon equippedWeapon in equippedWeapons)
            {
                if (equippedWeapon.WeaponType == weapon.WeaponType)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
