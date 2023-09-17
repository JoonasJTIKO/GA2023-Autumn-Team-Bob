using System;

namespace TeamBobFPS
{
    public class PreUpdateRunner : UpdateRunnerBase<IPreUpdateListener>
    {
        protected override void RunUpdates(float deltaTime)
        {
            foreach (IPreUpdateListener listener in Listeners)
            {
                if (removedListeners.Contains(listener))
                {
                    continue;
                }

                listener.OnPreUpdate(deltaTime);
            }
        }
    }
}