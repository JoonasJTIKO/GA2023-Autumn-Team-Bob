using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamBobFPS
{
    public class FirstPersonCamera : BaseUpdateListener
    {
        [SerializeField]
        private Transform playerCam;

        [SerializeField]
        private float xSensitivity = 10f;

        [SerializeField]
        private float ySensitivity = 1f;

        [SerializeField]
        private float smoothing = 3f;

        private float inputX, inputY;

        private PlayerUnit playerUnit;

        private InputAction cameraX, cameraY;

        private Vector2 input;

        private Vector2 smoothLook, absoluteLook;

        private Vector2 orientation;

        private bool lockCamera = false;

        private void Start()
        {
            playerUnit = GetComponent<PlayerUnit>();
            cameraX = playerUnit.Inputs.Movement.CameraX;
            cameraY = playerUnit.Inputs.Movement.CameraY;

            cameraX.performed += context => input.x = context.ReadValue<float>();
            cameraY.performed += context => input.y = context.ReadValue<float>();

            orientation = transform.localRotation.eulerAngles;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerUnit.OnPlayerDied += OnPlayerDied;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (playerUnit == null) return;
            playerUnit.OnPlayerDied -= OnPlayerDied;
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (lockCamera) return;

            inputX = input.x * xSensitivity * smoothing;
            inputY = input.y * ySensitivity * smoothing;

            smoothLook.x = Mathf.Lerp(smoothLook.x, inputX, 1f / smoothing);
            smoothLook.y = Mathf.Lerp(smoothLook.y, inputY, 1f / smoothing);

            absoluteLook += smoothLook;

            absoluteLook.y = Mathf.Clamp(absoluteLook.y, -85f, 85f);

            playerCam.transform.localRotation = Quaternion.AngleAxis(-absoluteLook.y, Quaternion.Euler(orientation) * Vector3.right) * Quaternion.Euler(orientation);

            Quaternion yRotation = Quaternion.AngleAxis(absoluteLook.x, Vector3.up);
            transform.localRotation = yRotation * Quaternion.Euler(orientation);
        }

        private void OnPlayerDied()
        {
            lockCamera = true;
        }
    }
}
