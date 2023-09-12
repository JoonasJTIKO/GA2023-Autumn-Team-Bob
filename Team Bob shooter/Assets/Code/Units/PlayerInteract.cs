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

        private bool firstInitialize = true;

        private IInteractable currentTargeted = null;

        protected override void Awake()
        {
            base.Awake();

            playerUnit = GetComponent<PlayerUnit>();
        }

        private void Start()
        {
            interactAction = playerUnit.Inputs.Movement.Interact;

            if (firstInitialize)
            {
                interactAction.performed += Interact;

                firstInitialize = false;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (GameInstance.Instance == null) return;

            if (!firstInitialize)
            {
                interactAction.performed += Interact;
            }
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

            if (interactAvailable)
            {
                if (currentTargeted != hit.collider.GetComponent<IInteractable>())
                {
                    currentTargeted = hit.collider.GetComponent<IInteractable>();
                    GameInstance.Instance.GetInGameHudCanvas().SetInteractText(currentTargeted.PromptText);
                }
            }
            else
            {
                if (currentTargeted != null)
                {
                    currentTargeted = null;
                    GameInstance.Instance.GetInGameHudCanvas().SetInteractText("");
                }
            }
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
