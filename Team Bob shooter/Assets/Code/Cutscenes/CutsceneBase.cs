using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class CutsceneBase : MonoBehaviour
    {
        [SerializeField]
        private int cutsceneIndex = 0;

        public int CutsceneIndex
        {
            get { return cutsceneIndex; }
        }

        protected PlayerUnit playerUnit;

        private void Awake()
        {
            playerUnit = FindObjectOfType<PlayerUnit>();
        }

        public virtual void StartCutscene()
        {

        }

        public virtual void StopCutscene()
        {

        }
    }
}
