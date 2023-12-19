using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class HubProgressionCheck : MonoBehaviour
    {
        [SerializeField]
        private ArenaLoadZone[] levelPortals;

        private void Start()
        {
            OnLoadScene();
        }

        private void OnLoadScene()
        {
            GameProgressionManager.GameProgress currentProgress = GameInstance.Instance.GetGameProgressionManager().CurrentGameProgress;

            switch (currentProgress)
            {
                case GameProgressionManager.GameProgress.NoneCleared:
                    levelPortals[0].gameObject.SetActive(true);
                    levelPortals[0].SetTargetState(StateType.Arena1);
                    break;
                case GameProgressionManager.GameProgress.FirstCleared:
                    levelPortals[0].gameObject.SetActive(true);
                    //TEMP
                    levelPortals[0].SetTargetState(StateType.Arena1);
                    break;
                case GameProgressionManager.GameProgress.SecondCleared:
                    levelPortals[0].gameObject.SetActive(true);
                    //TEMP
                    levelPortals[0].SetTargetState(StateType.Arena1);
                    break;
            }
        }
    }
}
