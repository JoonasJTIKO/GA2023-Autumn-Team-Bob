using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace TeamBobFPS
{
    public class EnemyFireAtPlayer : BaseFixedUpdateListener
    {
        [SerializeField]
        private Transform projectilePos;

        [SerializeField]
        private float timeBetweenShots = 1f;

        public bool Ready
        {
            get;
            private set;
        }

        private Coroutine cooldownRoutine = null;

        protected override void Awake()
        {
            base.Awake();

            Ready = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (cooldownRoutine != null)
            {
                StopCoroutine(cooldownRoutine);
            }
            Ready = true;
        }

        public bool Fire(Vector3 direction)
        {
            if (!Ready) return false;

            GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
            Debug.Log("null " + bullet == null);
            if (bullet != null)
            {
                bullet.transform.position = projectilePos.position;
                bullet.transform.rotation = Quaternion.LookRotation(direction);
                bullet.SetActive(true);
                cooldownRoutine = StartCoroutine(Cooldown());
                return true;
            }
            return false;
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
