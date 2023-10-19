using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class BouncyObject : MonoBehaviour
    {
        [SerializeField]
        private float bounceStrength;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 3)
            {
                PlayerUnit playerUnit = other.gameObject.GetComponent<PlayerUnit>();
                if (playerUnit != null)
                {
                    playerUnit.QueueJump(bounceStrength);
                }
            }
        }
    }
}
