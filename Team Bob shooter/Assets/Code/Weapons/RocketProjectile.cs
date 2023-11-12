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

        [SerializeField]
        private LayerMask environmentMask;

        private Mover mover;

        private float damage;

        private ScreenShake screenShake;

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
                if (hit.collider.gameObject.layer == 7)
                {
                    if (hit.collider.gameObject.tag == "EnemyRagdoll") continue;
                    if (Physics.Raycast(transform.position, (hit.transform.position - transform.position).normalized, (hit.transform.position - transform.position).magnitude, environmentMask)) continue;

                    UnitHealth unitHealth = hit.collider.GetComponentInParent<UnitHealth>();
                    unitHealth.ExplosionPoint = impactPoint;
                    unitHealth.ExplosionStrength = 2f;
                    unitHealth.RemoveHealth(damage, EnemyGibbing.DeathType.Explode);
                }
                else if (hit.collider.gameObject.layer == 3)
                {
                    PlayerHit?.Invoke(impactPoint, playerKnockback);
                }
            }

            float distance = (impactPoint - screenShake.gameObject.transform.position).magnitude;
            float strength = Mathf.Clamp01((20 - distance) / 20);
            screenShake.Shake(0, strength);

            launched = false;
            Expired?.Invoke(this);
        }

        public void Launch(float damage, ScreenShake screenShake)
        {
            this.damage = damage;
            this.screenShake = screenShake;
            mover = GetComponent<Mover>();
            mover.Setup(flightSpeed);
            launched = true;
        }
    }
}
