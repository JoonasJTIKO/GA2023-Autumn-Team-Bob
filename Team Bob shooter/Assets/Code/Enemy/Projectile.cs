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
        public Transform player;

        void Start()
        {
            //var direction = player.transform.position - transform.position;
            //rb.velocity = direction * speed;
        }

        void FixedUpdate()
        {
            StartCoroutine(AliveTimer());
        }

        private new void OnEnable()
        {
            var direction = (player.transform.position - transform.position).normalized;
            rb.velocity = direction * speed * Time.deltaTime;
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
