using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField] private GameObject beam = null;

    private void Start()
    {
        beam.SetActive(false);
    }

    public void SetBeamStatus(bool status)
    {
        beam.SetActive(status);
    }

}
