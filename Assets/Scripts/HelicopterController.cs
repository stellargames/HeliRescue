using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 30f;
    [SerializeField] private float rotateSpeed = 3f;
    [SerializeField] private float liftForce = 30f;

    private Rigidbody2D _rigidBody2D;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var rotation = Input.GetAxis("Rotation");

        var velocity = _rigidBody2D.velocity;
        var force = new Vector2(horizontal * moveSpeed, vertical * liftForce);
        _rigidBody2D.AddForce(force - velocity);

        var yRotation = transform.rotation.eulerAngles.y + rotateSpeed * rotation;
        var direction = transform.InverseTransformDirection(velocity);
        transform.rotation =
            Quaternion.Euler(new Vector3(direction.z, yRotation, -direction.x));
    }
}
