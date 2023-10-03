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

        private bool ready = true;

        public bool Fire(Vector3 direction)
        {
            if (!ready) return false;

            GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
            if (bullet != null)
            {
                bullet.transform.position = projectilePos.position;
                bullet.transform.rotation = Quaternion.LookRotation(direction);
                bullet.SetActive(true);
                StartCoroutine(Cooldown());
                return true;
            }
            return false;
        }

        private IEnumerator Cooldown()
        {
            ready = false;
            float timer = 0;
            while (timer < timeBetweenShots)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            ready = true;
        }
    }
}
