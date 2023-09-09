using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Projectile : BaseUpdateListener
    {
        private float aliveTime = 5;
        [SerializeField] private float speed;
        public Rigidbody rb;

        void Start()
        {
            rb.velocity = transform.forward * speed;

        }

        void FixedUpdate()
        {
            StartCoroutine(AliveTimer());
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                //other.GetComponent<Health>().TakeDamage(1, false);
                DestroyProjectile();
            }
            if (other.CompareTag("Wall"))
            {
                DestroyProjectile();
            }
        }

        void DestroyProjectile()
        {
            Destroy(gameObject);
        }
        private IEnumerator AliveTimer()
        {
            yield return new WaitForSeconds(aliveTime);
            Destroy(gameObject);
        }
    }
}
