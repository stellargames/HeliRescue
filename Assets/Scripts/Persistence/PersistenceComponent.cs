using Skytanet.SimpleDatabase;
using UnityEngine;

namespace Persistence
{
    [DisallowMultipleComponent]
    public class PersistenceComponent : GuidComponent
    {
        public void Load(SaveFile file)
        {
            foreach (var persistableComponent in GetComponents<IPersist>())
                persistableComponent.Load(file);
        }

        public void Save(SaveFile file)
        {
            foreach (var persistableComponent in GetComponents<IPersist>())
                persistableComponent.Save(file);
        }
    }
}
