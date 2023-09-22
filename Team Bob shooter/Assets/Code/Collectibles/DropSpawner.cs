using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class DropSpawner : MonoBehaviour
    {
        [SerializeField]
        private HealthOrb healthOrb;

        [SerializeField]
        private AmmoPickup ammoPickup;

        [SerializeField]
        private int minimumOrbs = 1;

        [SerializeField]
        private int maximumOrbs = 3;

        private ComponentPool<HealthOrb> healthPool;
        private ComponentPool<AmmoPickup> ammoPool;

        public void Awake()
        {
            healthPool = new ComponentPool<HealthOrb>(healthOrb, 10);
            ammoPool = new ComponentPool<AmmoPickup>(ammoPickup, 10);
        }

        public void SpawnThings()
        {
            int hpOrbAmount = Random.Range(minimumOrbs, maximumOrbs + 1);
            for (int i = 0; i < hpOrbAmount; i++)
            {
                HealthOrb spawned = healthPool.Get();
                spawned.Expired += OnExpired;
                spawned.transform.position = this.transform.position;
                spawned.Create();

                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0.8f, Random.Range(-1f, 1f));
                randomDirection.Normalize();
                spawned.GetComponent<Rigidbody>().AddForce(randomDirection * 5, ForceMode.Impulse);
            }

            int ammoPickupAmount = Random.Range(minimumOrbs, maximumOrbs + 1);
            for (int i = 0; i < ammoPickupAmount; i++)
            {
                AmmoPickup spawned = ammoPool.Get();
                spawned.Expired += OnExpired;
                spawned.transform.position = this.transform.position;
                spawned.Create();

                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0.8f, Random.Range(-1f, 1f));
                randomDirection.Normalize();
                spawned.GetComponent<Rigidbody>().AddForce(randomDirection * 5, ForceMode.Impulse);
            }
        }

        private void OnExpired(HealthOrb healthOrb)
        {
            healthOrb.Expired -= OnExpired;

            if (!healthPool.Return(healthOrb))
            {
                Debug.LogError("Can't return healthOrb to pool");
            }
        }

        private void OnExpired(AmmoPickup ammoPickup)
        {
            ammoPickup.Expired -= OnExpired;

            if (!ammoPool.Return(ammoPickup))
            {
                Debug.LogError("Can't return ammoPickup to pool");
            }
        }

    }
}
