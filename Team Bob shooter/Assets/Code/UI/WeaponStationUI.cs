using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TeamBobFPS
{
    public class WeaponStationUI : MonoBehaviour
    {
        private TextWriteOverTime[] writeComponents;

        private void Awake()
        {
            writeComponents = GetComponentsInChildren<TextWriteOverTime>();
        }

        private void OnEnable()
        {
            if (writeComponents == null) return;

            foreach (var component in writeComponents)
            {
                component.StartWrite();
            }
        }
    }
}
