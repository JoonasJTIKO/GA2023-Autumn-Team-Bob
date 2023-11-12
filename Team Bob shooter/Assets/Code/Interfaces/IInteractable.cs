using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public interface IInteractable
    {
        string PromptText { get; }

        bool OnInteract(int currentWeapon);

        void OnHover(bool state);
    }
}
