using System;

namespace TeamBobFPS
{
    public class PostUpdateRunner : UpdateRunnerBase<IPostUpdateListener>
    {
        protected override void RunUpdates(float deltaTime)
        {
            foreach (IPostUpdateListener listener in Listeners)
            {
                if (removedListeners.Contains(listener))
                {
                    continue;
                }

                listener.OnPostUpdate(deltaTime);
            }
        }
    }
}