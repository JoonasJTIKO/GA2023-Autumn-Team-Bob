using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class UnitBase : BaseFixedUpdateListener
    {
        [SerializeField]
        private float maxHealth = 100;

        private float health;

        public float Health
        {
            get { return health; }
        }
    }
}
