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

        private PlayerInputs playerInputs;

        public PlayerInputs Inputs
        {
            get { return playerInputs; }
        }

        private InputAction moveAction;

        private Mover mover;

        private Rigidbody rb;

        protected override void Awake()
        {
            base.Awake();

            mover = GetComponent<Mover>();
            mover.Setup(speed);
            rb = GetComponent<Rigidbody>();
            playerInputs = new PlayerInputs();
            playerInputs.Movement.Enable();
            moveAction = playerInputs.Movement.Move;
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            rb.useGravity = !mover.OnSlope();

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
        }
    }
}
