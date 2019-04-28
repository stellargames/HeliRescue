using System;
using Skytanet.SimpleDatabase;

namespace Persistence
{
    public interface IPersist
    {
        Guid Guid { get; }
        void Load(SaveFile file);
        void Save(SaveFile file);
    }
}