using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class EnemyDespawnEffect : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer meshRenderer;

        private SkinnedMeshRenderer[] renderers;

        public event Action OnDespawnEffectComplete;

        private void Awake()
        {
            if (meshRenderer != null) return;

            renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        }

        public void PlayDespawnEffect()
        {
            StartCoroutine(DespawnEffect());
        }

        private IEnumerator DespawnEffect()
        {
            float progress = -1f;
            while (progress < 0.5f)
            {
                if (meshRenderer != null)
                {
                    meshRenderer.material.SetFloat("_FadeProgress", progress);
                }
                else
                {
                    foreach (var renderer in renderers)
                    {
                        renderer.material.SetFloat("_FadeProgress", progress);
                    }
                }

                progress += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale * 0.5f;
                yield return null;
            }

            OnDespawnEffectComplete?.Invoke();
        }
    }
}
