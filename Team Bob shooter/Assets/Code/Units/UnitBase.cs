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

        protected override void Awake()
        {
            base.Awake();

            health = maxHealth;
        }

        public virtual void TakeDamage(float amount)
        {
            ChangeHealth(-amount);
        }

        protected virtual void ChangeHealth(float amount)
        {
            health += amount;
            
            if (health > maxHealth) health = maxHealth;
            if (health < 0) health = 0;
        }
    }
}
