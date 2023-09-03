using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TeamBobFPS
{
    public abstract class UpdateRunnerBase<T>
        where T : IUpdateListenerBase
    {
        public class PriorityComparer : Comparer<T>
        {
            public override int Compare(T x, T y)
            {
                // TODO: Implement null checks!

                return x.ID - y.ID;
            }
        }

        private SortedSet<T> listeners = null;
        
        private Queue<T> addedListeners = new Queue<T>();
        
        protected Queue<T> removedListeners = new Queue<T>();

        protected SortedSet<T> Listeners
        {
            get
            {
                if (listeners == null)
                {
                    listeners = new SortedSet<T>(new PriorityComparer());
                }

                return listeners;
            }
        }

        public void RemoveAllListeners()
        {
            foreach (var listener in Listeners)
            {
                removedListeners.Enqueue(listener);
            }
        }

        public void AddUpdateListener(T listener)
        {
            addedListeners.Enqueue(listener);
        }

        public void RemoveUpdateListener(T listener)
        {
            removedListeners.Enqueue(listener);
        }

        public void Run(float deltaTime)
        {
            // Remove obsolete listeners
            while (removedListeners.Count > 0)
            {
                T removed = removedListeners.Dequeue();
                Listeners.Remove(removed);
            }

            // Add new listeners
            while (addedListeners.Count > 0)
            {
                T added = addedListeners.Dequeue();
                Listeners.Add(added);
            }
            RunUpdates(deltaTime);
        }

        protected abstract void RunUpdates(float deltaTime);
    }
}