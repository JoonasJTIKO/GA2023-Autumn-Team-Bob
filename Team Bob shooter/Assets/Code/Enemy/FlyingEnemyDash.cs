using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class FlyingEnemyDash : MonoBehaviour
    {
        [SerializeField]
        private float maxSpeed = 10f;

        [SerializeField]
        private float dashDuration = 1f;

        [SerializeField]
        private AnimationCurve speedCurve;

        private Mover mover;

        private Coroutine dashRoutine;

        public event Action OnDashCompleted;

        private void Awake()
        {
            mover = GetComponent<Mover>();
        }

        public void Dash(Vector3 direction)
        {
            dashRoutine = StartCoroutine(DashRoutine(direction));
        }

        private IEnumerator DashRoutine(Vector3 direction)
        {
            float timer = 0f;

            while (timer < dashDuration)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, direction, Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale * 10, 0));

                mover.Setup(maxSpeed * speedCurve.Evaluate(timer / dashDuration));
                mover.Move(direction);

                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            OnDashCompleted?.Invoke();
        }
    }
}
