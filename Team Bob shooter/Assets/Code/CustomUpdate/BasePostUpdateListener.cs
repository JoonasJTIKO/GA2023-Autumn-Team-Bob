using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class BasePostUpdateListener : MonoBehaviour, IPostUpdateListener
    {
        protected UpdateManager updateManager;

        protected int id;

        public int ID { get { return id; } }

        public GameObject SelfReference
        {
            get { return this.gameObject; }
        }

        protected virtual void Awake()
        {
            updateManager = GameInstance.Instance.GetUpdateManager();

            id = updateManager.GetUniqueID();
        }

        protected virtual void OnEnable()
        {
            if (updateManager == null) return;

            updateManager.AddPostUpdateListener(this);
        }

        protected virtual void OnDisable()
        {
            if (updateManager == null) return;

            updateManager.RemovePostUpdateListener(this);
        }

        public virtual void OnPostUpdate(float deltaTime)
        {

        }
    }
}
