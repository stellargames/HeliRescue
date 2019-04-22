using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Persistence
{
    public class GameDataWriter
    {
        private readonly IFormatter _formatter;
        private readonly BinaryWriter _writer;

        public GameDataWriter(BinaryWriter writer)
        {
            _writer = writer;
            _formatter = new BinaryFormatter();
        }

        public void Write(float value)
        {
            _writer.Write(value);
        }

        public void Write(Color value)
        {
            _writer.Write(value.r);
            _writer.Write(value.g);
            _writer.Write(value.b);
            _writer.Write(value.a);
        }

        public void Write(int value)
        {
            _writer.Write(value);
        }

        public void Write(Guid value)
        {
            _writer.Write(value.ToByteArray());
        }

        public void Write(Quaternion value)
        {
            _writer.Write(value.x);
            _writer.Write(value.y);
            _writer.Write(value.z);
            _writer.Write(value.w);
        }

        public void Write(Vector3 value)
        {
            _writer.Write(value.x);
            _writer.Write(value.y);
            _writer.Write(value.z);
        }

        public void Write(Random.State value)
        {
            _writer.Write(JsonUtility.ToJson(value));
        }


        public void Write(bool value)
        {
            _writer.Write(value);
        }

        public void WriteDictionary<T>(Dictionary<Guid, T> dictionary)
        {
            _formatter.Serialize(_writer.BaseStream, dictionary);
        }
    }
}