using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class RagdollBehavior : MonoBehaviour
    {
        private Rigidbody[] ragdollRigidbodies;

        private CharacterJoint[] ragdollCharacterJoints;

        private Quaternion[] jointRotations;

        private Vector3 basePosition, baseLocalPosition;

        private Quaternion baseRotation, baseLocalRotation;

        private void Awake()
        {
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            ragdollCharacterJoints = GetComponentsInChildren<CharacterJoint>();
            jointRotations = new Quaternion[ragdollCharacterJoints.Length];
            int index = 0;
            foreach (var joint in ragdollCharacterJoints)
            {
                jointRotations[index] = joint.transform.localRotation;
                index++;
            }
            basePosition = transform.position;
            baseLocalPosition = transform.localPosition;
            baseRotation = transform.rotation;
            baseLocalRotation = transform.localRotation;

            DisableRagdoll();
        }

        public void PushRagdoll(Vector3 force)
        {
            foreach (var rb in ragdollRigidbodies)
            {
                rb.AddForce(force, ForceMode.Impulse);
            }
        }

        public void DisableRagdoll()
        {
            int index = 0;
            foreach (var joint in ragdollCharacterJoints)
            {
                joint.transform.localRotation = jointRotations[index];
                index++;
            }

            transform.position = basePosition;
            transform.localPosition = baseLocalPosition;
            transform.rotation = baseRotation;
            transform.localRotation = baseLocalRotation;

            foreach (var rb in ragdollRigidbodies)
            {
                rb.velocity = Vector3.zero; 
                rb.angularVelocity = Vector3.zero;
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
