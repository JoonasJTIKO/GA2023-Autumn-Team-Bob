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

        [SerializeField]
        private Material[] materials;

        [SerializeField]
        private DecalProjector decalProjector;

        private int decalIndex = 0;

        private ComponentPool<DecalProjector> decalPool;

        private int poolSize = 20;

        private DecalProjector[] activeDecals;

        private int index = 0;

        private void Awake()
        {

            decalPool = new ComponentPool<DecalProjector>(decalProjector, poolSize);
            activeDecals = new DecalProjector[poolSize];
        }

        public void ApplyDecal(Vector3 point, Vector3 normal)
        {
            if (activeDecals[index] != null)
            {
                decalPool.Return(activeDecals[index]);
            }
            activeDecals[index] = decalPool.Get();
            activeDecals[index].transform.position = point;

            int materialIndex = UnityEngine.Random.Range(0, materials.Length - 1);

            activeDecals[index].material = materials[materialIndex];
            float random = UnityEngine.Random.Range(1f, 2f);
            activeDecals[index].size = new Vector3(random, random, random);
            activeDecals[index].transform.forward = -normal;
            index++;
            if (index >= activeDecals.Length) index = 0;
        }
    }
}
