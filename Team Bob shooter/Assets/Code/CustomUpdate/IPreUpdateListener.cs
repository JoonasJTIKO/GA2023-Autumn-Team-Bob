namespace TeamBobFPS
{
	public interface IPreUpdateListener : IUpdateListenerBase
	{
		void OnPreUpdate(float deltaTime);
	}
}