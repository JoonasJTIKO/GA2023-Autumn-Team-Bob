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

        public void Shake(int curveIndex)
        {
            if (shakeRoutine != null)
            {
                StopCoroutine(shakeRoutine);
            }
            shakeRoutine = StartCoroutine(Shaking(curveIndex));
        }

        private IEnumerator Shaking(int curveIndex)
        {
            Vector3 startPos = transform.localPosition;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                float strength = animationCurves[curveIndex].Evaluate(timer / duration);
                transform.localPosition = startPos + Random.insideUnitSphere * strength;
                yield return null;
            }

            transform.localPosition = startPos;
            shakeRoutine = null;
        }
    }
}
