using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class EnemyLungeAttack : MonoBehaviour
    {
        [SerializeField]
        private float angle = 40f;

        [SerializeField]
        private float speed = 2f;

        private Vector3 targetPos;

        public bool launch = false;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            rb.AddForce(Physics.gravity * rb.mass, ForceMode.Acceleration);

            if (launch)
            {
                targetPos = transform.position + Vector3.forward * 10;
                Lunge();
                launch = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            rb.velocity = Vector3.zero;
        }

        private void Lunge()
        {
            float distance = Vector3.Distance(transform.position, targetPos);
            float gravity = Physics.gravity.y * rb.mass;
            float height = targetPos.y - transform.position.y;

            float velocityX = Mathf.Sqrt(gravity * distance * distance /
                (2f * (height - distance * Mathf.Tan(angle * Mathf.Deg2Rad))));
            float velocityY = Mathf.Tan(angle * Mathf.Deg2Rad) * velocityX;

            rb.velocity = transform.TransformDirection(new Vector3(0f, velocityY, velocityX));
        }
    }
}
