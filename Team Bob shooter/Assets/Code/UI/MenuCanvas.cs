using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TeamBobFPS.UI
{
    public class MenuCanvas : MonoBehaviour
    {
        [SerializeField]
        private GameObject eventSystem;

        [SerializeField]
        private GameObject initialSelectedObject;

        public virtual void Show()
        {
            gameObject.SetActive(true);

            eventSystem.SetActive(true);

            if (initialSelectedObject != null)
            {
                eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(initialSelectedObject);
            }

        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);

            eventSystem.SetActive(false);
        }
    }
}
