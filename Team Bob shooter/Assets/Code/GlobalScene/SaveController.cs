using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

namespace TeamBobFPS.Save
{
    public class SaveController : MonoBehaviour
    {
        private SaveSystem saveSystem;

        public SaveSystem SaveSystem
        {
            get { return saveSystem; }
        }


        private void Awake()
        {
            ISaveSystem saver = new BinarySaver();
            saveSystem = new SaveSystem(saver, saver);
        }

        private void Start()
        {
            this.QuickLoad();
        }

        public void QuickSave()
        {
            saveSystem.QuickSave();
        }

        public void QuickLoad() { saveSystem.QuickLoad(); }

    }
}
