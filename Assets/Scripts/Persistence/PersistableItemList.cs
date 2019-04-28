using Skytanet.SimpleDatabase;
using UnityEngine;

namespace Persistence
{
    public class PersistableItemList : MonoBehaviour
    {
        private PersistenceComponent[] _items;

        private void Awake()
        {
            _items = GetComponentsInChildren<PersistenceComponent>();
        }

        public void Load(SaveFile file)
        {
            foreach (var item in _items)
            {
                item.Load(file);
            }
        }

        public void Save(SaveFile file)
        {
            foreach (var item in _items)
            {
                item.Save(file);
            }
        }
    }
}