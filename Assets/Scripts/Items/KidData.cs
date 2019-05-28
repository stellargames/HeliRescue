using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Items
{
    [Serializable]
    [CreateAssetMenu(menuName = "Items/Kids", fileName = "KidName.asset")]
    public class KidData : InventoryItem
    {
#pragma warning disable 0649   // Backing fields are assigned through the Inspector
        [SerializeField] private AudioClip[] audioClips;
#pragma warning restore 0649

        public AudioClip GetAudioClip()
        {
            return audioClips[Random.Range(0, audioClips.Length)];
        }
    }
}