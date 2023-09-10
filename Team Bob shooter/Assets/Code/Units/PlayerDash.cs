using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class PlayerDash : BaseFixedUpdateListener
    {
        [SerializeField]
        private float dashDistance = 2f;

        [SerializeField]
        private float dashSpeed = 4f;

        [SerializeField]
        private float dashCooldown = 1f;

        private float dashDuration;

        private bool canDash = true;

        private PlayerUnit playerUnit;

        private Mover mover;

        private Rigidbody rb;

        private Vector3 dashDirection;

        protected override void Awake()
        {
            base.Awake();

            playerUnit = GetComponent<PlayerUnit>();
            mover = GetComponent<Mover>();
            rb = GetComponent<Rigidbody>();
            dashDuration = dashDistance / dashSpeed;
        }

        public void Dash()
        {
            if (!canDash) return;

            playerUnit.LockMovement = true;
            mover.Setup(dashSpeed);
            rb.useGravity = false;
            rb.velocity = Vector3.zero;

            dashDirection = playerUnit.MoveDirection;
            if (dashDirection == Vector3.zero)
            {
                dashDirection = playerUnit.GetForwardDirection();
            }

            canDash = false;
            StartCoroutine(DashRoutine());
        }

        private IEnumerator DashRoutine()
        {
            float timer = 0;
            while (timer < dashDuration)
            {
                mover.Move(dashDirection);
                timer += Time.deltaTime;
                yield return null;
            }

            playerUnit.LockMovement = false;
            playerUnit.ResetSpeed();
            StartCoroutine(Cooldown());
        }

        private IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }
}
