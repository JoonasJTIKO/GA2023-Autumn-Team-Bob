using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class RagdollBehavior : MonoBehaviour
    {
        private Rigidbody[] ragdollRigidbodies;

        private CharacterJoint[] ragdollCharacterJoints;

        private Vector3[] ragdollPartDefaultPositions;

        private Quaternion[] jointRotations;

        private Quaternion[] partRotations;

        private Quaternion defaultRotation;

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
            
            ragdollPartDefaultPositions = new Vector3[ragdollRigidbodies.Length];
            partRotations = new Quaternion[ragdollRigidbodies.Length];
            index = 0;
            foreach (var part in ragdollRigidbodies)
            {
                ragdollPartDefaultPositions[index] = part.transform.localPosition;
                partRotations[index] = part.transform.localRotation;
                index++;
            }

            defaultRotation = transform.rotation;

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

            index = 0;
            foreach (var rb in ragdollRigidbodies)
            {
                rb.transform.localPosition = ragdollPartDefaultPositions[index];
                rb.transform.localRotation = partRotations[index];
                index++;

                rb.velocity = Vector3.zero; 
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            transform.rotation = defaultRotation;
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
