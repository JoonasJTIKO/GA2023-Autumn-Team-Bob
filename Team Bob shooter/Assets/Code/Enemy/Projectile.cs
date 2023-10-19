using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Projectile : BaseFixedUpdateListener
    {
        private float aliveTime = 5;
        [SerializeField] private float speed;
        [SerializeField] private float damage = 34f;
        public Rigidbody rb;
        public Transform projectilePos;
        public Transform player;

        void Start()
        {
            //var direction = player.transform.position - transform.position;
            //rb.velocity = direction * speed;
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            StartCoroutine(AliveTimer());
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            var direction = (player.transform.position - transform.position).normalized;
            rb.velocity = transform.forward * speed;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            CancelInvoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 3)
            {
                other.GetComponent<UnitHealth>().RemoveHealth(damage);
            }
            DestroyProjectile();
        }

        void DestroyProjectile()
        {
            gameObject.SetActive(false);
        }
        private IEnumerator AliveTimer()
        {
            yield return new WaitForSeconds(aliveTime);
            gameObject.SetActive(false);
        }
    }
}
