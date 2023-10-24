using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class CutsceneManager : MonoBehaviour
    {
        [SerializeField]
        private CameraCutscene[] firstPersonCutscenes;

        [SerializeField]
        private FlyingCameraCutscene[] flyingCutscenes;

        private CutsceneBase currentCutscene;

        private void OnDisable()
        {
            if (currentCutscene != null)
            {
                currentCutscene.StopCutscene();
            }
        }

        public void PlayCutscene(int index)
        {
            foreach (var cutscene in firstPersonCutscenes)
            {
                if (cutscene.CutsceneIndex == index)
                {
                    cutscene.StartCutscene();
                    currentCutscene = cutscene;
                    return;
                }
            }

            foreach (var cutscene in flyingCutscenes)
            {
                if (cutscene.CutsceneIndex == index)
                {
                    cutscene.StartCutscene();
                    currentCutscene = cutscene;
                    return;
                }
            }
        }
    }
}
