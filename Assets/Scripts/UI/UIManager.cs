using TMPro;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private TextMeshProUGUI missileCount;

        private void Update()
        {
            var missileAmount = inventory.MissileAmount;
            missileCount.text = $"{missileAmount:D3}";
        }
    }
}