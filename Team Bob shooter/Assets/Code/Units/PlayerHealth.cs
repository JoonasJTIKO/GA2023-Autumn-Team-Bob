using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class PlayerHealth : UnitHealth
    {
        //private RespawnPositions respawnPositions;

        //private InGameHud inGameHud;

        //private BarMeter healthBar;

        //private PlayerAnimationController animationController;

        private PlayerUnit player;

        //private PlayerSFXController audioController;

        [SerializeField]
        private float lockMovementDuration = 0.4f;

        public bool LockMovement
        {
            get;
            private set;
        }

        public bool HealthFull
        {
            get { return Health == MaxHealth; }
        }

        public bool PlayerIsDead
        {
            get { return Health == 0; }
        }

        private Coroutine lockMovementRoutine;

        public static event Action<bool> OnPlayerDied;
        public static event Action OnHealthChanged;

        public override void Awake()
        {
            base.Awake();

            LockMovement = false;
            //respawnPositions = GetComponent<RespawnPositions>();
            //inGameHud = FindObjectOfType<InGameHud>();
            //animationController = GetComponent<PlayerAnimationController>();
            player = GetComponent<PlayerUnit>();
            //audioController = GetComponentInChildren<PlayerSFXController>();

            //if (inGameHud != null)
            {
              //  healthBar = inGameHud.GetComponentInChildren<BarMeter>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "EnemyAttack" || other.gameObject.tag == "EnvironmentHazard")
            {
                DamageDetected(other.gameObject);
            }
        }

        /// <summary>
        /// The player has collided with a damaging trigger. 
        /// Stores the correct knockback data from this collision and calls Knockback if damage is actually taken.
        /// </summary>
        /// <param name="other">The game object that dealt potential damage</param>
        private void DamageDetected(GameObject other)
        {
            //Damaging damage = other.gameObject.GetComponent<Damaging>();

            //knockbackAmount = damage.KnockbackAmount;
            knockbackDirection = transform.position - other.transform.position;
            knockbackDirection = Vector3.ProjectOnPlane(knockbackDirection, Vector3.up);
            knockbackDirection.Normalize();

            //if (TakeDamage(damage.Damage) && damage.AppliesKnockback && Health != 0)
            {
                Knockback();
              //  StartCoroutine(player.LockMovementTimer(lockMovementDuration));
            }

            if (Health == 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Checks if health can currently be deducted from the player, and plays proper sound
        /// as well as updating the health bar.
        /// </summary>
        /// <param name="amount">Amount of damage to potentially take</param>
        /// <returns>True if damage was taken, false if not</returns>
        private bool TakeDamage(float amount)
        {
            if (RemoveHealth(amount))
            {
                OnHealthChanged?.Invoke();
                //audioController.PlayTakeDamageSound();

                if (Health != 0)
                {
                  //  animationController.TakeDamage();
                }

                //if (healthBar != null)
                {
                  //  healthBar.ChangeLength(Health / MaxHealth);
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// If the player is able to receive more health, updates health bar.
        /// </summary>
        /// <param name="amount">Amount to heal player by</param>
        /// <returns>True if player could be healed, false if not</returns>
        public bool Heal(float amount)
        {
            if (AddHealth(amount))
            {
                //TODO: again do all the stuff
                OnHealthChanged?.Invoke();

                //if (healthBar != null)
                {
                  //  healthBar.ChangeLength(Health / MaxHealth);
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Resets the players health amount, resets animations and informs other scripts that player is no longer dead.
        /// Animation resets should not be called here but whatever.
        /// </summary>
        public void Restart()
        {
            Heal(100);
            LockMovement = false;
            if (lockMovementRoutine != null)
            {
                StopCoroutine(lockMovementRoutine);
            }
            //animationController.ResetAnims();
            OnPlayerDied?.Invoke(false);
        }

        /// <summary>
        /// Locks players movement, plays death animation and loads game over UI.
        /// </summary>
        private void Die()
        {
            OnPlayerDied?.Invoke(true);

            //if (respawnPositions.CurrentDieSpawn == null)
            {
                Debug.LogError("No respawn point set!");
            //}
           // else
           // {
                //lockMovementRoutine = StartCoroutine(player.LockMovementTimer(9999));
                LockMovement = true;
                rb.velocity = Vector3.zero;
              //  animationController.Die();
                //GameStateManager.Instance.Go(StateType.GameOver, loadScene: false);
            }
        }
    }
}
