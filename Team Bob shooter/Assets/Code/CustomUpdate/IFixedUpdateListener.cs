namespace TeamBobFPS
{
	public interface IFixedUpdateListener : IUpdateListenerBase
	{
		void OnFixedUpdate(float deltaTime);
	}
}