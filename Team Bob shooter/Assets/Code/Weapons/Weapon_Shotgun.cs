using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Weapon_Shotgun : WeaponBase
    {
        protected override void BeginReload()
        {
            throw new System.NotImplementedException();
        }

        protected override void Fire()
        {
            Debug.Log("Shotgun shoot!");
        }
    }
}
