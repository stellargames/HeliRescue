using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEngine.Object;

namespace Persistence
{
    public class GameData
    {
        private const int Version = 2;
        private readonly string _saveFilePath;

        public GameData()
        {
            Player = FindObjectOfType<Player>();
            PersistentCollections = FindObjectsOfType<PersistentCollection>()
                .OrderBy(collection => collection.name).ToArray();

            _saveFilePath = Path.Combine(Application.persistentDataPath, "saveFile");
        }

        private PersistentCollection[] PersistentCollections { get; }
        public Player Player { get; }

        public void Load()
        {
            if (!File.Exists(_saveFilePath)) return;
            Debug.Log("GameData Load");
            using (var binaryReader = new BinaryReader(File.OpenRead(_saveFilePath)))
            {
                var reader = new GameDataReader(binaryReader, -binaryReader.ReadInt32());
                if (reader.Version == Version)
                {
                    Player.Load(reader);
                    var inventory = FindObjectOfType<Inventory>();
                    inventory.Load(reader);
                    foreach (var persistentCollection in PersistentCollections) persistentCollection.Load(reader);
                }
                else
                {
                    Debug.LogWarningFormat("Game version {0} cannot read save file version {1}", Version,
                        reader.Version);
                }
            }
        }

        public void Save()
        {
            Debug.Log("GameData Save");
            using (var binaryWriter = new BinaryWriter(File.OpenWrite(_saveFilePath)))
            {
                var writer = new GameDataWriter(binaryWriter);
                writer.Write(-Version);
                Player.Save(writer);
                var inventory = FindObjectOfType<Inventory>();
                inventory.Save(writer);

                foreach (var persistentCollection in PersistentCollections) persistentCollection.Save(writer);
            }
        }
    }
}