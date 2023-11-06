using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace TeamBobFPS
{
    public class PlayerHealthIndicator : MonoBehaviour, IHealth
    {
        public float Health
        {
            get;
            protected set;
        }

        public float intensity = 0;

        PostProcessVolume volume;
        Vignette vignette;

        void Start()
        {
            volume = GetComponent<PostProcessVolume>();

            volume.profile.TryGetSettings(out vignette);

            if(!vignette)
            {
                Debug.LogError("No vignette found");
            }
            else
            {
                vignette.enabled.Override(false);
            }
        }

        public bool AddHealth(float amount)
        {
            float startingHealth = Health;
            Health += amount;
            if(Health > startingHealth)
            {
                //HealEffect
                return true;
            }
            return false;
        }

        public bool RemoveHealth(float amount, EnemyGibbing.DeathType potentialDeathType = EnemyGibbing.DeathType.Normal)
        {
            float startingHealth = Health;
            Health -= amount;
            if(Health < 75)
            {
                StartCoroutine(TakeDamageEffect());
                return true;
            }
            return false;
        }

        private IEnumerator TakeDamageEffect()
        {
            intensity = 0.4f;

            vignette.enabled.Override(true);
            

            while (Health < 75)
            {
                vignette.intensity.Override(0.4f);
            }
            while(Health < 50)
            {

            }
            while (Health < 25)
            {

            }

            vignette.enabled.Override(false);
            yield break;
        }

    }
}
