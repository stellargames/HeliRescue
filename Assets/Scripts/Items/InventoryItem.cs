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

#pragma warning disable 0649   // Backing fields are assigned through the Inspector
        [SerializeField] private int startAmount;
#pragma warning restore 0649

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
