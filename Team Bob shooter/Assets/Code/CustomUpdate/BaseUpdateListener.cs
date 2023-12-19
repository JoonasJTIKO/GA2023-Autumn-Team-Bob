using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class BaseUpdateListener : MonoBehaviour, IUpdateListener
    {
        protected UpdateManager updateManager;

        protected int id;

        private bool listenerAdded = false;

        public int ID { get { return id; } }

        protected virtual void Awake()
        {
            if (GameInstance.Instance == null) return;

            updateManager = GameInstance.Instance.GetUpdateManager();

            id = updateManager.GetUniqueID();
        }

        protected virtual void OnEnable()
        {
            if (updateManager == null || !updateManager.Ready) return;

            updateManager.AddUpdateListener(this);
            listenerAdded = true;
        }

        protected virtual void OnDisable()
        {
            if (updateManager == null || !updateManager.Ready) return;

            updateManager.RemoveUpdateListener(this);
            listenerAdded = false;
        }

        protected virtual void OnDestroy()
        {
            if (updateManager == null || !updateManager.Ready) return;

            updateManager.RemoveUpdateListener(this);
            listenerAdded = false;
        }

        private void Update()
        {
            if (!listenerAdded)
            {
                if (updateManager == null || !updateManager.Ready) return;

                updateManager.AddUpdateListener(this);
                listenerAdded = true;
            }
        }

        public virtual void OnUpdate(float deltaTime)
        {

        }
    }
}
