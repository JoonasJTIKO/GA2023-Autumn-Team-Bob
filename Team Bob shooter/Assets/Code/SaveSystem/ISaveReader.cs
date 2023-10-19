using UnityEngine;

namespace TeamBobFPS.Save
{
	public interface ISaveReader
	{
		bool PrepareRead(string savePath);
		void FinalizeRead();

		int ReadInt();
		float ReadFloat();
		string ReadString();
		bool ReadBool();
		Vector2 ReadVector2();
		Vector3 ReadVector3();
		Quaternion ReadQuaternion();
	}
}