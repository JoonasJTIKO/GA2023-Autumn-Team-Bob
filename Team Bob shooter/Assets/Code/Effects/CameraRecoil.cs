using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class CameraRecoil : MonoBehaviour
    {
        [SerializeField]
        private float initialRecoilTime;

        [SerializeField]
        private float recoveryTime;

        private Coroutine recoilRoutine;

        public void DoRecoil(float amount)
        {
            if (amount > 0)
            {
                amount = -amount;
            }
            if (recoilRoutine != null)
            {
                StopCoroutine(recoilRoutine);
                transform.localRotation = Quaternion.Euler(new(0, transform.localRotation.y, transform.localRotation.z));
            }
            recoilRoutine = StartCoroutine(Recoil(amount));
        }

        private IEnumerator Recoil(float amount)
        {
            float timer = 0f;

            while (timer < initialRecoilTime)
            {
                float rot = Mathf.Lerp(0, amount, timer / initialRecoilTime);
                transform.localRotation = Quaternion.Euler(new(rot, transform.localRotation.y, transform.localRotation.z));
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            timer = 0f;

            while (timer < recoveryTime)
            {
                float rot = Mathf.Lerp(amount, 0, timer / recoveryTime);
                transform.localRotation = Quaternion.Euler(new(rot, transform.localRotation.y, transform.localRotation.z));
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
        }
    }
}
