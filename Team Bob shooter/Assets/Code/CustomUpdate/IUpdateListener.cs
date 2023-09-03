namespace TeamBobFPS
{
	public interface IUpdateListener : IUpdateListenerBase
	{
		void OnUpdate(float deltaTime);
	}
}