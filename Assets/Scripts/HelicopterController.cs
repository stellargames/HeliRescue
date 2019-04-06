using System;
using Interfaces;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HelicopterController : MonoBehaviour, IHaveThrottle
{
    [SerializeField] private float moveSpeed = 1000f;
    [SerializeField] private float rotateSpeed = 4f;
    [SerializeField] private float liftForce = 1000f;

    private Rigidbody2D _rigidBody2D;
    private float _rotateForce;
    private Vector2 _throttleForce;

    public float Throttle => Math.Abs(_throttleForce.x) + _throttleForce.y;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var rotation = Input.GetAxis("Rotation");

        var horizontalForce = horizontal * moveSpeed * Time.deltaTime;
        var verticalForce = vertical * liftForce * Time.deltaTime;
        _throttleForce = new Vector2(horizontalForce,verticalForce);
        _rotateForce = rotateSpeed * rotation;
    }

    private void FixedUpdate()
    {
        var velocity = _rigidBody2D.velocity;
        _rigidBody2D.AddForce(_throttleForce - velocity);

        var yRotation = transform.rotation.eulerAngles.y + _rotateForce;
        var direction = transform.InverseTransformDirection(velocity);
        var rotateDegrees = new Vector3(direction.z, yRotation, -direction.x);
        transform.rotation = Quaternion.Euler(rotateDegrees);
    }
}
