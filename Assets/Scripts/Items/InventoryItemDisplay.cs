using TMPro;
using UnityEngine;

namespace Items
{
    public class InventoryItemDisplay : MonoBehaviour
    {
        private TextMeshProUGUI _text;

#pragma warning disable 0649   // Backing fields are assigned through the Inspector
        [SerializeField] private InventoryItem item;
#pragma warning restore 0649

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            _text.text = item.amount.ToString();
        }
    }
}