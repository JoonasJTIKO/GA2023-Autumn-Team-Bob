using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class FlyingEnemyOrbit : BaseUpdateListener
    {
        [SerializeField]
        private float orbitRadius = 5f;

        [SerializeField]
        private float speed;

        private Vector3 orbitPoint;

        private Mover mover;

        private bool orbitRadiusReached = false;

        protected override void Awake()
        {
            base.Awake();

            mover = GetComponent<Mover>();
        }

        public void Setup()
        {
            orbitPoint = transform.position;
            mover.Setup(speed);
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (!orbitRadiusReached)
            {
                mover.Move(transform.forward);
                if (Vector3.Distance(transform.position, orbitPoint) >= orbitRadius)
                {
                    orbitRadiusReached = true;
                }
            }
            else
            {
                Vector3 direction = (transform.position - new Vector3(orbitPoint.x, transform.position.y, orbitPoint.z)).normalized;
                direction = Quaternion.Euler(0, 90, 0) * direction;

                mover.Move(direction);

                transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, direction, Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale * 10, 0));
            }

        }
    }
}
