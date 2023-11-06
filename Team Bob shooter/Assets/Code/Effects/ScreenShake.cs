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
        private AnimationCurve[] animationCurves;

        private Coroutine shakeRoutine = null;

        public void Shake(int curveIndex, float strength = 1)
        {
            if (shakeRoutine != null)
            {
                StopCoroutine(shakeRoutine);
            }
            shakeRoutine = StartCoroutine(Shaking(curveIndex, strength));
        }

        private IEnumerator Shaking(int curveIndex, float strength)
        {
            Vector3 startPos = transform.localPosition;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                float shakeStrength = animationCurves[curveIndex].Evaluate(timer / duration) * strength;
                transform.localPosition = startPos + Random.insideUnitSphere * shakeStrength;
                yield return null;
            }

            transform.localPosition = startPos;
            shakeRoutine = null;
        }
    }
}
