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

        [SerializeField]
        private GameObject VFX;

        private float dashDuration;

        private bool canDash = true;

        private PlayerUnit playerUnit;

        private Mover mover;

        private Rigidbody rb;

        private Vector3 dashDirection;

        private Coroutine dashRoutine = null;

        protected override void Awake()
        {
            base.Awake();

            playerUnit = GetComponent<PlayerUnit>();
            mover = GetComponent<Mover>();
            rb = GetComponent<Rigidbody>();
            dashDuration = dashDistance / dashSpeed;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            playerUnit.OnPlayerDied += AbortDash;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            playerUnit.OnPlayerDied -= AbortDash;
        }

        /// <summary>
        /// Initiates dash, setting direction to movement direction / forward if not moving
        /// </summary>
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
            dashRoutine = StartCoroutine(DashRoutine());
        }

        private void AbortDash()
        {
            if (dashRoutine != null)
            {
                StopCoroutine(dashRoutine);
                playerUnit.ResetSpeed();
                StartCoroutine(Cooldown());
            }
        }

        private IEnumerator DashRoutine()
        {
            VFX.SetActive(true);
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
            StartCoroutine(VFXDisable());
        }

        private IEnumerator VFXDisable()
        {
            float timer = 0;
            while (timer < 0.2f)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            VFX.SetActive(false);
        }

        private IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }
}
