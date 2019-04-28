using System;
using Skytanet.SimpleDatabase;
using UnityEngine;

namespace Persistence
{
    [DisallowMultipleComponent]
    public class PersistenceComponent : GuidComponent
    {
        private IPersist[] _persistableComponents;

        private void Awake()
        {
            _persistableComponents = GetComponents<IPersist>();
        }

        public void Load(SaveFile file)
        {
            foreach (var persistableComponent in _persistableComponents)
            {
                persistableComponent.Load(file);
            }
        }

        public void Save(SaveFile file)
        {
            foreach (var persistableComponent in _persistableComponents)
            {
                if (persistableComponent.Guid != Guid.Empty)
                {
                    persistableComponent.Save(file);
                }
            }
        }
    }
}