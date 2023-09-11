using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public interface IInteractable
    {
        bool OnInteract(int currentWeapon);
    }
}
