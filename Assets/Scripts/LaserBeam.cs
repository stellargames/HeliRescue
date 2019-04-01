using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField] private GameObject beam = null;
    [SerializeField] private GameObject turret;

    private void Start()
    {
        beam.SetActive(false);
    }

    public void SetBeamStatus(bool status)
    {
        beam.SetActive(status);
    }


}
