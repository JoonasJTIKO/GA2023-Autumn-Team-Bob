using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TeamBobFPS
{
    public class DamageIndicator : BaseFixedUpdateListener
    {
        private const float maxTimer = 8.0f;
        private float timer = maxTimer;

        private CanvasGroup canvasGroup = null;
        protected CanvasGroup CanvasGroup
        {
            get
            {
                if(canvasGroup == null)
                {
                    canvasGroup = GetComponent<CanvasGroup>();
                    if(canvasGroup == null)
                    {
                        canvasGroup = gameObject.AddComponent<CanvasGroup>();
                    }
                }
                return canvasGroup;
            }
        }

        private RectTransform rect = null;
        protected RectTransform Rect
        {
            get
            {
                if (rect == null)
                {
                    rect = GetComponent<RectTransform>();
                    if (rect == null)
                    {
                        rect = gameObject.AddComponent<RectTransform>();
                    }
                }
                return rect;
            }
        }

        public Transform Target { get; protected set; } = null;
        private Transform player = null;

        private IEnumerator IE_Countdown = null;
        private Action unRegister = null;

        private Quaternion tRot = Quaternion.identity;
        private Vector3 tPos = Vector3.zero;

        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Register the data of the enemy position.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="player"></param>
        /// <param name="unRegister"></param>
        public void Register(Transform target, Transform player, Action unRegister)
        {
            this.Target = target;
            this.player = player;
            this.unRegister = unRegister;
            StartCoroutine(RotateToTarget());
            StartTimer();
        }

        public void Restart()
        {
            timer = maxTimer;
            StartTimer();
        }

        private void StartTimer()
        {
           if(IE_Countdown != null) { StopCoroutine(IE_Countdown); }
            IE_Countdown = Countdown();
            StartCoroutine(IE_Countdown);
        }

        private IEnumerator Countdown()
        {
            while(CanvasGroup.alpha < 1.0f)
            {
                CanvasGroup.alpha += 4 * Time.deltaTime;
                yield return null;
            }
            while(timer > 0)
            {
                timer--;
                yield return new WaitForSeconds(1);
            }
            while(CanvasGroup.alpha > 0.0f)
            {
                CanvasGroup.alpha -= 2 * Time.deltaTime;
                yield return null;
            }
            unRegister();
            Destroy(gameObject);
        }

        IEnumerator RotateToTarget()
        {
            while (enabled) 
            {
                if (Target)
                {
                    tPos = Target.position;
                    tRot = Target.rotation;
                }
                Vector3 direction = player.position - tPos;

                tRot = Quaternion.LookRotation(direction);
                tRot.z = -tRot.y;
                tRot.x = 0;
                tRot.y = 0;

                Vector3 northDirection = new Vector3(0, 0, player.eulerAngles.y);
                Rect.localRotation = tRot * Quaternion.Euler(northDirection);

                yield return null;
            }
        }


    }
}
