using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Weapon_Minigun : WeaponBase
    {
        [SerializeField]
        private float knockbackStrength = 2f;

        [SerializeField]
        private float spreadAngle = 0.5f;

        [SerializeField]
        private LayerMask environmentLayers;

        [SerializeField]
        private LayerMask enemyLayers;

        private PlayerUnit playerUnit;

        private Rigidbody rb;

        private Transform[] activeHitEffects = new Transform[8];

        private int index = 0;

        public override int CurrentReserveAmmo
        {
            get { return currentMagAmmoCount; }
        }

        public override int MaxReserveAmmo
        {
            get { return magSize; }
        }

        protected override void Awake()
        {
            base.Awake();

            playerUnit = GetComponent<PlayerUnit>();
            rb = GetComponent<Rigidbody>();
        }

        public override void AbortReload()
        {
        }

        public override void BeginReload()
        {

        }

        protected override void Fire()
        {
            RaycastHit hit;
            Vector3 angle = playerUnit.PlayerCam.transform.TransformDirection(Vector3.forward);
            angle = new(angle.x + Random.Range(-spreadAngle, spreadAngle),
                angle.y + Random.Range(-spreadAngle, spreadAngle),
                angle.z + Random.Range(-spreadAngle, spreadAngle));

            BulletTracer bullet = bulletTrailPool.Get();
            bullet.Expired += RecycleTracer;
            bullet.transform.position = bulletOrigin.transform.position;
            bullet.Launch(angle);

            if (Physics.Raycast(playerUnit.PlayerCam.transform.position,
                angle, out hit, Mathf.Infinity, enemyLayers))
            {
                float damage = bulletDamage;
                if (hit.collider.gameObject.layer == 11)
                {
                    damage *= 1.5f;
                }

                hit.collider.gameObject.GetComponentInParent<UnitHealth>().RemoveHealth(damage);

                if (activeHitEffects[index] != null)
                {
                    hitEffectPool.Return(activeHitEffects[index]);
                }
                activeHitEffects[index] = hitEffectPool.Get();
                activeHitEffects[index].position = hit.point;
                index++;
                if (index >= activeHitEffects.Length) index = 0;
            }
            else if (Physics.Raycast(playerUnit.PlayerCam.transform.position,
                angle, out hit, Mathf.Infinity, environmentLayers))
            {
                if (activeHitEffects[index] != null)
                {
                    hitEffectPool.Return(activeHitEffects[index]);
                }
                activeHitEffects[index] = hitEffectPool.Get();
                activeHitEffects[index].position = hit.point;
                index++;
                if (index >= activeHitEffects.Length) index = 0;
            }

            if (!playerUnit.IsGrounded)
            {
                if (rb.velocity.y < 0 && playerUnit.PlayerCam.transform.TransformDirection(transform.forward).y < 0) rb.velocity = new(rb.velocity.x, 0, rb.velocity.z);

                rb.AddForce(-playerUnit.PlayerCam.transform.TransformDirection(Vector3.forward) * knockbackStrength, ForceMode.Impulse);
            }
        }

        public override void AddAmmo(int amount)
        {
            currentMagAmmoCount += amount;
            if (currentMagAmmoCount > magSize)
            {
                currentMagAmmoCount = magSize;
            }
        }

        public override void FireButtonHeld(bool state)
        {
            viewmodelAnimator.SetBool("Firing", state);
        }
    }
}
