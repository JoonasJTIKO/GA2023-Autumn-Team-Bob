using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class FaceMainCamera : BaseUpdateListener
    {
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            transform.forward = (transform.position - Camera.main.transform.position).normalized;
        }
    }
}
