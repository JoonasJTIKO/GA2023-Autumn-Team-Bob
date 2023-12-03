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
        private LayerMask playerLayer;

        private Transform parentEnemy;

        public bool Ready
        {
            get;
            private set;
        }

        private Coroutine cooldownRoutine = null;

        public void Awake()
        {
            Ready = true;
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
        }

        private IEnumerator Attack()
        {
            float timer = 0f;
            while (timer < chargeTime)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            RaycastHit hit;
            if (Physics.SphereCast(new Vector3(transform.position.x, transform.position.y + 20, transform.position.z), range, Vector3.down, out hit, 20f, playerLayer))
            {
                hit.collider.gameObject.GetComponent<UnitHealth>().RemoveHealth(damage);
            }

            cooldownRoutine = StartCoroutine(Cooldown());

            timer = 0f;
            while (timer < 0.3f)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            transform.parent = parentEnemy;
            gameObject.SetActive(false);
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
