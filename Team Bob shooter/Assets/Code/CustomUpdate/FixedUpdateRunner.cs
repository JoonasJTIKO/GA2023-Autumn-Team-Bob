namespace TeamBobFPS
{
	public class FixedUpdateRunner : UpdateRunnerBase<IFixedUpdateListener>
	{
		protected override void RunUpdates(float deltaTime)
		{
			foreach (IFixedUpdateListener listener in Listeners)
			{
                if (removedListeners.Contains(listener))
                {
                    continue;
                }
                listener.OnFixedUpdate(deltaTime);
			}
		}
	}
}