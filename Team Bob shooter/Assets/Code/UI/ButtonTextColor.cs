using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TeamBobFPS
{
    public class ButtonTextColor : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField]
        private Color color;

        private Color defaultColor;

        private TMP_Text textComponent;

        private void Awake()
        {
            textComponent = GetComponentInChildren<TMP_Text>();
            defaultColor = textComponent.color;
        }

        public void OnSelect(BaseEventData eventData)
        {
            textComponent.color = color;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            textComponent.color = defaultColor;
        }
    }
}
