using System;
using Interfaces;
using Items;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody2D))]
public class HelicopterController : MonoBehaviour, IHaveThrottle
{
    private float _landingTimer;
    private float _missileFireDelayTimer;
    private Rigidbody2D _rigidBody2D;
    private Vector2 _throttleForce;
    private float _verticalInput;
    private float _horizontalInput;
    private float _rotationInput;

#pragma warning disable 0649   // Backing fields are assigned through the Inspector
    [SerializeField] private float landingDelay = 1f;
    [SerializeField] private float liftForce = 1000f;
    [SerializeField] private Missile missile;
    [SerializeField] private float missileFireDelay = 0.2f;
    [SerializeField] private float moveSpeed = 1000f;
    [SerializeField] private float rotateSpeed = 2f;
#pragma warning restore 0649

    public bool IsLanded => _landingTimer > landingDelay;

    public float Throttle => Math.Abs(_throttleForce.x) + _throttleForce.y;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetThrottleInput();

        if (CrossPlatformInputManager.GetButtonUp("Fire1")) TryFireMissile();

        _missileFireDelayTimer += Time.deltaTime;

        var moving = Math.Abs(_rotationInput) > 0.2f ||
                     Math.Abs(_rigidBody2D.velocity.sqrMagnitude) > 0.2f;
        _landingTimer = moving ? 0 : _landingTimer + Time.deltaTime;
    }

    private void FixedUpdate()
    {
        ApplyThrottleToBody();
    }

    private void GetThrottleInput()
    {
        _horizontalInput = CrossPlatformInputManager.GetAxis("Horizontal");
        _verticalInput = CrossPlatformInputManager.GetAxis("Vertical");
        var rotateJoys = CrossPlatformInputManager.GetAxis("RotateJoys");
        var rotateKeys = CrossPlatformInputManager.GetAxis("RotateKeys");
        _rotationInput = Mathf.Clamp(rotateJoys + rotateKeys, -1f, 1f);
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
        var horizontalForce = _horizontalInput * moveSpeed * Time.fixedDeltaTime;
        var verticalForce = _verticalInput * liftForce * Time.fixedDeltaTime;
        _throttleForce = new Vector2(horizontalForce, verticalForce);

        _rigidBody2D.AddForce(_throttleForce - _rigidBody2D.velocity);

        transform.rotation = CalculateRotation(_rotationInput);
    }

    private Quaternion CalculateRotation(float rotationInput)
    {
        var rotateForce = rotateSpeed * rotationInput;
        var yRotation = transform.rotation.eulerAngles.y + rotateForce;
        var direction = transform.InverseTransformDirection(_rigidBody2D.velocity);
        var rotateDegrees = new Vector3(direction.z, yRotation, -direction.x);
        return Quaternion.Euler(rotateDegrees);
    }

    private static Vector2 Vector3ToVector2(Vector3 vector)
    {
        return new Vector2(vector.x, vector.y + vector.z);
    }
}
