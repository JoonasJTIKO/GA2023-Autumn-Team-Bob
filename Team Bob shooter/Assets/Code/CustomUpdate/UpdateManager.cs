using System;
using UnityEngine;

namespace TeamBobFPS
{
	public class UpdateManager : MonoBehaviour
	{
		public float timeScale = 1.0f;
		public float fixedTimeScale = 1.0f;

		private float timeSinceUpdate = 0, timeSinceFixedUpdate = 0;

		private int latestGivenID = 0;

		private UpdateRunner updateRunner;
		private PreUpdateRunner preUpdateRunner;
		private PostUpdateRunner postUpdateRunner;
		private FixedUpdateRunner fixedUpdateRunner;

		private void Awake()
		{
			updateRunner = new UpdateRunner();
			preUpdateRunner = new PreUpdateRunner();
			postUpdateRunner = new PostUpdateRunner();
			fixedUpdateRunner = new FixedUpdateRunner();
		}

		private void Update()
		{
			timeSinceUpdate += Time.deltaTime * timeScale;

			if (timeSinceUpdate >= Time.deltaTime && timeScale != 0)
			{
				preUpdateRunner.Run(Time.deltaTime * timeScale);
				updateRunner.Run(Time.deltaTime * timeScale);
				postUpdateRunner.Run(Time.deltaTime * timeScale);
                timeSinceUpdate = 0;
            }
		}

		private void FixedUpdate()
		{
			timeSinceFixedUpdate += Time.fixedDeltaTime * fixedTimeScale;

			if (timeSinceFixedUpdate >= Time.fixedDeltaTime && timeScale != 0)
			{
				fixedUpdateRunner.Run(Time.fixedDeltaTime * fixedTimeScale);
				timeSinceFixedUpdate = 0;
			}
		}

		public int GetUniqueID()
		{
			latestGivenID += 1;
			return latestGivenID;
		}

		public void ClearListeners()
		{
			preUpdateRunner.RemoveAllListeners();
			updateRunner.RemoveAllListeners();
			postUpdateRunner.RemoveAllListeners();
			fixedUpdateRunner.RemoveAllListeners();
		}

		public void AddPreUpdateListener(IPreUpdateListener listener)
		{
			preUpdateRunner.AddUpdateListener(listener);
		}
		
		public void AddUpdateListener(IUpdateListener listener)
		{
			updateRunner.AddUpdateListener(listener);
		}
		
		public void AddPostUpdateListener(IPostUpdateListener listener)
		{
			postUpdateRunner.AddUpdateListener(listener);
		}
		
		public void AddFixedUpdateListener(IFixedUpdateListener listener)
		{
			fixedUpdateRunner.AddUpdateListener(listener);
		}

		public void RemovePreUpdateListener(IPreUpdateListener listener)
		{
			preUpdateRunner.RemoveUpdateListener(listener);
		}
		
		public void RemoveUpdateListener(IUpdateListener listener)
		{
			updateRunner.RemoveUpdateListener(listener);
		}
		
		public void RemovePostUpdateListener(IPostUpdateListener listener)
		{
			postUpdateRunner.RemoveUpdateListener(listener);
		}
		
		public void RemoveFixedUpdateListener(IFixedUpdateListener listener)
		{
			fixedUpdateRunner.RemoveUpdateListener(listener);
		}
	}
}