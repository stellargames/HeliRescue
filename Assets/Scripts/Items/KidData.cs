using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Items
{
    [Serializable]
    [CreateAssetMenu(menuName = "Items/Kids", fileName = "KidName.asset")]
    public class KidData : InventoryItem
    {
        [SerializeField] private AudioClip[] audioClips;

        public AudioClip GetAudioClip()
        {
            return audioClips[Random.Range(0, audioClips.Length)];
        }
    }
}
