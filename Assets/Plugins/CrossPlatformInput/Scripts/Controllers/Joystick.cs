using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler,
    IPointerUpHandler, IPointerClickHandler
{
    public float Horizontal =>
        (snapX) ? SnapFloat(_input.x, AxisOption.OnlyHorizontal) : _input.x;

    public float Vertical =>
        (snapY) ? SnapFloat(_input.y, AxisOption.OnlyVertical) : _input.y;

    public Vector2 Direction => new Vector2(Horizontal, Vertical);
    public string horizontalAxisName = "Horizontal";
    public string verticalAxisName = "Vertical";
    public string fireButtonName = "Fire1";

    [SerializeField] private float handleRange = 1;
    [SerializeField] private float deadZone = 0;
    [SerializeField] private AxisOption axesToUse = AxisOption.Both;
    [SerializeField] private bool snapX = false;
    [SerializeField] private bool snapY = false;
    [SerializeField] private bool canFire = false;
    [SerializeField] private float fireThreshold = 0.05f;
    [SerializeField] protected RectTransform background = null;
    [SerializeField] private RectTransform handle = null;

    private RectTransform _baseRect = null;
    private Canvas _canvas;
    private Camera _camera;
    private Vector2 _input = Vector2.zero;

    private bool _useX;
    private bool _useY;
    private CrossPlatformInputManager.VirtualAxis _horizontalVirtualAxis;
    private CrossPlatformInputManager.VirtualAxis _verticalVirtualAxis;
    private CrossPlatformInputManager.VirtualButton _virtualButton;
    private float _fireTimer;

    private void OnEnable()
    {
        CreateVirtualAxes();
    }

    protected virtual void Start()
    {
        handleRange = Mathf.Abs(handleRange);
        deadZone = Mathf.Abs(deadZone);
        _baseRect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        var center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        _fireTimer = Time.unscaledTime;
        OnDrag(eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        _input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        UpdateVirtualAxes(_input);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canFire && Time.unscaledTime - _fireTimer < fireThreshold)
        {
            _virtualButton.Released();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        _camera = null;
        if (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
            _camera = _canvas.worldCamera;

        var position =
            RectTransformUtility.WorldToScreenPoint(_camera, background.position);
        var radius = background.sizeDelta / 2;
        _input = (eventData.position - position) / (radius * _canvas.scaleFactor);
        FormatInput();
        HandleInput(_input.magnitude, _input.normalized, radius, _camera);
        UpdateVirtualAxes(_input);
        handle.anchoredPosition = _input * radius * handleRange;
    }

    protected virtual void HandleInput(float magnitude, Vector2 normalised,
        Vector2 radius, Camera camera)
    {
        if (magnitude > deadZone)
        {
            if (magnitude > 1)
                _input = normalised;
        }
        else
        {
            _input = Vector2.zero;
        }
    }

    private void UpdateVirtualAxes(Vector3 value)
    {
        if (_useX)
        {
            _horizontalVirtualAxis.Update(value.x);
        }

        if (_useY)
        {
            _verticalVirtualAxis.Update(value.y);
        }
    }

    private void CreateVirtualAxes()
    {
        _useX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
        _useY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

        if (_useX)
        {
            _horizontalVirtualAxis =
                new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(_horizontalVirtualAxis);
        }

        if (_useY)
        {
            _verticalVirtualAxis =
                new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(_verticalVirtualAxis);
        }

        if (canFire)
        {
            _virtualButton = new CrossPlatformInputManager.VirtualButton(fireButtonName);
            CrossPlatformInputManager.RegisterVirtualButton(_virtualButton);
        }
    }

    private void FormatInput()
    {
        if (axesToUse == AxisOption.OnlyHorizontal)
            _input = new Vector2(_input.x, 0f);
        else if (axesToUse == AxisOption.OnlyVertical)
            _input = new Vector2(0f, _input.y);
    }

    private float SnapFloat(float value, AxisOption snapAxis)
    {
        if (Math.Abs(value) < float.Epsilon)
            return value;

        if (axesToUse == AxisOption.Both)
        {
            var angle = Vector2.Angle(_input, Vector2.up);
            if (snapAxis == AxisOption.OnlyHorizontal)
            {
                if (angle < 22.5f || angle > 157.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            else if (snapAxis == AxisOption.OnlyVertical)
            {
                if (angle > 67.5f && angle < 112.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }

            return value;
        }
        else
        {
            if (value > 0)
                return 1;
            if (value < 0)
                return -1;
        }

        return 0;
    }

    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_baseRect,
            screenPosition, _camera, out var localPoint))
        {
            Vector2 sizeDelta;
            var pivotOffset = _baseRect.pivot * (sizeDelta = _baseRect.sizeDelta);
            return localPoint - (background.anchorMax * sizeDelta) + pivotOffset;
        }

        return Vector2.zero;
    }

    private void OnDisable()
    {
        if (_useX)
        {
            _horizontalVirtualAxis.Remove();
        }

        if (_useY)
        {
            _verticalVirtualAxis.Remove();
        }

        if (canFire)
        {
            _virtualButton.Remove();
        }
    }

    private enum AxisOption
    {
        Both,
        OnlyHorizontal,
        OnlyVertical
    }
}