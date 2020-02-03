using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class ExplosionFlashDimmer2D : MonoBehaviour
{
    private float _intensity;
    private Light2D _light;
    private float _time;
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private float dimSpeed = 1;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _intensity = _light.intensity;
        _light.intensity = 0;
    }

    private void OnEnable()
    {
        _light.intensity = _intensity;
        _time = 0;
    }

    private void Update()
    {
        if (_light.intensity <= 0) return;

        _time += Time.deltaTime;
        if (_time < delay) return;

        _light.intensity -= Time.deltaTime * dimSpeed;

        if (_light.intensity < 0)
            _light.intensity = 0;
    }
}