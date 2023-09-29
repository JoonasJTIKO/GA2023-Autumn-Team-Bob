using System;
using System.Collections;
using System.Collections.Generic;
using TeamBobFPS;
using UnityEngine;

namespace TeamBobFPS
{
    public class HealthOrb : BaseFixedUpdateListener
    {
        [SerializeField]
        private float healAmount = 10;

        [SerializeField]
        private float pickUpRange = 3;

        [SerializeField]
        private float speed = 1;

        [SerializeField]
        private float acceleration = 1.1f;

        [SerializeField]
        private float canBeCollectedTimer = 1f;

        [SerializeField]
        private float aliveTime = 10;

        private float aliveTimer = 0;

        private Coroutine aliveTimerRoutine = null;

        private bool flyToPlayer = false, canBeCollected = false;

        private Mover mover;

        private Transform playerPosition;

        private UnitHealth playerHealth;

        private Rigidbody rb;

        public event Action<HealthOrb> Expired;


        protected override void Awake()
        {
            base.Awake();
            mover = GetComponent<Mover>();
            playerPosition = FindObjectOfType<PlayerUnit>().gameObject.transform;
            playerHealth = playerPosition.gameObject.GetComponent<UnitHealth>();
            rb = GetComponent<Rigidbody>();
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            if (canBeCollected && !flyToPlayer && !playerHealth.HealthFull && (playerPosition.position - transform.position).magnitude <= pickUpRange)
            {
                rb.constraints = RigidbodyConstraints.None;
                mover.Setup(speed);
                flyToPlayer = true;
            }

            if (flyToPlayer)
            {
                Vector3 moveDirection = (playerPosition.position - transform.position).normalized;
                mover.Setup(speed);
                speed *= acceleration;
                mover.Move(moveDirection);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player" && flyToPlayer)
            {
                //GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_COLLECT_HEALTH, transform.position, make2D: true);
                playerHealth.AddHealth(healAmount);
                flyToPlayer = false;
                Recycle();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 6)
            {
                rb.velocity = Vector3.zero;
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezePosition;
            }
        }

        public void Create()
        {
            flyToPlayer = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            StartCoroutine(CanBeCollectedTimer(canBeCollectedTimer));
            aliveTimerRoutine = StartCoroutine(AliveTimer());
        }

        private void Recycle()
        {
            if (aliveTimerRoutine != null)
            {
                StopCoroutine(aliveTimerRoutine);
            }

            if (Expired != null)
            {
                Expired(this);
            }
        }

        private IEnumerator CanBeCollectedTimer(float time)
        {
            while (time > 0)
            {
                time -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            canBeCollected = true;
        }

        private IEnumerator AliveTimer()
        {
            aliveTimer = aliveTime;
            while (aliveTimer > 0)
            {
                aliveTimer -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            Recycle();
        }
    }
}