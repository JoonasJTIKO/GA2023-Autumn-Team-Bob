using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class EquipModel : MonoBehaviour
    {
        private Rotate rotate;

        private SineWaveHover sineHover;

        private Coroutine routine;

        private float baseRotateSpeed;

        private Vector3 basePos;

        private void Awake()
        {
            rotate = GetComponent<Rotate>();
            sineHover = GetComponent<SineWaveHover>();

            baseRotateSpeed = rotate.Speed;
            basePos = transform.position;
        }

        private void OnEnable()
        {
            if (GameInstance.Instance == null) return;

            routine = StartCoroutine(EnableAnimation());
        }

        private void OnDisable()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
            }
        }

        private IEnumerator EnableAnimation()
        {
            sineHover.enabled = false;
            float speed = 30 * baseRotateSpeed;
            float timer = 1f;

            while (timer >= 0)
            {
                rotate.Speed = speed;
                speed = Mathf.Lerp(30 * baseRotateSpeed, baseRotateSpeed, 1 - timer);

                transform.position = Vector3.Lerp(new Vector3(basePos.x, basePos.y - 0.5f, basePos.z), basePos, 1 - timer);
                timer -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            rotate.Speed = baseRotateSpeed;
            transform.position = basePos;
            sineHover.enabled = true;
        }
    }
}
