using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Rotate : BaseUpdateListener
    {
        public float Speed = 30.0f;

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            Vector3 rotation = transform.localRotation.eulerAngles;
            rotation.y += Speed * deltaTime;
            transform.localEulerAngles = rotation;
        }
    }
}
