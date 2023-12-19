using System;
using System.Collections;
using System.Collections.Generic;
using TeamBobFPS;
using UnityEngine;
using UnityEngine.UIElements;

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

        private float startDistance;

        private Bezier flightCurve;

        private float currentSpeed = 1f;

        private float currentDistance = 100f;

        private SineWaveHover waveHover;

        protected override void Awake()
        {
            base.Awake();
            mover = GetComponent<Mover>();
            playerPosition = FindObjectOfType<PlayerUnit>().gameObject.transform;
            playerHealth = playerPosition.gameObject.GetComponent<UnitHealth>();
            rb = GetComponent<Rigidbody>();
            flightCurve = GetComponent<Bezier>();
            waveHover = GetComponent<SineWaveHover>();
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            if (canBeCollected && !flyToPlayer && !playerHealth.HealthFull && (playerPosition.position - transform.position).magnitude <= pickUpRange)
            {
                rb.constraints = RigidbodyConstraints.None;
                mover.Setup(currentSpeed);
                flyToPlayer = true;
                Vector3 flatPos = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 flatPlayerPos = new Vector3(playerPosition.position.x, 0, playerPosition.position.z);
                startDistance = (flatPlayerPos - flatPos).magnitude;
                flightCurve.points = new Vector3[3];
                flightCurve.points[0] = transform.position;
                flightCurve.points[1] = transform.position + ((playerPosition.position - transform.position).normalized * (Vector3.Distance(playerPosition.position, transform.position) / 2));
                flightCurve.points[1] = new Vector3(flightCurve.points[1].x, transform.position.y + 2, flightCurve.points[1].z);
                waveHover.enabled = false;
            }

            if (flyToPlayer)
            {
                flightCurve.points[1] = transform.position + ((playerPosition.position - transform.position).normalized * (startDistance / 3));
                flightCurve.points[1] = new Vector3(flightCurve.points[1].x, transform.position.y + 2, flightCurve.points[1].z);
                flightCurve.points[2] = playerPosition.position;

                Vector3 direction;
                if ((new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(playerPosition.position.x, 0, playerPosition.position.z)).magnitude <= currentDistance)
                {
                    currentDistance = (new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(playerPosition.position.x, 0, playerPosition.position.z)).magnitude;
                    direction = flightCurve.GetDirection(1 - (currentDistance / startDistance));
                }
                else
                {
                    direction = (playerPosition.position - transform.position).normalized;
                }

                mover.Setup(currentSpeed);
                if (currentSpeed < speed)
                {
                    currentSpeed *= acceleration;
                }
                mover.Move(direction);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player" && flyToPlayer)
            {
                GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_COLLECT_HEALTH, transform.position, 0.5f, make2D: true);
                playerHealth.AddHealth(healAmount);
                flyToPlayer = false;
                Recycle();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 6 && !flyToPlayer)
            {
                rb.velocity = Vector3.zero;
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezePosition;
                waveHover.enabled = true;
                waveHover.Initialize();

                GetComponent<SphereCollider>().radius = 0.4f;
            }
        }

        public void Create()
        {
            rb.velocity = Vector3.zero;
            flyToPlayer = false;
            canBeCollected = false;
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

            waveHover.enabled = false;
            Recycle();
        }
    }
}
