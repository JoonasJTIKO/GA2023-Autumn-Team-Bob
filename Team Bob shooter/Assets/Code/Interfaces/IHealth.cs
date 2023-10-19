using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public interface IHealth
    {
        float Health { get; }

        bool AddHealth(float amount);

        bool RemoveHealth(float amount, EnemyGibbing.DeathType potentialDeathType = EnemyGibbing.DeathType.Normal);
    }
}
