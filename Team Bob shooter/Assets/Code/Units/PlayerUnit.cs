using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
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

        [SerializeField]
        private float accelerationTime = 0.25f;

        [SerializeField]
        private float decelerationTime = 0.25f;

        private PlayerInputs playerInputs;

        public PlayerInputs Inputs
        {
            get { return playerInputs; }
        }

        private InputAction moveAction, jumpAction, menu;

        private Mover mover;

        private Rigidbody rb;

        private UnitHealth unitHealth;

        private Camera playerCam;

        private FirstPersonCamera firstPersonCamera;

        private WeaponSwap weaponSwap;

        private FootstepPlayer footstepPlayer;

        private float footStepTimer = 0f;

        private Vector3 pauseVelocity;

        public Camera PlayerCam
        {
            get { return playerCam; }
        }

        public Vector3 MoveDirection
        {
            get;
            private set;
        }

        public int CurrentWeaponSlot;

        public float MoveSpeedModifier = 0f;

        private float SetJumpStrength = 0f;

        public bool IsGrounded
        {
            get { return mover.IsGrounded; }
        }

        public float GravityScale = 1f;

        public bool LockMovement = false;

        private bool jumping = false;

        private float waitFrames;

        private bool doJump = false;

        public bool EnableDoubleJump = false;

        private bool canDoubleJump = true;

        private bool lockInputs = false;

        private bool forceLockedInputs = false;

        private bool isPaused = false;

        private Animator animator;

        public event Action OnPlayerDied;

        protected override void Awake()
        {
            base.Awake();
            if (GameInstance.Instance == null) return;

            firstPersonCamera = GetComponent<FirstPersonCamera>();
            weaponSwap = GetComponent<WeaponSwap>();
            mover = GetComponent<Mover>();
            mover.Setup(speed, accelerationTime, decelerationTime, false);
            rb = GetComponent<Rigidbody>();
            unitHealth = GetComponent<UnitHealth>();
            playerCam = GetComponentInChildren<Camera>();
            footstepPlayer = GetComponent<FootstepPlayer>();
            playerInputs = new PlayerInputs();
            moveAction = playerInputs.Movement.Move;
            jumpAction = playerInputs.Movement.Jump;

            animator = gameObject.GetComponentInChildren<Animator>();
            isPaused = false;
        }

        private void Start()
        {
            StartCoroutine(StartLockout());
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (GameInstance.Instance == null) return;

            playerInputs.Movement.Enable();
            jumpAction.performed += QueueJump;

            unitHealth.OnDied += OnDie;
            RocketProjectile.PlayerHit += ReceiveKnockback;

            menu = playerInputs.Menu.Pause;
            menu.Enable();

            menu.performed += Pause;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (GameInstance.Instance == null) return;

            playerInputs.Movement.Disable();
            jumpAction.performed -= QueueJump;

            unitHealth.OnDied -= OnDie;
            RocketProjectile.PlayerHit -= ReceiveKnockback;

            menu.Disable();
            menu.performed -= Pause;
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            if (GameInstance.Instance.GetUpdateManager().fixedTimeScale == 0)
            {
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
                mover.Move(Vector3.zero);
                return;
            }
            else
            {
                rb.useGravity = true;
            }

            if (IsGrounded && rb.velocity.y <= 0)
            {
                if (waitFrames > 10)
                {
                    waitFrames--;
                }
                else if (jumping)
                {
                    jumping = false;
                    weaponSwap.CurrentWeaponLand();
                }
            }

            if (!IsGrounded && !jumping && rb.velocity.y > 0)
            {
                rb.velocity = new(rb.velocity.x, 0, rb.velocity.z);
            }

            if (doJump && !lockInputs)
            {
                doJump = false;
                Jump(SetJumpStrength);
            }

            if (!LockMovement) rb.useGravity = !mover.OnSlope();

            if (!mover.OnSlope() && !IsGrounded && rb.useGravity)
            {
                rb.AddForce(Physics.gravity * GravityScale * rb.mass * fallSpeedModifier, ForceMode.Force);
            }

            if (lockInputs)
            {
                mover.Move(Vector3.zero);
                weaponSwap.SetCurrentWeaponWalking(false);
                return;
            }

            Vector3 camForward = new(playerCam.transform.forward.x, 0, playerCam.transform.forward.z);
            Vector3 camRight = new(playerCam.transform.right.x, 0, playerCam.transform.right.z);
            camForward.Normalize();
            camRight.Normalize();

            Vector3 move = moveAction.ReadValue<Vector3>();
            move.Normalize();

            if (move != Vector3.zero && IsGrounded)
            {
                weaponSwap.SetCurrentWeaponWalking(true);
            }
            else if (!IsGrounded || move == Vector3.zero)
            {
                weaponSwap.SetCurrentWeaponWalking(false);
            }

            if (move != Vector3.zero)
            {
                move = move.x * camRight + move.z * camForward;
            }

            if (!jumping)
            {
                move = mover.GetSlopeDirection(move);
            }

            MoveDirection = move;

            if (!LockMovement)
            {
                mover.Move(move);
            }

            if (!LockMovement && !jumping && move != Vector3.zero)
            {
                ProgressFootstepTimer(fixedDeltaTime);
            }
            else
            {
                footStepTimer = 0f;
            }
        }

        private void ProgressFootstepTimer(float deltaTime)
        {
            if (footStepTimer == 0f)
            {
                footstepPlayer.PlayFootstep();
            }

            footStepTimer += deltaTime;
            if (footStepTimer >= 0.3f)
            {
                footStepTimer = 0f;
            }
        }

        private void QueueJump(InputAction.CallbackContext context)
        {
            doJump = true;
        }

        public void QueueJump(float jumpStrength)
        {
            SetJumpStrength = jumpStrength;
            doJump = true;
        }

        private void Jump(float strength = 0)
        {
            if (!IsGrounded)
            {
                if (!EnableDoubleJump || !canDoubleJump) return;
                canDoubleJump = false;
            }
            else
            {
                canDoubleJump = true;
            }

            rb.useGravity = true;
            rb.velocity = new(rb.velocity.x, 0, rb.velocity.z);
            if (strength == 0) strength = jumpStrength;
            rb.AddForce(Vector3.up * strength, ForceMode.Impulse);
            jumping = true;
            waitFrames = 10;
            SetJumpStrength = 0f;

            weaponSwap.CurrentWeaponJump();
        }

        private void ReceiveKnockback(Vector3 origin, float strength)
        {
            Vector3 direction = transform.position - origin;
            direction.Normalize();

            if (IsGrounded && direction.y < 0) return;

            rb.AddForce(direction * strength, ForceMode.Impulse);
        }

        private void OnDie(float explosionStrength, Vector3 explosionPoint, EnemyGibbing.DeathType deathType = EnemyGibbing.DeathType.Normal)
        {
            OnPlayerDied?.Invoke();
            LockMovement = true;
            jumpAction.performed -= QueueJump;
            GameInstance.Instance.GetPlayerDefeatedCanvas().Show();
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
            if (mover == null) return;

            mover.Setup(speed * MoveSpeedModifier, accelerationTime, decelerationTime, false);
        }

        public void LockControls(bool lockMovement = false, bool lockWeapons = false, bool lockLook = false)
        {
            if (lockMovement || lockWeapons || lockLook)
            {
                forceLockedInputs = true;
            }
            else
            {
                forceLockedInputs = false;
            }
            lockInputs = lockMovement;
            weaponSwap.LockInputs(lockWeapons);
            firstPersonCamera.LockInputs(lockLook);
        }

        private IEnumerator StartLockout()
        {
            lockInputs = true;
            weaponSwap.LockInputs(true);
            firstPersonCamera.LockInputs(true);

            float timer = 0;
            bool fadeOutStarted = false;
            while (timer < 2f)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                if (timer <= 0.5f && !fadeOutStarted)
                {
                    fadeOutStarted = true;
                    GameInstance.Instance.GetFadeCanvas().FadeFrom(1f);
                }
                yield return null;
            }

            lockInputs = false;
            if (!forceLockedInputs)
            {
                weaponSwap.LockInputs(false);
                firstPersonCamera.LockInputs(false);
            }
        }

        public void Pause(InputAction.CallbackContext context)
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                GameInstance.Instance.GetUpdateManager().timeScale = 0;
                GameInstance.Instance.GetUpdateManager().fixedTimeScale = 0;
                GameInstance.Instance.GetPauseMenu().Show();
                animator.enabled = false;
                LockMovement = true;
                isPaused = true;
                pauseVelocity = rb.velocity;
                rb.velocity = Vector3.zero;
                rb.useGravity = false;
            }
            else
            {
                GameInstance.Instance.GetUpdateManager().timeScale = 1;
                GameInstance.Instance.GetUpdateManager().fixedTimeScale = 1;
                GameInstance.Instance.GetPauseMenu().Hide();
                animator.enabled = true;
                LockMovement = false;
                isPaused = false;
                rb.velocity = pauseVelocity;
                rb.useGravity = true;
            }
        }
    }
}
