using System;
using System.Collections.Generic;

// Class to handle registering and accessing objects by GUID
namespace Persistence
{
    public class GuidManager
    {
        private static GuidManager _instance;
        private readonly Dictionary<Guid, int> _guidToInstanceIdMap;

        private GuidManager()
        {
            _guidToInstanceIdMap = new Dictionary<Guid, int>();
        }

        public static bool Add(Guid guid, int instanceId)
        {
            if (_instance == null) _instance = new GuidManager();

            return _instance.InternalAdd(guid, instanceId);
        }

        public static void Remove(Guid guid)
        {
            if (_instance == null) _instance = new GuidManager();

            _instance.InternalRemove(guid);
        }

        private bool InternalAdd(Guid guid, int instanceId)
        {
            if (_guidToInstanceIdMap.ContainsKey(guid)) return _guidToInstanceIdMap[guid] == instanceId;

            _guidToInstanceIdMap.Add(guid, instanceId);
            return true;
        }

        private void InternalRemove(Guid guid)
        {
            _guidToInstanceIdMap.Remove(guid);
        }
    }
}