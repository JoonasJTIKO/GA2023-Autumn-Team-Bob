using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class PooledParticle : MonoBehaviour
    {
        public event Action<PooledParticle> Expired;

        public void OnParticleSystemStopped()
        {
            if (Expired != null)
            {
                Expired(this);
            }
        }
    }
}
