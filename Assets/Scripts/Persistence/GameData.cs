using System.IO;
using UnityEngine;
using static UnityEngine.Object;

namespace Persistence
{
    public class GameData
    {
        private string _saveFilePath;
        private const int Version = 1;

        public PlayerSpawnPoint PlayerSpawnPoint { get; }
        public CheckpointManager CheckpointManager { get; }

        public GameData()
        {
            CheckpointManager = FindObjectOfType<CheckpointManager>();
            PlayerSpawnPoint = FindObjectOfType<PlayerSpawnPoint>();
            _saveFilePath = Path.Combine(Application.persistentDataPath, "saveFile");
        }

        public void Load()
        {
            if (!File.Exists(_saveFilePath))
            {
                return;
            }

            using (var binaryReader = new BinaryReader(File.OpenRead(_saveFilePath)))
            {
                var reader = new GameDataReader(binaryReader, -binaryReader.ReadInt32());
                CheckpointManager.Load(reader);
                PlayerSpawnPoint.Load(reader);
            }
        }

        public void Save()
        {
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
