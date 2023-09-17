using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TeamBobFPS.UI
{
    public class MenuCanvas : BaseUpdateListener
    {
        [SerializeField]
        private EventSystem eventSystem;

        [SerializeField]
        private GameObject initialSelectedObject;

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (initialSelectedObject != null && eventSystem.currentSelectedGameObject == null)
            {
                eventSystem.SetSelectedGameObject(initialSelectedObject);
            }
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);

            eventSystem.gameObject.SetActive(true);

            if (initialSelectedObject != null)
            {
                eventSystem.SetSelectedGameObject(initialSelectedObject);
            }

        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);

            eventSystem.gameObject.SetActive(false);
        }
    }
}
