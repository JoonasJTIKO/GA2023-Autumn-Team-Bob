using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
	public class ComponentPool<TComponent> : Pool<TComponent>
        where TComponent : Component
    {
        public ComponentPool(TComponent prefab, int capacity)
            : base(prefab, capacity)
        {
        }

        protected override bool IsActive(TComponent item)
        {
            return item.gameObject.activeSelf;
        }

        protected override void SetActive(TComponent item, bool setActive)
        {
            item.gameObject.SetActive(setActive);
        }
    }
}

