using System;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Persistence
{
    public class GameDataReader
    {
        private readonly BinaryReader _reader;

        public GameDataReader(BinaryReader reader, int version)
        {
            Version = version;
            _reader = reader;
        }

        public int Version { get; }

        public float ReadFloat()
        {
            return _reader.ReadSingle();
        }

        public int ReadInt()
        {
            return _reader.ReadInt32();
        }

        public Quaternion ReadQuaternion()
        {
            Quaternion value;
            value.x = _reader.ReadSingle();
            value.y = _reader.ReadSingle();
            value.z = _reader.ReadSingle();
            value.w = _reader.ReadSingle();
            return value;
        }

        public Vector3 ReadVector3()
        {
            Vector3 value;
            value.x = _reader.ReadSingle();
            value.y = _reader.ReadSingle();
            value.z = _reader.ReadSingle();
            return value;
        }

        public Color ReadColor()
        {
            Color value;
            value.r = _reader.ReadSingle();
            value.g = _reader.ReadSingle();
            value.b = _reader.ReadSingle();
            value.a = _reader.ReadSingle();
            return value;
        }

        public Random.State ReadRandomState()
        {
            return JsonUtility.FromJson<Random.State>(_reader.ReadString());
        }

        public bool ReadBool()
        {
            return _reader.ReadBoolean();
        }

        public Guid ReadGuid()
        {
            var bytes = _reader.ReadBytes(16);
            return new Guid(bytes);
        }
    }
}