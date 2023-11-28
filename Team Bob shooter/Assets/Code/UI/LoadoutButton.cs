using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TeamBobFPS.UI
{
    public class LoadoutButton : MonoBehaviour, ISelectHandler
    {
        [SerializeField]
        private int modelIndex;

        private LoadoutSelectCanvas loadoutSelectCanvas;

        private void Awake()
        {
            loadoutSelectCanvas = GetComponentInParent<LoadoutSelectCanvas>();
        }

        public void OnSelect(BaseEventData eventData)
        {
            loadoutSelectCanvas.SetModel(modelIndex);
        }
    }
}
