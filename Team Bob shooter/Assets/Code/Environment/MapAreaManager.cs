using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class MapAreaManager : MonoBehaviour
    {
        public int PlayerLocation = 0;

        public bool PlayerInArea(int area)
        {
            if (PlayerLocation == area)
            {
                return true;
            }
            return false;
        }

        public void PlayerEnteredArea(int area)
        {
            PlayerLocation = area;
        }
    }
}
