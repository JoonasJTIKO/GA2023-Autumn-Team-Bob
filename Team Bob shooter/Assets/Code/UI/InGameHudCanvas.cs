using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TeamBobFPS.UI
{
    public class InGameHudCanvas : MenuCanvas
    {
        [SerializeField]
        private TMP_Text magCountText;

        [SerializeField]
        private TMP_Text reserveCountText;

        public void UpdateMagCount(int amount)
        {
            magCountText.text = amount.ToString();
        }

        public void UpdateReserveCount(int amount)
        {
            reserveCountText.text = amount.ToString();
        }
    }
}
