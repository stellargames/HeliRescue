using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Persistence
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class GuidComponent : MonoBehaviour, ISerializationCallbackReceiver
    {
        private Guid _guid = Guid.Empty;
        [SerializeField] private byte[] serializedGuid;

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabAsset(this))
            {
                serializedGuid = new byte[0];
                _guid = Guid.Empty;
            }
            else
#endif
            {
                if (_guid != Guid.Empty) serializedGuid = _guid.ToByteArray();
            }
        }

        public void OnAfterDeserialize()
        {
            if (serializedGuid != null && serializedGuid.Length == 16) _guid = new Guid(serializedGuid);
        }

        private void Awake()
        {
            CreateGuid();
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabAsset(this))
            {
                serializedGuid = null;
                _guid = Guid.Empty;
            }
            else
#endif
            {
                CreateGuid();
            }
        }

        public Guid GetGuid()
        {
            if (_guid == Guid.Empty && serializedGuid != null && serializedGuid.Length == 16)
                _guid = new Guid(serializedGuid);

            return _guid;
        }

        private void OnDestroy()
        {
            GuidManager.Remove(_guid);
        }

        private void CreateGuid()
        {
            if (serializedGuid == null || serializedGuid.Length != 16)
            {
                _guid = Guid.NewGuid();

#if UNITY_EDITOR
                if (PrefabUtility.IsPartOfNonAssetPrefabInstance(this))
                    PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif
            }
            else if (_guid == Guid.Empty)
            {
                _guid = new Guid(serializedGuid);
            }

            var instanceId = GetInstanceID();
            while (!GuidManager.Add(_guid, instanceId)) _guid = Guid.NewGuid();

            serializedGuid = _guid.ToByteArray();
        }
    }
}