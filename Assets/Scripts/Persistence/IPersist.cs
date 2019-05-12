using Skytanet.SimpleDatabase;

namespace Persistence
{
    public interface IPersist
    {
        void Load(SaveFile file);
        void Save(SaveFile file);
    }
}
