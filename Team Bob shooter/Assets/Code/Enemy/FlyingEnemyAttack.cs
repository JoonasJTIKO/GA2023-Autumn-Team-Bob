using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class FlyingEnemyAttack : MonoBehaviour
    {
        [SerializeField]
        private SkyBlastAttack attackObject;

        [SerializeField]
        private float timeBetweenShots = 1f;

        [SerializeField]
        private LayerMask environmentLayers;

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

        public bool Fire(Transform playerTransform)
        {
            if (!Ready) return false;

            Vector3 target = FindTargetPosition(playerTransform);
            if (target == Vector3.zero) return false;

            attackObject.transform.position = target;
            attackObject.gameObject.SetActive(true);
            attackObject.Execute();
            cooldownRoutine = StartCoroutine(Cooldown());
            
            return true;
        }

        private Vector3 FindTargetPosition(Transform playerTransform)
        {
            Vector3 targetPosition = Vector3.zero;

            RaycastHit hit;
            if (Physics.Raycast(playerTransform.position, Vector3.down, out hit, 10f, environmentLayers))
            {
                targetPosition = hit.point;
            }

            return targetPosition;
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
