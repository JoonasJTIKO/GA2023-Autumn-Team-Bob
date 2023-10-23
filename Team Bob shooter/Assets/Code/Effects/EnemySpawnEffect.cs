using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class EnemySpawnEffect : MonoBehaviour
    {
        private SkinnedMeshRenderer[] renderers;

        private Material[] materials;

        private void Awake()
        {
            renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            materials = new Material[renderers.Length];

            int index = 0;
            foreach (var ren in renderers)
            {
                materials[index] = ren.material;
                index++;
            }
        }

        public void PlayEffect()
        {
            StartCoroutine(SpawnEffect());
        }

        private IEnumerator SpawnEffect()
        {
            float progress = 0.5f;
            while (progress > -1)
            {
                progress -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;

                foreach (var material in materials)
                {
                    material.SetFloat("FadeProgress", progress);
                }
                yield return null;
            }
        }
    }
}
