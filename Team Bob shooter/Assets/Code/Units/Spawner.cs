using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS

{
    public abstract class Spawner<TComponent> : MonoBehaviour
      where TComponent : Component
    {
        [SerializeField] private TComponent prefab;

        public TComponent Prefab
        {
            get { return this.prefab; }
            protected set { this.prefab = value; }
        }

        public virtual void Setup(TComponent prefab = null)
        {
            if (prefab != null)
            {
                this.prefab = prefab;
            }
        }

        public abstract TComponent Create(Vector3 position, Quaternion rotation, Transform parent);
        public abstract bool Recycle(TComponent item);
    }
}
