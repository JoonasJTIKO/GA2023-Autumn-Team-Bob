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

        private Dictionary<GameObject, MeleeEnemy> meleeEnemiesInArea = new Dictionary<GameObject, MeleeEnemy>();
        private Dictionary<GameObject, RangeEnemy> rangeEnemiesInArea = new Dictionary<GameObject, RangeEnemy>();

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == 3)
            {
                GameInstance.Instance.GetMapAreaManager().PlayerLocation = areaIndex;
            }
            else if (other.gameObject.layer == 7)
            {
                if (other.gameObject.tag == "MeleeEnemy" && !meleeEnemiesInArea.ContainsKey(other.gameObject))
                {
                    meleeEnemiesInArea.Add(other.gameObject, other.gameObject.GetComponent<MeleeEnemy>());
                }

                if (other.gameObject.tag == "RangeEnemy" && !rangeEnemiesInArea.ContainsKey(other.gameObject))
                {
                    rangeEnemiesInArea.Add(other.gameObject, other.gameObject.GetComponent<RangeEnemy>());
                }

                if (meleeEnemiesInArea.ContainsKey(other.gameObject))
                {
                    meleeEnemiesInArea[other.gameObject].CurrentMapArea = areaIndex;
                    return;
                }

                if (rangeEnemiesInArea.ContainsKey(other.gameObject))
                {
                    rangeEnemiesInArea[other.gameObject].CurrentMapArea = areaIndex;
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
