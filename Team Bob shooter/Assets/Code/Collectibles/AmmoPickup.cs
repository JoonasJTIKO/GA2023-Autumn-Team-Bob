using System;
using System.Collections;
using System.Collections.Generic;
using TeamBobFPS;
using UnityEngine;

namespace TeamBobFPS
{
    public class AmmoPickup : BaseFixedUpdateListener
    {
        [SerializeField]
        private int ammoRecoveryAmount = 10;

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

        private WeaponSwap weaponSwap;

        public event Action<AmmoPickup> Expired;

        protected override void Awake()
        {
            base.Awake();
            mover = GetComponent<Mover>();
            playerPosition = FindObjectOfType<PlayerUnit>().gameObject.transform;
            weaponSwap = playerPosition.gameObject.GetComponent<WeaponSwap>();
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            if (canBeCollected && !flyToPlayer && !weaponSwap.CurrentReserveAmmoFull && (playerPosition.position - transform.position).magnitude <= pickUpRange)
            {
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
                // GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_COLLECT_HEALTH, transform.position, make2D: true);
                weaponSwap.AddAmmo(ammoRecoveryAmount);
                flyToPlayer = false;
                Recycle();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 6)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        public void Create()
        {
            flyToPlayer = false;
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
