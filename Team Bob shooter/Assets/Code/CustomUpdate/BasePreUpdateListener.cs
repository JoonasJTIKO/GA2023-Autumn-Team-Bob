using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class BasePreUpdateListener : MonoBehaviour, IPreUpdateListener
    {
        protected UpdateManager updateManager;

        protected int id;

        public int ID { get { return id; } }

        protected virtual void Awake()
        {
            if (GameInstance.Instance == null) return;

            updateManager = GameInstance.Instance.GetUpdateManager();

            id = updateManager.GetUniqueID();
        }

        protected virtual void OnEnable()
        {
            if (updateManager == null) return;

            updateManager.AddPreUpdateListener(this);
        }

        protected virtual void OnDestroy()
        {
            if (updateManager == null) return;

            updateManager.RemovePreUpdateListener(this);
        }

        protected virtual void OnDisable()
        {
            if (updateManager == null) return;

            updateManager.RemovePreUpdateListener(this);
        }

        public virtual void OnPreUpdate(float deltaTime)
        {

        }
    }
}
