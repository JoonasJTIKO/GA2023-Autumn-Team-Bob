using UnityEngine;

namespace TeamBobFPS
{
	public class UpdateRunner : UpdateRunnerBase<IUpdateListener>
	{
		protected override void RunUpdates(float deltaTime)
		{
			foreach (IUpdateListener listener in Listeners)
			{
				if (removedListeners.Contains(listener))
				{
					continue;
				}
				listener.OnUpdate(deltaTime);
			}
		}
	}
}