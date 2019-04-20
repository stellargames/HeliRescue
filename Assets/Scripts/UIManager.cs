using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI missileCount;

    private void OnEnable()
    {
        Inventory.MissileAmountChanged += InventoryOnMissileAmountChanged;
    }

    private void OnDisable()
    {
        Inventory.MissileAmountChanged -= InventoryOnMissileAmountChanged;
    }

    private void InventoryOnMissileAmountChanged(int amount)
    {
        missileCount.text = $"{amount:D3}";
    }
}