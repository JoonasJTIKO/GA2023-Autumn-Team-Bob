using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class ArenaLoadZone : MonoBehaviour
    {
        [SerializeField]
        private StateType targetState;

        private void OnTriggerEnter(Collider other)
        {
            GameInstance.Instance.GetGameStateManager().Go(targetState);
        }
    }
}
