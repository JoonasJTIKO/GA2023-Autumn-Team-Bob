using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TeamBobFPS
{
    public class EnemyHealth : UnitHealth
    {
        //private BarMeter healthBar;

       // [SerializeField]
        //private HealthOrb healthOrb;

        [SerializeField]
        private int minimumOrbs = 1;

        [SerializeField]
        private int maximumOrbs = 3;

        public bool takeKnockBack = false;

       // private ComponentPool<HealthOrb> pool;

        //private EnemyTargetPoint targetPoint;

        private PlayerUnit player;

        public Action OnTakeDamage;

        public static Action<Transform> OnDied;

        public bool Dead
        {
            get;
            private set;
        }

        public override void Awake()
        {
            base.Awake();

            //pool = new ComponentPool<HealthOrb>(healthOrb, maximumOrbs);
            //targetPoint = GetComponentInChildren<EnemyTargetPoint>();
            player = FindObjectOfType<PlayerUnit>();

            Dead = false;
        }

        private void OnTriggerEnter(Collider other)
        {
                if (Health == 0 && !Dead)
                {
                    Die();
                }
        }

        private bool TakeDamage(float amount)
        {
            if (RemoveHealth(amount))
            {
                OnTakeDamage?.Invoke();
                //TODO: do all the health stuff lol

               // if (healthBar != null)
                //{
                  //  healthBar.ChangeLength(Health);
                //}

                return true;
            }
            return false;
        }

        private void Die()
        {
            Dead = true;
            OnDied?.Invoke(this.transform);

            int amount = Random.Range(minimumOrbs, maximumOrbs + 1);
            for (int i = 0; i < amount; i++)
            {
               // HealthOrb spawned = pool.Get();
                //spawned.Expired += OnExpired;
                //spawned.transform.position = targetPoint.transform.position;
                //spawned.Create();

                Vector3 randomDirection = new Vector3(Random.Range(-1, 1), 0.8f, Random.Range(-1, 1));
                randomDirection.Normalize();
                //spawned.GetComponent<Rigidbody>().AddForce(randomDirection * 5, ForceMode.Impulse);
            }
        }

       // private void OnExpired(HealthOrb healthOrb)
        //{
          //  healthOrb.Expired -= OnExpired;

            //if (!pool.Return(healthOrb))
            //{
              //  Debug.LogError("Can't return healthOrb to pool");
           // }
        //}

        public void EnemyReset(bool dead)
        {
            Dead = dead;
            if (!dead) 
            {
                health = MaxHealth;
               // GetComponent<Explode>().ResetToNormal();
            }
            else OnDied?.Invoke(this.transform);
            gameObject.SetActive(!dead);
        }
    }
}
