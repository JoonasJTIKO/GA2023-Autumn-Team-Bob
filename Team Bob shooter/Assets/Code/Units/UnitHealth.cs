using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class UnitHealth : MonoBehaviour, IHealth
    {
        [SerializeField]
        protected float health = 100f;

        public float damageMultiplier = 1f;

        [SerializeField]
        protected float invincibilityTime = 1f;

        public bool Invincible
        {
            get;
            protected set;
        }

        public float Health
        { get; protected set; }

        public float MaxHealth
        {
            get; protected set;
        }

        public bool HealthFull 
        {
            get { return Health == MaxHealth; }
        }

        public Vector3 ExplosionPoint = Vector3.zero;

        public float ExplosionStrength = 1f;

        public event Action<float> OnHealthUpdate;

        public event Action<float> OnTakeDamage;

        public event Action<float> OnHeal;

        public event Action<float, Vector3, EnemyGibbing.DeathType> OnDied;

        protected Rigidbody rb;

        protected float knockbackAmount = 100f;

        protected Vector3 knockbackDirection;

        [SerializeField]
        protected float knockbackAngle = 45f;

        public virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
            Invincible = false;
            Health = health;
            MaxHealth = Health;
        }

        /// <summary>
        /// Adds given amount of health to current health if not already at max health,
        /// health can not go over max.
        /// </summary>
        /// <param name="amount">Amount to add</param>
        /// <returns>True if health could be added, false if not</returns>
        public bool AddHealth(float amount)
        {
            if (Health < MaxHealth)
            {
                float startingHealth = Health;
                Health += amount;
                if (Health > MaxHealth)
                {
                    Health = MaxHealth;
                }
                OnHealthUpdate?.Invoke(Health - startingHealth);
                OnHeal?.Invoke(amount);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes given amount of health from current health if not at 0 health,
        /// not currently invulnerable and the units damagemultiplier is not 0.
        /// Health can not go below 0.
        /// </summary>
        /// <param name="amount">Amount to remove</param>
        /// <returns>True if health could be removed, false if not</returns>
        public virtual bool RemoveHealth(float amount, EnemyGibbing.DeathType potentialDeathType = EnemyGibbing.DeathType.Normal)
        {
            if (Health > 0 && !Invincible && damageMultiplier != 0)
            {
                Health -= amount * damageMultiplier;
                if (Health < 0)
                {
                    Health = 0;
                }
                if (Health == 0)
                {
                    Die(ExplosionPoint,potentialDeathType);
                    return true;
                }
                Invincible = true;
                ExplosionPoint = Vector3.zero;
                StartCoroutine(InvincibilityTimer());
                OnHealthUpdate?.Invoke(-amount * damageMultiplier);
                OnTakeDamage?.Invoke(amount);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Knockbackdirection should always initially be stored as a flat vector on xz plane.
        /// It is angled vertically by specified angle, then the unit receives impulse force in that direction.
        /// </summary>
        public void Knockback()
        {
            knockbackDirection = Quaternion.AngleAxis(knockbackAngle, transform.right) * knockbackDirection;

            rb.velocity = new(0, 0, 0);
            rb.AddForce(knockbackDirection * knockbackAmount, ForceMode.Impulse);
        }

        private void Die(Vector3 explosionPoint, EnemyGibbing.DeathType deathType)
        {
            OnDied?.Invoke(ExplosionStrength, explosionPoint, deathType);
            ExplosionStrength = 1f;
        }

        private IEnumerator InvincibilityTimer()
        {
            yield return new WaitForSeconds(invincibilityTime);
            Invincible = false;
        }
    }
}
