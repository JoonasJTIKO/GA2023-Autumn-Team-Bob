using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class AnimationEventReceiver : MonoBehaviour
    {
        private RangeEnemy rangeEnemy;

        private void Awake()
        {
            rangeEnemy = GetComponentInParent<RangeEnemy>();
        }

        public void Shoot()
        {
            rangeEnemy.Shoot();
        }

        public void AttemptPositionChange()
        {
            rangeEnemy.AttemptPositionChange();
        }
    }
}
