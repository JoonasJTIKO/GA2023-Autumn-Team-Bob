using UnityEngine;

namespace TeamBobFPS.Save
{ 
	public interface ISaveWriter
	{
		bool PrepareWrite(string savePath);
		void FinalizeWrite();

		void WriteInt(int value);
		void WriteFloat(float value);
		void WriteString(string value);
		void WriteBool(bool value);
		void WriteVector2(Vector2 value);
		void WriteVector3(Vector3 value);
		void WriteQuaternion(Quaternion value);
	}
}