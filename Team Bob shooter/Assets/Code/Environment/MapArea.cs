using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class MapArea : MonoBehaviour
    {
        [SerializeField]
        private int areaIndex = 0;

        public int AreaIndex
        {
            get { return areaIndex; }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == 3)
            {
                GameInstance.Instance.GetMapAreaManager().PlayerLocation = areaIndex;
            }
            else if (other.gameObject.layer == 10)
            {
                MeleeEnemy meleeEnemy = other.gameObject.GetComponent<MeleeEnemy>();
                if (meleeEnemy != null)
                {
                    meleeEnemy.CurrentMapArea = areaIndex;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == 3)
            {
                GameInstance.Instance.GetMapAreaManager().PlayerLocation = -1;
            }
        }
    }
}
