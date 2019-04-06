using UnityEngine;

public class LaserSequence : MonoBehaviour
{
    [SerializeField] private LaserBeam[] laserBeams = null;
    [SerializeField] private float intervalSeconds = 3f;
    [SerializeField] private int modulo = 2;

    private float _currentTime;
    private int _currentIndex;

    private void Start()
    {
        // enable and disable the appropriate beams
        SwitchBeams();
    }

    private void SwitchBeams()
    {
        for (var i = 0; i < laserBeams.Length; i++)
        {
            laserBeams[i].SetBeamStatus((i + _currentIndex) % modulo == 0);
        }
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime >= intervalSeconds)
        {
            // on interval go to next set
            _currentTime = 0f;
            _currentIndex = (_currentIndex + 1) % laserBeams.Length;
            SwitchBeams();
        }
    }
}
