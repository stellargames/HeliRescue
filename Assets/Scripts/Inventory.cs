using System;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class Inventory : ScriptableObject
{
    [SerializeField] private int maxMissiles = 100;

    [field: SerializeField] public int MissileAmount { get; private set; } = 100;

    public int TakeMissiles(int amount)
    {
        if (amount <= 0) return 0;
        if (amount > MissileAmount) amount = MissileAmount;

        AdjustMissileAmount(-amount);
        return amount;
    }

    public int AddMissiles(int amount)
    {
        if (amount <= 0) return 0;
        var freeSpace = maxMissiles - MissileAmount;
        if (amount > freeSpace) amount = freeSpace;

        AdjustMissileAmount(amount);
        return amount;
    }

    private void AdjustMissileAmount(int amount)
    {
        MissileAmount += amount;
    }
}