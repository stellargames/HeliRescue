using System;
using UnityEngine;

namespace Items
{
    [Serializable]
    [CreateAssetMenu(menuName = "Items/Missile", fileName = "MissileName.asset")]
    public class Missile : InventoryItem
    {
        public MissileController prefab;
        public float thrust = 20f;
    }
}