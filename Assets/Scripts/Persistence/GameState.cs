using System;
using Items;
using Skytanet.SimpleDatabase;
using UnityEngine;

namespace Persistence
{
    [Serializable]
    public class GameState
    {
        private readonly string _filename;
        private readonly Player _player;

        public GameState(Player player)
        {
            _filename = "game";
            _player = player;
        }

        public void Load()
        {
            var saveFile = new SaveFile(_filename);

            LoadPlayer(saveFile);
            LoadInventory(saveFile);
            LoadPersistanceComponents(saveFile);

            saveFile.Close();
        }

        public void Save()
        {
            var saveFile = new SaveFile(_filename);

            SavePlayer(saveFile);
            SaveInventory(saveFile);
            SavePersistanceComponents(saveFile);

            saveFile.Close();
        }

        private static void SavePersistanceComponents(SaveFile saveFile)
        {
            var persistenceComponents =
                Resources.FindObjectsOfTypeAll<PersistenceComponent>();
            foreach (var item in persistenceComponents) item.Save(saveFile);
        }

        private static void LoadPersistanceComponents(SaveFile saveFile)
        {
            var persistenceComponents =
                Resources.FindObjectsOfTypeAll<PersistenceComponent>();
            foreach (var item in persistenceComponents) item.Load(saveFile);
        }

        private static void LoadInventory(SaveFile saveFile)
        {
            var inventoryItems = Resources.FindObjectsOfTypeAll<InventoryItem>();
            foreach (var item in inventoryItems) item.Load(saveFile);
        }

        private static void SaveInventory(SaveFile saveFile)
        {
            var inventoryItems = Resources.FindObjectsOfTypeAll<InventoryItem>();
            foreach (var item in inventoryItems) item.Save(saveFile);
        }

        private void LoadPlayer(SaveFile saveFile)
        {
            _player.Load(saveFile);
        }

        private void SavePlayer(SaveFile saveFile)
        {
            _player.Save(saveFile);
        }
    }
}
