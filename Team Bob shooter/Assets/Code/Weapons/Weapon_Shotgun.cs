using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Weapon_Shotgun : WeaponBase
    {
        [SerializeField]
        private float spreadAngle = 0.5f;

        [SerializeField]
        private LayerMask layerMask;

        [SerializeField]
        private int pelletCount = 5;

        [SerializeField]
        private float reloadTime = 0.5f;

        private PlayerUnit playerUnit;

        private Rigidbody rb;

        private Transform[] activeHitEffects = new Transform[8];

        private int index = 0;

        protected override void Awake()
        {
            base.Awake();

            playerUnit = GetComponent<PlayerUnit>();
            rb = GetComponent<Rigidbody>();
        }

        public override void BeginReload()
        {
            if (currentMagAmmoCount == magSize) return;

            StartCoroutine(ReloadAfterDelay());
        }

        private IEnumerator ReloadAfterDelay()
        {
            yield return new WaitForSeconds(reloadTime);
            ReloadCompleted();
        }

        protected override void Fire()
        {
            for (int i = 0; i < pelletCount; i++)
            {
                RaycastHit hit;
                Vector3 angle = playerUnit.PlayerCam.transform.TransformDirection(Vector3.forward);
                angle = new(angle.x + Random.Range(-spreadAngle, spreadAngle),
                    angle.y + Random.Range(-spreadAngle, spreadAngle),
                    angle.z + Random.Range(-spreadAngle, spreadAngle));

                if (Physics.Raycast(playerUnit.PlayerCam.transform.position,
                    angle, out hit, Mathf.Infinity, layerMask))
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
            }
        }
    }
}
