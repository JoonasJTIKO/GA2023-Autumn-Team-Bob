using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class MainMenuCamera : MonoBehaviour
    {
        [SerializeField]
        private float travelTime;

        [SerializeField]
        private Vector3 targetPos;

        [SerializeField]
        private Transform lookAt;

        private Vector3 startPos;

        private bool goBack = false;

        private float timer = 0f;

        private void Awake()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            transform.forward = (lookAt.position - transform.position).normalized;

            if (!goBack)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, timer / travelTime);
                timer += Time.deltaTime;
                if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                {
                    timer = 0f;
                    goBack = true;
                }
            }
            else if (goBack)
            {
                transform.position = Vector3.Lerp(targetPos, startPos, timer / travelTime);
                timer += Time.deltaTime;
                if (Vector3.Distance(transform.position, startPos) < 0.1f)
                {
                    timer = 0f;
                    goBack = false;
                }
            }
        }
    }
}
