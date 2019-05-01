using System;
using Interfaces;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HelicopterController : MonoBehaviour, IHaveThrottle
{
//    private Inventory _inventory;
    private float _missileFireDelayTimer;
    private Rigidbody2D _rigidBody2D;
    private float _rotateForce;
    private Vector2 _throttleForce;
    [SerializeField] private Inventory inventory;

    [SerializeField] private float liftForce = 1000f;
    [SerializeField] private float missileFireDelay = 0.2f;
    [SerializeField] private Missile missilePrefab;
    [SerializeField] private float moveSpeed = 1000f;
    [SerializeField] private float rotateSpeed = 4f;

    public float Throttle => Math.Abs(_throttleForce.x) + _throttleForce.y;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
//        _inventory = GetComponentInParent<Inventory>();
    }

    private void Update()
    {
        GetThrottleInput();

        if (Input.GetButtonDown("Fire1")) TryFireMissile();

        _missileFireDelayTimer += Time.deltaTime;
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

    private void TryFireMissile()
    {
        if (_missileFireDelayTimer < missileFireDelay) return;
//        if (_inventory.TakeMissiles(1) != 1) return;
        if (inventory.TakeMissiles(1) != 1) return;

        var direction = Vector3ToVector2(transform.right);
        var missileInstance =
            missilePrefab.Get<Missile>(transform.position, Quaternion.identity);
        missileInstance.Launch(direction, _rigidBody2D.velocity);
        _missileFireDelayTimer = 0;
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

    private static Vector2 Vector3ToVector2(Vector3 vector)
    {
        return new Vector2(vector.x, vector.y + vector.z);
    }
}
