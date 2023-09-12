using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TeamBobFPS
{
    public class PlayerUnit : UnitBase
    {
        [SerializeField]
        private float speed = 5;

        [SerializeField]
        private float jumpStrength = 1;

        [SerializeField]
        private float fallSpeedModifier = 10f;

        private PlayerInputs playerInputs;

        public PlayerInputs Inputs
        {
            get { return playerInputs; }
        }

        private InputAction moveAction, jumpAction;

        private Mover mover;

        private Rigidbody rb;

        private UnitHealth unitHealth;

        private Camera playerCam;

        public Camera PlayerCam
        {
            get { return playerCam; }
        }

        private bool jumping = false;

        public Vector3 MoveDirection
        {
            get;
            private set;
        }

        public int CurrentWeaponSlot;

        public bool IsGrounded
        {
            get { return mover.IsGrounded; }
        }

        public bool LockMovement = false;

        public static event Action OnPlayerHealthChanged;

        protected override void Awake()
        {
            base.Awake();
            if (GameInstance.Instance == null) return;

            mover = GetComponent<Mover>();
            mover.Setup(speed);
            rb = GetComponent<Rigidbody>();
            unitHealth = GetComponent<UnitHealth>();
            playerCam = GetComponentInChildren<Camera>();
            playerInputs = new PlayerInputs();
            moveAction = playerInputs.Movement.Move;
            jumpAction = playerInputs.Movement.Jump;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (GameInstance.Instance == null) return;

            playerInputs.Movement.Enable();
            jumpAction.performed += Jump;

            RocketProjectile.PlayerHit += ReceiveKnockback;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (GameInstance.Instance == null) return;

            playerInputs.Movement.Disable();
            jumpAction.performed -= Jump;

            RocketProjectile.PlayerHit -= ReceiveKnockback;
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            if (IsGrounded)
            {
                rb.velocity = Vector3.zero;
            }

            if (!LockMovement) rb.useGravity = !mover.OnSlope();

            if (!mover.OnSlope() && !IsGrounded && rb.useGravity)
            {
                rb.AddForce(Physics.gravity * rb.mass * fallSpeedModifier, ForceMode.Force);
            }

            Vector3 camForward = new(playerCam.transform.forward.x, 0, playerCam.transform.forward.z);
            Vector3 camRight = new(playerCam.transform.right.x, 0, playerCam.transform.right.z);
            camForward.Normalize();
            camRight.Normalize();

            Vector3 move = moveAction.ReadValue<Vector3>();
            move.Normalize();

            if (move != Vector3.zero)
            {
                move = move.x * camRight + move.z * camForward;
            }

            move = mover.GetSlopeDirection(move);

            MoveDirection = move;

            if (!LockMovement)
            {
                mover.Move(move);
            }

            if (IsGrounded && jumping) jumping = false;
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (jumping || !IsGrounded) return;

            rb.useGravity = true;
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
        }

        private void ReceiveKnockback(Vector3 origin, float strength, float radius)
        {
            rb.AddExplosionForce(strength * 100, origin, radius);
        }

        public Vector3 GetForwardDirection()
        {
            Vector3 forward = new(playerCam.transform.forward.x, 0, playerCam.transform.forward.z);
            forward.Normalize();
            forward = mover.GetSlopeDirection(forward);
            return forward;
        }

        public void ResetSpeed()
        {
            mover.Setup(speed);
        }
    }
}
