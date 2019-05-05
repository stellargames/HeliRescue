using TMPro;
using UnityEngine;

namespace Items
{
    public class InventoryItemDisplay : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        [SerializeField] private InventoryItem item;

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