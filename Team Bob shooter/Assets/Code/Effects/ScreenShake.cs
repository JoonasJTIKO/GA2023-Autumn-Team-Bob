using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class ScreenShake : MonoBehaviour
    {
        [SerializeField]
        private float duration = 0.5f;

        [SerializeField]
        private AnimationCurve animationCurve;

        public void Shake()
        {
            StartCoroutine(Shaking());
        }

        private IEnumerator Shaking()
        {
            Vector3 startPos = transform.localPosition;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                float strength = animationCurve.Evaluate(timer / duration);
                transform.localPosition = startPos + Random.insideUnitSphere * strength;
                yield return null;
            }

            transform.localPosition = startPos;
        }
    }
}
