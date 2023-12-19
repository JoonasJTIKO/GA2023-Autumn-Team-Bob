using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamBobFPS
{
    public class Crosshair : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] crosshairs;

        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public void SetCrosshair(int index)
        {
            image.sprite = crosshairs[index];

            transform.localScale = new Vector3(1, 1, 1);
            if (index == 1)
            {
                transform.localScale = new Vector3(2, 2, 2);
            }
        }
    }
}
