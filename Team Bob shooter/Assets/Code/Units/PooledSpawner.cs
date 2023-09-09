using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS

{
    public class PooledSpawner<TComponent> : Spawner<TComponent>
        where TComponent : Component
    {
        [SerializeField] private int capacity = 1;

        private ComponentPool<TComponent> pool;

        protected ComponentPool<TComponent> Pool
        {
            get { return pool; }
        }

        public override void Setup(TComponent prefab = null)
        {
            base.Setup(prefab);

            pool = new ComponentPool<TComponent>(Prefab, capacity);
        }

        public override TComponent Create(Vector3 position, Quaternion rotation, Transform parent)
        {
            TComponent item = pool.Get();
            if (item != null)
            {
                // If the parent is null, Unity will put the GameObject to scene's root level.
                // TODO: Local or global positions?
                item.transform.parent = parent;
                item.transform.localPosition = position;
                item.transform.localRotation = rotation;
            }

            return item;
        }

        public override bool Recycle(TComponent item)
        {
            return pool.Return(item);
        }
    }
}
