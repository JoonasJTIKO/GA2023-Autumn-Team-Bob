using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Projectile : BaseFixedUpdateListener
    {
        private float aliveTime = 5;
        [SerializeField] private float speed;
        public Rigidbody rb;
        public Transform projectilePos;

        void Start()
        {
            rb.velocity = projectilePos.transform.forward * speed;
        }

        void FixedUpdate()
        {
            StartCoroutine(AliveTimer());
        }

        private new void OnEnable()
        {
            rb.velocity = projectilePos.transform.forward * speed;
        }

        private new void OnDisable()
        {
            CancelInvoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            //if (other.CompareTag("Enemy"))
            //{
            //    //other.GetComponent<Health>().TakeDamage(1, false);
            //    DestroyProjectile();
            //}
            //if (other.CompareTag("Wall"))
            //{
            //    DestroyProjectile();
            //}
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
