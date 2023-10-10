using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace TeamBobFPS
{
    public class EnemyLungeAttack : BaseFixedUpdateListener
    {
        [SerializeField]
        private float damage = 25f;

        [SerializeField]
        private float angle = 40f;

        [SerializeField]
        private float heightCap = 2f;

        [SerializeField]
        private float cooldown = 2f;

        [SerializeField]
        private DamageBox damageBox;

        private Vector3 targetPos;

        private bool onCooldown = false;

        private Rigidbody rb;

        private PlayerUnit playerUnit;

        public event Action AttackEnd;

        private Coroutine cooldownRoutine = null;

        protected override void Awake()
        {
            base.Awake();

            rb = GetComponent<Rigidbody>();
            playerUnit = FindObjectOfType<PlayerUnit>();
            damageBox.Damage = damage;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (cooldownRoutine != null)
            {
                StopCoroutine(cooldownRoutine);
            }
            onCooldown = false;
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            rb.AddForce(Physics.gravity * rb.mass, ForceMode.Acceleration);
        }

        private void OnCollisionEnter(Collision collision)
        {
            rb.velocity = Vector3.zero;
            damageBox.gameObject.SetActive(false);
            AttackEnd?.Invoke();
        }

        public bool Lunge()
        {
            if (onCooldown) return false;

            targetPos = GetTargetPosition();

            float distance = Vector3.Distance(transform.position, targetPos);
            float gravity = Physics.gravity.y * rb.mass;
            float height = targetPos.y - transform.position.y;
            if (height > heightCap) height = heightCap;

            float velocityX = Mathf.Sqrt(gravity * distance * distance /
                (2f * (height - distance * Mathf.Tan(angle * Mathf.Deg2Rad))));
            float velocityY = Mathf.Tan(angle * Mathf.Deg2Rad) * velocityX;

            transform.LookAt(targetPos);

            Vector3 newVelocity = transform.TransformDirection(new Vector3(0f, velocityY, velocityX));
            if (newVelocity.x == float.NaN ||  newVelocity.y == float.NaN || newVelocity.z == float.NaN)
            {
                return false;
            }

            rb.velocity = transform.TransformDirection(new Vector3(0f, velocityY, velocityX));
            damageBox.gameObject.SetActive(true);
            cooldownRoutine = StartCoroutine(Cooldown());
            return true;
        }

        private Vector3 GetTargetPosition()
        {
            Vector3 playerPos = playerUnit.transform.position;
            Vector3 toPlayer = playerPos - transform.position;
            Vector3 targetPos = transform.position + toPlayer * 1.1f;

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
