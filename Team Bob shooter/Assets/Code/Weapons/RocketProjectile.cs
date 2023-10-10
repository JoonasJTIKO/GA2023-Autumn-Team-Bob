using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class RocketProjectile : BaseFixedUpdateListener
    {
        [SerializeField]
        private float flightSpeed = 1.0f;

        [SerializeField]
        private float playerKnockback = 1.0f;

        [SerializeField]
        private float explosionRadius = 4f;

        [SerializeField]
        private GameObject explosionPrefab;

        [SerializeField]
        private LayerMask mask;

        private Mover mover;

        private float damage;

        bool launched = false;

        public event Action<RocketProjectile> Expired;

        public static event Action<Vector3, float> PlayerHit;

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            if (launched)
            {
                mover.Move(transform.up);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!launched) return;

            Vector3 impactPoint = collision.GetContact(0).point;

            RaycastHit[] hits = Physics.SphereCastAll(impactPoint, explosionRadius,
                transform.forward, 0, mask);

            foreach (RaycastHit hit in hits )
            {
                if (hit.collider.gameObject.layer == 10)
                {
                    hit.collider.GetComponentInParent<UnitHealth>().RemoveHealth(damage);
                }
                else if (hit.collider.gameObject.layer == 3)
                {
                    PlayerHit?.Invoke(impactPoint, playerKnockback);
                }
            }

            launched = false;
            Expired?.Invoke(this);
        }

        public void Launch(float damage)
        {
            this.damage = damage;
            mover = GetComponent<Mover>();
            mover.Setup(flightSpeed);
            launched = true;
        }
    }
}
