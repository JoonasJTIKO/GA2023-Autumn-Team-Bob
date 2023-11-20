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

        private List<GameObject> enemiesInArea = new List<GameObject>();

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == 3)
            {
                GameInstance.Instance.GetMapAreaManager().PlayerLocation = areaIndex;
            }
            else if (other.gameObject.layer == 7 && !enemiesInArea.Contains(other.gameObject))
            {
                enemiesInArea.Add(other.gameObject);

                MeleeEnemy meleeEnemy = other.gameObject.GetComponent<MeleeEnemy>();
                if (meleeEnemy != null)
                {
                    meleeEnemy.CurrentMapArea = areaIndex;
                    return;
                }
                RangeEnemy rangeEnemy = other.gameObject.GetComponent<RangeEnemy>();
                if (rangeEnemy != null)
                {
                    rangeEnemy.CurrentMapArea = areaIndex;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == 3)
            {
                GameInstance.Instance.GetMapAreaManager().PlayerLocation = -1;
            }
            else if (other.gameObject.layer == 7 && enemiesInArea.Contains(other.gameObject))
            {
                enemiesInArea.Remove(other.gameObject);
            }
        }
    }
}
