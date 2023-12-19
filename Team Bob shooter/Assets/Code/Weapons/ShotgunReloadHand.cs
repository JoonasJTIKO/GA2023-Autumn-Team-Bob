using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class ShotgunReloadHand : BaseUpdateListener
    {
        [SerializeField]
        private GameObject hand;

        [SerializeField]
        private Vector3 slidePos;

        [SerializeField]
        private Vector3 magPos;

        [SerializeField]
        private float duration = 0.2f;

        public void MoveToMag()
        {
            StartCoroutine(MoveToMagAnimation());
        }

        public void MoveToSlide()
        {
            StartCoroutine(MoveToSlideAnimation());
        }

        private IEnumerator MoveToMagAnimation()
        {
            Vector3 startPos = hand.transform.localPosition;

            float timer = 0f;
            while (timer <= duration)
            {
                hand.transform.localPosition = Vector3.Lerp(startPos, magPos, timer / duration);
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
        }

        private IEnumerator MoveToSlideAnimation()
        {
            Vector3 startPos = hand.transform.localPosition;

            float timer = 0f;
            while (timer <= duration)
            {
                hand.transform.localPosition = Vector3.Lerp(startPos, slidePos, timer / duration);
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
        }
    }
}
