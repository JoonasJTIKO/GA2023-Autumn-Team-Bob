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

        private bool jumping = false;

        public static event Action OnPlayerHealthChanged;

        protected override void Awake()
        {
            base.Awake();
            if (GameInstance.Instance == null) return;

            mover = GetComponent<Mover>();
            mover.Setup(speed);
            rb = GetComponent<Rigidbody>();
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
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (GameInstance.Instance == null) return;

            playerInputs.Movement.Disable();
            jumpAction.performed -= Jump;
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            rb.useGravity = !mover.OnSlope();

            if (!mover.OnSlope() && !mover.IsGrounded)
            {
                rb.AddForce(Physics.gravity * rb.mass * fallSpeedModifier, ForceMode.Force);
            }

            Vector3 camForward = new(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
            Vector3 camRight = new(Camera.main.transform.right.x, 0, Camera.main.transform.right.z);
            camForward.Normalize();
            camRight.Normalize();

            Vector3 move = moveAction.ReadValue<Vector3>();
            move.Normalize();

            if (move != Vector3.zero)
            {
                move = move.x * camRight + move.z * camForward;
            }

            move = mover.GetSlopeDirection(move);

            mover.Move(move);

            if (mover.IsGrounded && jumping) jumping = false;
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (jumping || !mover.IsGrounded) return;

            rb.useGravity = true;
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
        }

        protected override void ChangeHealth(float amount)
        {
            base.ChangeHealth(amount);

            OnPlayerHealthChanged?.Invoke();
        }
    }
}
