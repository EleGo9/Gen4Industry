using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneScript : MonoBehaviour
{
    public float UVScaleMin, UVScaleMax;
    public float SineFreqMax;
    public float DistortNoisePeriodMin, DistortNoisePeriodMax;
    public float DistortAmplitudeMax;

    public float SineAmplitudeMax;

    public float DisplacementMultMin;
    public float DisplacementMultMax;

    public MeshRenderer mr;

    public SpawnPositionScript sps;
    public float heightOffset = 0;

    public float maxRotAngle = 60f;

    public float maxDistance;

    public bool useDiamter = true;
    public float diameterMult = 0.0001f;

    int offset = 0;
    float initialScale;

    private void Start()
    {
        if (mr == null)
        {
            mr = GetComponent<MeshRenderer>();
        }
        initialScale = transform.localScale.x;
    }

    void Update()
    {
        mr.material.SetFloat("_UVScale", Random.Range(UVScaleMin,UVScaleMax));
        mr.material.SetFloat("_SineFrequency", Random.Range(0, SineFreqMax));
        mr.material.SetFloat("_SineRotation", Random.Range(-360 * Mathf.Deg2Rad, 360 * Mathf.Deg2Rad));
        mr.material.SetFloat("_DistortNoisePeriod", Random.Range(DistortNoisePeriodMin, DistortNoisePeriodMax));
        mr.material.SetFloat("_DistortAmplitude", Random.Range(0.3f, DistortAmplitudeMax));
        mr.material.SetFloat("_SineAmplitude", Random.Range(0.3f, SineAmplitudeMax));
        mr.material.SetFloat("_DisplacementMult", Random.Range(DisplacementMultMin, DisplacementMultMax));
    }

    public void UpdatePosition(Vector3 pos, bool useOffset)
    {
        if (useOffset)
        {
            transform.position = pos - new Vector3(0, heightOffset, 0);
        }
        else
        {
            transform.position = pos;
        }
        UpdateScale();
        UpdateRotation();
    }

    public void UpdatePosition(Vector3 pos, float diameter)
    {
        transform.position = pos - new Vector3(0, diameter/2f, 0);
        UpdateDisplacement(diameter);
        UpdateScale();
        UpdateRotation();
    }

    public void UpdateDisplacement(float diameter)
    {
        if (useDiamter)
        {
            DisplacementMultMin = diameter * diameterMult * 0.6f;
            DisplacementMultMax = diameter * diameterMult * 1.2f;
        }
    }

    public void UpdateScale()
    {
        transform.localScale = Vector3.one * (initialScale / maxDistance * Vector3.Distance(transform.position, Vector3.zero));
    }

    public void UpdateRotation()
    {
        transform.rotation = Quaternion.identity;
        transform.rotation = Quaternion.Euler(90f + Random.Range(0, maxRotAngle), 0, 0);
        transform.Rotate(0, Random.Range(0f, 360f), 0, Space.World);
    }
}
