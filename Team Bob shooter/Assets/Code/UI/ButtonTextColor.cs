using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeamBobFPS
{
    public class ButtonTextColor : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField]
        private Color color;

        [SerializeField]
        private bool canBeUnlocked = false;

        private Color defaultColor;

        private TMP_Text textComponent;

        private Button button;

        private string weaponName;

        private void Awake()
        {
            textComponent = GetComponentInChildren<TMP_Text>();
            defaultColor = textComponent.color;
            button = GetComponent<Button>();
            weaponName = textComponent.text;
        }

        private void Update()
        {
            if (!canBeUnlocked) return;

            if (button.interactable)
            {
                textComponent.text = weaponName;
            }
            else
            {
                textComponent.text = "Not available";
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            textComponent.color = color;
            GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_UI_SELECT, transform.position, make2D: true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            textComponent.color = defaultColor;
        }
    }
}
