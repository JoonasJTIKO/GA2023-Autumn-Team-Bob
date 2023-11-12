using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class EnemySpawnEffect : MonoBehaviour
    {
        private SkinnedMeshRenderer[] renderers;

        private void Awake()
        {
            renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        }

        public void PlayEffect()
        {
            StartCoroutine(SpawnEffect());
        }

        private IEnumerator SpawnEffect()
        {
            float progress = 0.5f;
            while (progress > -1.1f)
            {
                foreach (var renderer in renderers)
                {
                    renderer.material.SetFloat("_FadeProgress", progress);
                }
                progress -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale * 0.5f;
                yield return null;
            }
        }

        
    }
}
