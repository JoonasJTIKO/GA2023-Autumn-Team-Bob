using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace TeamBobFPS
{
    public class DecalPaint : MonoBehaviour
    {
        [Serializable]
        public class DecalTextureData
        {
            public Sprite sprite;
            public Vector3 size;
        }

        [SerializeField]
        private DecalTextureData[] decalData;

        [SerializeField]
        private DecalProjector decalProjector;

        private Material[] decalMaterials;

        private int decalIndex = 0;

        private ComponentPool<DecalProjector> decalPool;

        private int poolSize = 20;

        private DecalProjector[] activeDecals;

        private int index = 0;

        private void Awake()
        {
            decalMaterials = new Material[decalData.Length];

            decalPool = new ComponentPool<DecalProjector>(decalProjector, poolSize);
            activeDecals = new DecalProjector[poolSize];
        }

        public void ChangeDecal(int index)
        {
            if (index >= 0 && index < decalData.Length)
            {
                decalIndex = index;
            }
        }

        public void ApplyDecal(Vector3 point, Vector3 normal)
        {
            if (activeDecals[index] != null)
            {
                decalPool.Return(activeDecals[index]);
            }
            activeDecals[index] = decalPool.Get();
            activeDecals[index].transform.position = point;

            if (decalMaterials[decalIndex] == null)
            {
                decalMaterials[decalIndex] = new Material(activeDecals[index].material);
            }

            activeDecals[index].material = decalMaterials[decalIndex];
            activeDecals[index].material.SetTexture("Base_Map", decalData[decalIndex].sprite.texture);
            activeDecals[index].size = decalData[decalIndex].size;
            activeDecals[index].transform.forward = -normal;
            index++;
            if (index >= activeDecals.Length) index = 0;
        }
    }
}
