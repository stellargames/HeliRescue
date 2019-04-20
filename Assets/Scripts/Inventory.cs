using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int maxMissiles = 100;
    [SerializeField] private int missiles = 100;

    public static event Action<int> MissileAmountChanged = delegate { };

    private void Awake()
    {
        AdjustMissileAmount(0);
    }

    public int TakeMissiles(int amount)
    {
        if (amount <= 0) return 0;
        if (amount > missiles) amount = missiles;

        AdjustMissileAmount(-amount);
        return amount;
    }

    public int AddMissiles(int amount)
    {
        if (amount <= 0) return 0;
        var freeSpace = maxMissiles - missiles;
        if (amount > freeSpace) amount = freeSpace;

        AdjustMissileAmount(amount);
        return amount;
    }

    private void AdjustMissileAmount(int amount)
    {
        missiles += amount;
        MissileAmountChanged.Invoke(missiles);
    }
}