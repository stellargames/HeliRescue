﻿using System.IO;
using UnityEngine;
using static UnityEngine.Object;

namespace Persistence
{
    public class GameData
    {
        private const int Version = 1;
        private readonly string _saveFilePath;

        public GameData()
        {
            CheckpointManager = FindObjectOfType<CheckpointManager>();
            PlayerSpawnPoint = FindObjectOfType<PlayerSpawnPoint>();
            _saveFilePath = Path.Combine(Application.persistentDataPath, "saveFile");
        }

        public PlayerSpawnPoint PlayerSpawnPoint { get; }
        public CheckpointManager CheckpointManager { get; }

        public void Load()
        {
            if (!File.Exists(_saveFilePath)) return;
            Debug.Log("GameData Load");
            using (var binaryReader = new BinaryReader(File.OpenRead(_saveFilePath)))
            {
                var reader = new GameDataReader(binaryReader, -binaryReader.ReadInt32());
                CheckpointManager.Load(reader);
                PlayerSpawnPoint.Load(reader);
            }
        }

        public void Save()
        {
            Debug.Log("GameData Save");
            using (var binaryWriter = new BinaryWriter(File.OpenWrite(_saveFilePath)))
            {
                var writer = new GameDataWriter(binaryWriter);
                writer.Write(-Version);
                CheckpointManager.Save(writer);
                PlayerSpawnPoint.Save(writer);
            }
        }
    }
}