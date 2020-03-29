using Skytanet.SimpleDatabase;
using UnityEngine;

namespace Persistence
{
    [DisallowMultipleComponent]
    public class PersistenceComponent : GuidComponent
    {
        public void Load(SaveFile file)
        {
            foreach (IPersist persistableComponent in GetComponents<IPersist>())
                persistableComponent.Load(file);
        }

        public void Save(SaveFile file)
        {
            foreach (IPersist persistableComponent in GetComponents<IPersist>())
                persistableComponent.Save(file);
        }
    }
}