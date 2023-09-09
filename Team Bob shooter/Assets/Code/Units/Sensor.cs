using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Sensor : MonoBehaviour
    {
        private int collisionCount = 0;

        public bool Active
        {
            get;
            private set;
        }

        private void OnTriggerEnter(Collider other)
        {
            collisionCount++;
            Active = true;
        }

        private void OnTriggerStay(Collider other)
        {
            Active = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (collisionCount > 0)
            {
                collisionCount--;
            }

            if (collisionCount == 0)
            {
                Active = false;
            }
        }
    }
}
