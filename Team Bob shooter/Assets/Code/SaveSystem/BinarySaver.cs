using System;
using System.IO;
using UnityEngine;

namespace TeamBobFPS.Save
{
	public class BinarySaver : ISaveSystem
	{
		private BinaryReader reader;
		private BinaryWriter writer;
		private FileStream currentSaveFile;

		public void FinalizeRead()
		{
			reader.Close();
			currentSaveFile.Close();

			reader = null;
			currentSaveFile = null;
		}

		public void FinalizeWrite()
		{
			writer.Close();
			currentSaveFile.Close();

			writer = null;
			currentSaveFile = null;
		}

		public bool PrepareRead(string savePath)
		{
			if (!File.Exists(savePath))
			{
				// The save file does not exist!
				return false;
			}

			try
			{
				currentSaveFile = File.OpenRead(savePath);
				reader = new BinaryReader(currentSaveFile);
			}
			catch (Exception error)
			{
				Debug.LogException(error);
				return false;
			}

			return true;
		}

		public bool PrepareWrite(string savePath)
		{
			try
			{
				string saveDirectory = Path.GetDirectoryName(savePath);
				if (!Directory.Exists(saveDirectory))
				{
					// Save directory does not exist, let's create it.
					Directory.CreateDirectory(saveDirectory);
				}

				currentSaveFile = File.Open(savePath, FileMode.Create);
				writer = new BinaryWriter(currentSaveFile);
			}
			catch(Exception error)
			{
				Debug.LogException(error);
				return false;
			}

			return true;
		}

		public bool ReadBool()
		{
			return reader.ReadBoolean();
		}

		public float ReadFloat()
		{
			return reader.ReadSingle();
		}

		public int ReadInt()
		{
			return reader.ReadInt32();
		}

        

        public string ReadString()
		{
			return reader.ReadString();
		}

		public Vector2 ReadVector2()
		{
			float x = reader.ReadSingle();
			float y = reader.ReadSingle();
			return new Vector2(x, y);
		}

		public Vector3 ReadVector3()
		{
			float x = reader.ReadSingle();
			float y = reader.ReadSingle();
			float z = reader.ReadSingle();
			return new Vector3(x, y, z);
		}
		
		public Quaternion ReadQuaternion()
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float w = reader.ReadSingle();
            return new Quaternion(x, y, z, w);
        }

		public void WriteBool(bool value)
		{
			writer.Write(value);
		}

		public void WriteFloat(float value)
		{
			writer.Write(value);
		}

		public void WriteInt(int value)
		{
			writer.Write(value);
		}


        public void WriteString(string value)
		{
			writer.Write(value);
		}

		public void WriteVector2(Vector2 value)
		{
			writer.Write(value.x);
			writer.Write(value.y);
		}

		public void WriteVector3(Vector3 value)
		{
			writer.Write(value.x);
			writer.Write(value.y);
			writer.Write(value.z);
		}
        public void WriteQuaternion(Quaternion value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }
	}
}