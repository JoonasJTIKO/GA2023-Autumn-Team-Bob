using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class SkyBlastAttack : MonoBehaviour
    {
        [SerializeField]
        private float damage;

        [SerializeField]
        private float range;

        [SerializeField]
        private float chargeTime;

        [SerializeField]
        private float timeBetweenShots;

        [SerializeField]
        private float disableTime;

        [SerializeField]
        private LayerMask playerLayer;

        private Transform parentEnemy;

        public event Action<SkyBlastAttack> AttackComplete;

        private UnitHealth playerHealthComponent;

        public bool Ready
        {
            get;
            private set;
        }

        private Coroutine cooldownRoutine = null;

        private Coroutine attackActiveRoutine = null;

        private AudioSource attackAudioSource;

        public void Awake()
        {
            Ready = true;
            playerHealthComponent = FindObjectOfType<PlayerUnit>().GetComponent<UnitHealth>();
        }

        public void OnDisable()
        {
            if (cooldownRoutine != null)
            {
                StopCoroutine(cooldownRoutine);
            }
            Ready = true;
        }

        public void Execute()
        {
            parentEnemy = transform.parent;
            transform.parent = null;

            StartCoroutine(Attack());
            StartCoroutine(Disable());
        }

        private IEnumerator Attack()
        {
            float timer = 0f;
            while (timer < chargeTime)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            attackActiveRoutine = StartCoroutine(AttackActive());

            cooldownRoutine = StartCoroutine(Cooldown());
            
        }

        private IEnumerator AttackActive()
        {
            attackAudioSource = GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_DRAGON_BEAM, transform.position, 0.4f, true);

            while (true)
            {
                RaycastHit hit;
                if (Physics.SphereCast(new Vector3(transform.position.x, transform.position.y + 20, transform.position.z), range, Vector3.down, out hit, 20f, playerLayer))
                {
                    playerHealthComponent.RemoveHealth(damage);
                }
                yield return null;
            }
        }

        private IEnumerator Disable()
        {
            float timer = 0f;

            while (timer < disableTime)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            StopCoroutine(attackActiveRoutine);
            GameInstance.Instance.GetAudioManager().StopLoopingAudio(attackAudioSource);
            transform.parent = parentEnemy;
            gameObject.SetActive(false);
            AttackComplete?.Invoke(this);
        }

        private IEnumerator Cooldown()
        {
            Ready = false;
            float timer = 0;
            while (timer < timeBetweenShots)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            Ready = true;
        }
    }
}
