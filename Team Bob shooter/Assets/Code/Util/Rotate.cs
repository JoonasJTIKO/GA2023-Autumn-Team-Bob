using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Rotate : BaseUpdateListener
    {
        [SerializeField]
        private float speed = 1.0f;

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            Vector3 rotation = transform.localRotation.eulerAngles;
            rotation.y += speed * deltaTime;
            transform.localEulerAngles = rotation;
        }
    }
}
