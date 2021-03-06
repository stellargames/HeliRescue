﻿using System;
using Items;
using Persistence;
using Skytanet.SimpleDatabase;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(PersistenceComponent))]
public class KidController : MonoBehaviour, IPersist
{
    private static readonly int AnimatorRun = Animator.StringToHash("Run");
    private AudioSource _audioSource;
    private Guid _guid;
    private bool _rescued;

#pragma warning disable 0649   // Backing fields are assigned through the Inspector
    [SerializeField] private Animator animator;
    [SerializeField] private Transform kidBody;
    [SerializeField] private KidData kidData;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
#pragma warning restore 0649

    public void Load(SaveFile file)
    {
        transform.position = file.Get(_guid.ToString(), transform.position);
    }

    public void Save(SaveFile file)
    {
        file.Set(_guid.ToString(), transform.position);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_rescued || !other.CompareTag("Player")) return;

        var helicopter = other.attachedRigidbody.gameObject
            .GetComponent<HelicopterController>();
        var helicopterIsLanded = helicopter.IsLanded;
        animator.SetBool(AnimatorRun, helicopterIsLanded);

        if (helicopterIsLanded)
        {
            var distance = helicopter.transform.position.x - kidBody.position.x;
            if (Mathf.Abs(distance) < 0.5f)
            {
                BeRescued();
            }
            else
            {
                var toTheLeft = helicopter.transform.position.x < kidBody.position.x;
                MoveTowardsHelicopter(toTheLeft);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        animator.SetBool(AnimatorRun, false);
    }

    private void MoveTowardsHelicopter(bool goingLeft)
    {
        var position = kidBody.position;
        spriteRenderer.flipX = goingLeft;
        var speed = goingLeft ? -runSpeed : runSpeed;
        var positionX = position.x + speed * Time.fixedDeltaTime;
        kidBody.position = new Vector2(positionX, position.y);
    }

    private void BeRescued()
    {
        _rescued = true;
        PlayRescueAudio();
        animator.SetBool(AnimatorRun, false);
        Destroy(gameObject, 1f);
        kidData.amount++;
    }

    private void Awake()
    {
        _guid = GetComponent<GuidComponent>().GetGuid();
        _audioSource = kidBody.GetComponent<AudioSource>();
    }

    private void PlayRescueAudio()
    {
        if (!_audioSource) return;
        _audioSource.Stop();

        _audioSource.PlayOneShot(kidData.GetAudioClip());
    }
}
