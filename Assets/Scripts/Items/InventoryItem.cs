using System;
using Skytanet.SimpleDatabase;
using UnityEngine;

namespace Items
{
    [Serializable]
    public abstract class InventoryItem : ScriptableObject
    {
        public int amount;
        public int maximumAmount;
        [SerializeField] private int startAmount;

        public void Load(SaveFile file)
        {
            amount = file.Get("inventory." + name, startAmount);
        }

        public void Save(SaveFile file)
        {
            file.Set("inventory." + name, amount);
        }
    }
}
