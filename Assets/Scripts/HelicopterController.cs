using System;
using Interfaces;
using Items;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HelicopterController : MonoBehaviour, IHaveThrottle
{
    private float _landingTimer;
    private float _missileFireDelayTimer;
    private Rigidbody2D _rigidBody2D;
    private float _rotateForce;
    private Vector2 _throttleForce;
    [SerializeField] private float landingDelay = 1f;

    [SerializeField] private float liftForce = 1000f;
    [SerializeField] private Missile missile;
    [SerializeField] private float missileFireDelay = 0.2f;
    [SerializeField] private float moveSpeed = 1000f;
    [SerializeField] private float rotateSpeed = 2f;

    public bool IsLanded => _landingTimer > landingDelay;

    public float Throttle => Math.Abs(_throttleForce.x) + _throttleForce.y;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetThrottleInput();

        if (Input.GetButtonDown("Fire1")) TryFireMissile();

        _missileFireDelayTimer += Time.deltaTime;

        var moving = Math.Abs(_rotateForce) > 0.2f ||
                     Math.Abs(_rigidBody2D.velocity.sqrMagnitude) > 0.2f;
        _landingTimer = moving ? 0 : _landingTimer + Time.deltaTime;
    }

    private void FixedUpdate()
    {
        ApplyThrottleToBody();
    }

    private void GetThrottleInput()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var rotateJoys = Input.GetAxis("RotateJoys");
        var rotateKeys = Input.GetAxis("RotateKeys");
        var rotation = Mathf.Clamp(rotateJoys + rotateKeys, -1f, 1f);

        var horizontalForce = horizontal * moveSpeed * Time.deltaTime;
        var verticalForce = vertical * liftForce * Time.deltaTime;
        _throttleForce = new Vector2(horizontalForce, verticalForce);
        _rotateForce = rotateSpeed * rotation;
    }

    private void TryFireMissile()
    {
        if (_missileFireDelayTimer < missileFireDelay) return;
        if (missile.amount < 1) return;

        var direction = Vector3ToVector2(transform.right);
        var missileInstance =
            missile.prefab.Get<MissileController>(transform.position,
                Quaternion.identity);
        missileInstance.Launch(direction, _rigidBody2D.velocity, missile);
        _missileFireDelayTimer = 0;
        missile.amount--;
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
