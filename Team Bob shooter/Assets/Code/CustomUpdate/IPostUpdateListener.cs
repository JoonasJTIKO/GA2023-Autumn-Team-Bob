namespace TeamBobFPS
{
	public interface IPostUpdateListener : IUpdateListenerBase
	{
		void OnPostUpdate(float deltaTime);
	}
}