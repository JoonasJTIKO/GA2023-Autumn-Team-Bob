using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace TeamBobFPS
{
    public class EnemyLungeAttack : MonoBehaviour
    {
        [SerializeField]
        private float angle = 40f;

        [SerializeField]
        private float cooldown = 2f;

        private Vector3 targetPos;

        public bool launch = false;

        private bool onCooldown = false;

        private Rigidbody rb;

        private PlayerUnit playerUnit;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            playerUnit = FindObjectOfType<PlayerUnit>();
        }

        private void FixedUpdate()
        {
            rb.AddForce(Physics.gravity * rb.mass, ForceMode.Acceleration);

            if (launch)
            {
                Lunge();
                launch = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            rb.velocity = Vector3.zero;
        }

        public bool Lunge()
        {
            if (onCooldown) return false;

            targetPos = GetTargetPosition();

            float distance = Vector3.Distance(transform.position, targetPos);
            float gravity = Physics.gravity.y * rb.mass;
            float height = targetPos.y - transform.position.y;

            float velocityX = Mathf.Sqrt(gravity * distance * distance /
                (2f * (height - distance * Mathf.Tan(angle * Mathf.Deg2Rad))));
            float velocityY = Mathf.Tan(angle * Mathf.Deg2Rad) * velocityX;

            transform.LookAt(targetPos);
            rb.velocity = transform.TransformDirection(new Vector3(0f, velocityY, velocityX));
            StartCoroutine(Cooldown());
            return true;
        }

        private Vector3 GetTargetPosition()
        {
            Vector3 playerPos = playerUnit.transform.position;
            Vector3 toPlayer = playerPos - transform.position;
            Vector3 targetPos = transform.position + toPlayer.normalized * (toPlayer.magnitude - 1);

            return targetPos;
        }

        private IEnumerator Cooldown()
        {
            float timer = cooldown;
            onCooldown = true;

            while (timer > 0)
            {
                timer -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            onCooldown = false;
        }
    }
}
