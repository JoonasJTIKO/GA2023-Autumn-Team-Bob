using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class RagdollBehavior : MonoBehaviour
    {
        private Rigidbody[] ragdollRigidbodies;

        private void Awake()
        {
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            DisableRagdoll();
        }

        public void DisableRagdoll()
        {
            foreach (var rb in ragdollRigidbodies)
            {
                rb.isKinematic = true;
            }
        }

        public void EnableRagdoll()
        {
            foreach (var rb in ragdollRigidbodies)
            {
                rb.isKinematic = false;
            }
        }
    }
}
