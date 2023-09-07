using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class TimedParticle : MonoBehaviour
    {

        [SerializeField]
        private PooledParticle runParticle;

        [SerializeField]
        private PooledParticle jumpParticle;

        [SerializeField]
        private Transform particlePosition;

        private ComponentPool<PooledParticle> runParticlePool;

        private ComponentPool<PooledParticle> jumpParticlePool;

        [SerializeField]
        private Sensor groundSensor;

        private void Awake()
        {
            runParticlePool = new ComponentPool<PooledParticle>(runParticle, 10);
            jumpParticlePool = new ComponentPool<PooledParticle>(jumpParticle, 5);
        }

        private void RunOnExpired(PooledParticle particle)
        {
            particle.Expired -= RunOnExpired;

            if (!runParticlePool.Return(particle))
            {
                Debug.LogError("Can't return particle to pool");
            }
        }

        private void JumpOnExpired(PooledParticle particle)
        {
            particle.Expired -= JumpOnExpired;

            if (!jumpParticlePool.Return(particle))
            {
                Debug.LogError("Can't return particle to pool");
            }
        }

        public void RunParticle()
        {
            if (groundSensor.Active)
            {
                PooledParticle spawned = runParticlePool.Get();
                spawned.Expired += RunOnExpired;
                spawned.transform.position = particlePosition.position;
                spawned.transform.rotation = particlePosition.rotation;
            }
        }

        public void JumpParticle()
        {
            PooledParticle spawned = jumpParticlePool.Get();
            spawned.Expired += JumpOnExpired;
            spawned.transform.position = particlePosition.position;
            spawned.transform.rotation = particlePosition.rotation;
        }
    }
}
