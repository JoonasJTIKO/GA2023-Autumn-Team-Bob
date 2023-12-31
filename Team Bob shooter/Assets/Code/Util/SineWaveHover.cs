using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class SineWaveHover : BaseUpdateListener
    {
        [SerializeField]
        private float speed = 1.0f;

        [SerializeField]
        private float distance = 1.0f;

        [SerializeField]
        private float originYOffset = 1f;

        private Vector3 origin;

        private float angle;

        protected override void Awake()
        {
            base.Awake();

            Initialize();
        }

        public void Initialize()
        {
            origin = new Vector3(transform.position.x, transform.position.y + originYOffset, transform.position.z);
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            angle += speed * deltaTime;
            float sine = Mathf.Sin(angle) * distance;

            transform.position = origin + new Vector3(0f, sine, 0f);
        }
    }
}
