using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class DamageBox : MonoBehaviour
    {
        public float Damage
        {
            get;
            set;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 3)
            {
                UnitHealth unitHealth = other.GetComponent<UnitHealth>();

                if (unitHealth != null)
                {
                    unitHealth.RemoveHealth(Damage);
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
