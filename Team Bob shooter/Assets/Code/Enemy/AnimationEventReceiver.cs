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
            if (rangeEnemy == null) return;

            rangeEnemy.Shoot();
        }

        public void AttemptPositionChange()
        {
            if (rangeEnemy == null) return;

            rangeEnemy.AttemptPositionChange();
        }
    }
}
