using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class BulletTracer : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        [SerializeField]
        private float aliveDuration;

        private Mover mover;

        private Vector3 moveDirection;

        private TrailRenderer trailRenderer;

        public event Action<BulletTracer> Expired;

        private void Awake()
        {
            trailRenderer = GetComponent<TrailRenderer>();
            mover = GetComponent<Mover>();
            mover.Setup(speed);
        }

        public void Launch(Vector3 direction)
        {
            moveDirection = direction;
            StartCoroutine(Go());
        }

        private IEnumerator Go()
        {
            trailRenderer.enabled = true;
            trailRenderer.Clear();
            float timer = aliveDuration;
            while (timer > 0)
            {
                mover.Move(moveDirection);
                timer -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            moveDirection = Vector3.zero;
            trailRenderer.enabled = false;
            Expired?.Invoke(this);
        }
    }
}
