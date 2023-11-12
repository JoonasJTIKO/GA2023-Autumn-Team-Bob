using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class CutsceneTrigger : MonoBehaviour
    {
        [SerializeField]
        private int cutsceneIndex = 0;

        private void OnTriggerEnter(Collider other)
        {
            GetComponent<Collider>().enabled = false;
            FindObjectOfType<CutsceneManager>().PlayCutscene(cutsceneIndex);
        }
    }
}
