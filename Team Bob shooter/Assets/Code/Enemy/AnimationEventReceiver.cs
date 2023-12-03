using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class AnimationEventReceiver : MonoBehaviour
    {
        private MeleeEnemy meleeEnemy;

        private RangeEnemy rangeEnemy;

        private FlyingEnemy flyingEnemy;

        private void Awake()
        {
            meleeEnemy = GetComponentInParent<MeleeEnemy>();
            rangeEnemy = GetComponentInParent<RangeEnemy>();
            flyingEnemy = GetComponentInParent<FlyingEnemy>();
        }

        public void Shoot()
        {
            if (meleeEnemy != null)
            {
                meleeEnemy.DoAttack();
            }
            else if (rangeEnemy != null)
            {
                rangeEnemy.Shoot();
            }
            else if (flyingEnemy != null)
            {
                flyingEnemy.Shoot();
            }
        }

        public void AttemptPositionChange()
        {
            if (rangeEnemy == null) return;

            rangeEnemy.AttemptPositionChange();
        }
    }
}
