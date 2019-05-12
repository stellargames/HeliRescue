using System;
using System.Collections.Generic;
using Items;
using Skytanet.SimpleDatabase;
using UnityEngine;
using static UnityEngine.Object;

namespace Persistence
{
    [Serializable]
    public class GameState
    {
        private readonly string _filename;
        private readonly InventoryItem[] _inventoryItems;

        private readonly Dictionary<Guid, PersistenceComponent> _persistenceComponents;

        private readonly Player _player;

        public GameState(Player player)
        {
            _filename = "game";
            _player = player;
            _persistenceComponents = GetPersistenceComponents();
            _inventoryItems = Resources.FindObjectsOfTypeAll<InventoryItem>();
        }

        private static Dictionary<Guid, PersistenceComponent> GetPersistenceComponents()
        {
            var persistenceComponents = new Dictionary<Guid, PersistenceComponent>();
            var components = Resources.FindObjectsOfTypeAll<PersistenceComponent>();
            foreach (var component in components)
                persistenceComponents[component.GetGuid()] = component;

            return persistenceComponents;
        }

        public void Load()
        {
            var saveFile = new SaveFile(_filename);

            LoadPlayer(saveFile);
            LoadInventory(saveFile);
            LoadPersistenceComponents(saveFile);

            saveFile.Close();
        }

        public void Save()
        {
            var saveFile = new SaveFile(_filename);

            SavePlayer(saveFile);
            SaveInventory(saveFile);
            SavePersistenceComponents(saveFile);

            saveFile.Close();
        }

        private void SavePersistenceComponents(SaveFile saveFile)
        {
            var destroyedItems = new List<Guid>();
            foreach (var item in _persistenceComponents)
                if (item.Value == null)
                    destroyedItems.Add(item.Key);
                else
                    item.Value.Save(saveFile);

            saveFile.Set("GameState.destroyedItems", destroyedItems);
        }

        private void LoadPersistenceComponents(SaveFile saveFile)
        {
            var destroyedItems =
                saveFile.Get("GameState.destroyedItems", new List<Guid>());
            foreach (var item in _persistenceComponents)
                if (destroyedItems.Contains(item.Key))
                    Destroy(item.Value.gameObject);
                else
                    item.Value.Load(saveFile);
        }

        private void LoadInventory(SaveFile saveFile)
        {
            foreach (var item in _inventoryItems) item.Load(saveFile);
        }

        private void SaveInventory(SaveFile saveFile)
        {
            foreach (var item in _inventoryItems) item.Save(saveFile);
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