using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamBobFPS
{
    public class Mover : BaseFixedUpdateListener
    {
        [SerializeField]
        private LayerMask layerMask;

        private new CapsuleCollider collider;
        private Rigidbody rb;
        private Vector3 direction;

        private RaycastHit slopeCast;
        private RaycastHit hit;

        public bool IsGrounded
        {
            get;
            private set;
        }

        public float Speed
        {
            get;
            private set;
        }

        public void Setup(float speed)
        {
            collider = GetComponent<CapsuleCollider>();

            Speed = speed;
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
            if (rb == null)
            {
                Debug.LogError("Rigidbody can't be found!");
            }
        }

        public void Move(Vector3 direction)
        {
            this.direction = direction;
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            IsGrounded = Physics.SphereCast(transform.position, collider.radius, 
                -transform.up, out hit, 0.6f, layerMask);

            Move(fixedDeltaTime);
        }

        private void Move(float deltaTime)
        {
            Vector3 move = direction * Speed * deltaTime;
            Vector3 position = transform.position + move;

            rb.MovePosition(position);

            direction = Vector3.zero;
        }

        public bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeCast, 2f * 0.5f + 0.3f, layerMask) && IsGrounded)
            {
                float angle = Vector3.Angle(Vector3.up, slopeCast.normal);
                return angle < 45f && angle != 0;
            }
            return false;
        }

        public Vector3 GetSlopeDirection(Vector3 moveDirection)
        {
            return Vector3.ProjectOnPlane(moveDirection, slopeCast.normal).normalized;
        }
    }
}
