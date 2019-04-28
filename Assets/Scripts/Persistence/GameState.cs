using System;
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

            var lists = Resources.FindObjectsOfTypeAll<PersistableItemList>();
            foreach (var itemList in lists)
            {
                itemList.Load(saveFile);
            }

            saveFile.Close();
        }

        public void Save()
        {
            var saveFile = new SaveFile(_filename);

            SavePlayer(saveFile);

            var lists = Resources.FindObjectsOfTypeAll<PersistableItemList>();
            foreach (var itemList in lists)
            {
                itemList.Save(saveFile);
            }

            saveFile.Close();
        }

        private void LoadPlayer(SaveFile saveFile)
        {
            _player.Position = saveFile.Get("playerData", _player.Position);
        }

        private void SavePlayer(SaveFile saveFile)
        {
            saveFile.Set("playerData", _player.Position);
        }
    }
}