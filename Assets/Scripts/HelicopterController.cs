using System;
using Interfaces;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HelicopterController : MonoBehaviour, IHaveThrottle
{
    private Rigidbody2D _rigidBody2D;
    private float _rotateForce;
    private Vector2 _throttleForce;
    [SerializeField] private float liftForce = 1000f;
    [SerializeField] private Missile missilePrefab;
    [SerializeField] private float moveSpeed = 1000f;
    [SerializeField] private float rotateSpeed = 4f;

    public float Throttle => Math.Abs(_throttleForce.x) + _throttleForce.y;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetThrottleInput();

        if (Input.GetButtonDown("Fire1")) FireMissile();
    }

    private void FixedUpdate()
    {
        ApplyThrottleToBody();
    }

    private void GetThrottleInput()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var rotation = Input.GetAxis("Rotation") / 2f;

        var horizontalForce = horizontal * moveSpeed * Time.deltaTime;
        var verticalForce = vertical * liftForce * Time.deltaTime;
        _throttleForce = new Vector2(horizontalForce, verticalForce);
        _rotateForce = rotateSpeed * rotation;
    }

    private void FireMissile()
    {
        var noseDirection = transform.right;
        var direction =
            new Vector3(noseDirection.x, noseDirection.y + noseDirection.z, 0);
        var missileInstance =
            Instantiate(missilePrefab, transform.position, Quaternion.identity);
        missileInstance.transform.right = direction;
        var missileBody = missileInstance.GetComponent<Rigidbody2D>();
        missileBody.AddForce(_rigidBody2D.velocity * 0.5f, ForceMode2D.Impulse);
    }

    private void ApplyThrottleToBody()
    {
        var velocity = _rigidBody2D.velocity;
        _rigidBody2D.AddForce(_throttleForce - velocity);
        var yRotation = transform.rotation.eulerAngles.y + _rotateForce;
        var direction = transform.InverseTransformDirection(velocity);
        var rotateDegrees = new Vector3(direction.z, yRotation, -direction.x);
        transform.rotation = Quaternion.Euler(rotateDegrees);
    }
}
