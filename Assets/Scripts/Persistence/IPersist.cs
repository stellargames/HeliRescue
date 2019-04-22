using System;

namespace Persistence
{
    internal interface IPersist
    {
        Guid GetGuid();
        object Save();
        void Load(object obj);
    }
}