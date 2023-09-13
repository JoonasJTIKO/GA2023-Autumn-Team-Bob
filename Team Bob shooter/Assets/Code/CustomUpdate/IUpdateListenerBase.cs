using UnityEngine;

namespace TeamBobFPS
{
	public interface IUpdateListenerBase
	{
		/// <summary>
		/// Unique ID
		/// </summary>
		int ID { get; }
	}
}