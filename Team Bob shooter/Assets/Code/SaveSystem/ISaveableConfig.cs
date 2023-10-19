using System.Collections;
using UnityEngine;

namespace TeamBobFPS.Save
{
	public interface ISaveableConfig
	{
		SaveObjectType SaveType { get; }
		void Save(ISaveWriter writer);
		void Load(ISaveReader reader);
	}
}