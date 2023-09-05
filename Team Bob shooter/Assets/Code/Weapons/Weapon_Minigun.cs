using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Weapon_Minigun : WeaponBase
    {
        protected override void BeginReload()
        {
            throw new System.NotImplementedException();
        }

        protected override void Fire()
        {
            Debug.Log("Minigun shoot!");
        }
    }
}
