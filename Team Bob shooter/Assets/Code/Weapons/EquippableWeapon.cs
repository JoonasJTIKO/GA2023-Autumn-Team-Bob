using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    [System.Serializable]
    public class EquippableWeapon
    {
        public enum Weapon
        {
            Minigun = 0,
            Shotgun = 1,
            RocketLauncher = 2,
        }

        [SerializeField]
        public Weapon WeaponType;
    }
}
