using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamBobFPS
{
    public class Mover : BaseFixedUpdateListener
    {
        [SerializeField]
        private LayerMask groundLayer;

        [SerializeField]
        private LayerMask environmentLayer;

        [SerializeField]
        private bool doGroundCheck = true;

        private new CapsuleCollider collider;
        private Rigidbody rb;
        private Vector3 direction, previousDirection;

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

        private float currentSpeed = 0f;

        private float accelerationTime, decelerationTime;

        private bool noAcceleration = true;

        private bool setupComplete = false;

        public void Setup(float speed, float accelerationTime = 0f, float decelerationTime = 0f, bool noAcceleration = true)
        {
            collider = GetComponent<CapsuleCollider>();

            Speed = speed;
            this.accelerationTime = accelerationTime;
            this.decelerationTime = decelerationTime;
            this.noAcceleration = noAcceleration;
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
            if (rb == null)
            {
                Debug.LogError("Rigidbody can't be found!");
            }

            setupComplete = true;
        }

        public void Move(Vector3 direction)
        {
            this.direction = direction;
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            if (doGroundCheck)
            {
                IsGrounded = Physics.SphereCast(transform.position, collider.radius * 0.7f,
                                -transform.up, out hit, 0.7f, groundLayer);
            }

            if (rb != null && setupComplete)
            {
                Move(fixedDeltaTime);
            }
        }

        private void Move(float deltaTime)
        {
            if (noAcceleration)
            {
                currentSpeed = Speed;
            }
            else if (direction != Vector3.zero && currentSpeed < Speed && accelerationTime > 0)
            {
                currentSpeed += deltaTime * (Speed * (1 / accelerationTime));
            }
            else if (direction == Vector3.zero && currentSpeed > 0 && decelerationTime > 0)
            {
                direction = previousDirection;
                currentSpeed -= deltaTime * (Speed * (1 / decelerationTime));
            }
            if (currentSpeed > Speed) currentSpeed = Speed;
            if (currentSpeed < 0) currentSpeed = 0;

            Vector3 velocity = direction * currentSpeed * deltaTime * 70;
            if (direction.y != 0)
            {
                rb.velocity = velocity;
            }
            else
            {
                rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
            }

            previousDirection = direction;
            direction = Vector3.zero;
        }

        public bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeCast, 2f * 0.5f + 0.3f, groundLayer) && IsGrounded)
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
