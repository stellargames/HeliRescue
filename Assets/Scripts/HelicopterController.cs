using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HelicopterController : MonoBehaviour, IHaveThrottle
{
    [SerializeField] private float moveSpeed = 1000f;
    [SerializeField] private float rotateSpeed = 4f;
    [SerializeField] private float liftForce = 1000f;

    private Rigidbody2D _rigidBody2D;
    private float _rotateForce;
    private Vector2 _appliedForce;

    public float Throttle => Math.Abs(_appliedForce.x) + _appliedForce.y;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var rotation = Input.GetAxis("Rotation");

        _appliedForce = new Vector2(horizontal * moveSpeed * Time.deltaTime,
            vertical * liftForce * Time.deltaTime);
        _rotateForce = rotateSpeed * rotation;
    }

    private void FixedUpdate()
    {
        var velocity = _rigidBody2D.velocity;
        _rigidBody2D.AddForce(_appliedForce - velocity);

        var yRotation = transform.rotation.eulerAngles.y + _rotateForce;
        var direction = transform.InverseTransformDirection(velocity);
        transform.rotation =
            Quaternion.Euler(new Vector3(direction.z, yRotation, -direction.x));
    }
}
