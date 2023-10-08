using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Ragdoll : MonoBehaviour
    {
        private Rigidbody[] ragdollParts;

        private Animator animator;

        private void Awake()
        {
            ragdollParts = GetComponentsInChildren<Rigidbody>();
            animator = GetComponent<Animator>();
        }

        public void DisableRagdoll()
        {
            foreach (Rigidbody rigidbody in ragdollParts)
            {
                rigidbody.isKinematic = true;
            }
            animator.enabled = true;
        }

        public void EnableRagdoll()
        {
            foreach (Rigidbody rigidbody in ragdollParts)
            {
                rigidbody.isKinematic = false;
            }
            animator.enabled = false;
        }
    }
}
