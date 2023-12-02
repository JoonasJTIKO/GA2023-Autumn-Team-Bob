using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS.UI
{
    public class LoadoutSelectWeaponModel : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] models;

        public void EnableModel(int modelIndex)
        {
            foreach (var model in models)
            {
                model.SetActive(false);
                models[modelIndex].SetActive(true);
            }
        }
    }
}
