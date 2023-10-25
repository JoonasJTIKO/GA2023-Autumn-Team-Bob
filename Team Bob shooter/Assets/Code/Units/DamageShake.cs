using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

namespace TeamBobFPS
{
    public class DamageShake : MonoBehaviour
    {
        public new Camera camera;
        private Vector3 direction;

        public void OnTriggerEnter(Collider collider)
        {
            if(collider.gameObject.CompareTag("EnemyProjectile"))
            {
                Projectile projectile = collider.gameObject.GetComponent<Projectile>();

                direction = (this.transform.position - projectile.transform.position).normalized;

                StartCoroutine(CameraShake(0.5f));
            }
        }

        public IEnumerator CameraShake(float magnitude)
        {
            Vector3 originalPos = camera.transform.localEulerAngles;
            float elapsed = 0.0f;

            while (elapsed < 0.05f)
            {
                float x = Random.Range(-5f, 5f) * magnitude;
                float y = Random.Range(-5f, 5f) * magnitude;

                camera.transform.localEulerAngles = new Vector3(x, y, originalPos.z);

                elapsed += Time.deltaTime;

                yield return null;
            }
            camera.transform.localEulerAngles = originalPos;
        }
    }
}
