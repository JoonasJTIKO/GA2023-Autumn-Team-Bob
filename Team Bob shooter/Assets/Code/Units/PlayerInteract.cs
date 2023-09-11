using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamBobFPS
{
    public class PlayerInteract : BaseUpdateListener
    {
        [SerializeField]
        private Camera playerCam;

        [SerializeField]
        private float interactRange = 2f;

        [SerializeField]
        private LayerMask layerMask;

        private PlayerUnit playerUnit;

        private InputAction interactAction;

        private bool interactAvailable = false;

        private RaycastHit hit;

        protected override void Awake()
        {
            base.Awake();

            playerUnit = GetComponent<PlayerUnit>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (GameInstance.Instance == null) return;

            interactAction = playerUnit.Inputs.Movement.Interact;
            interactAction.performed += Interact;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (GameInstance.Instance == null) return;

            interactAction.performed -= Interact;
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            interactAvailable = Physics.Raycast(playerCam.transform.position,
                playerCam.transform.forward, out hit, interactRange, layerMask);
        }

        private void Interact(InputAction.CallbackContext context)
        {
            if (interactAvailable)
            {
                hit.collider.GetComponent<IInteractable>().OnInteract(playerUnit.CurrentWeaponSlot);
            }
        }
    }
}
