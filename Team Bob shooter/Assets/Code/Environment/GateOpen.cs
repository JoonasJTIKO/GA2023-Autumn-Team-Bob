using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace TeamBobFPS
{
    public class GateOpen : WaveClearAction
    {
        [SerializeField]
        private float targetRotateAmount = 90f;

        [SerializeField]
        private float speed = 1.0f;

        private void Awake()
        {
            if (targetRotateAmount < 0)
            {
                speed = -speed;
            }
        }

        protected override bool DoAction(int waveIndex)
        {
            if (!base.DoAction(waveIndex)) return false;

            StartCoroutine(Open());
            return true;
        }

        private IEnumerator Open()
        {
            float amountRotated = 0f;

            while (amountRotated < Mathf.Abs(targetRotateAmount))
            {
                transform.Rotate(new Vector3(0, speed, 0) * Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale);
                amountRotated += Mathf.Abs(speed * Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale);
                yield return null;
            }
        }
    }
}
