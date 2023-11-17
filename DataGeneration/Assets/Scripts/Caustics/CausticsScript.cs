using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CausticsScript : MonoBehaviour
{
    public MeshRenderer mr;

    public float minScale, maxScale;
    public float minLum, maxLum;
    public bool alternateCaustics;

    private void Start()
    {
        if (mr == null)
        {
            mr = GetComponent<MeshRenderer>();
        }
    }

    private void Update()
    {
        mr.material.SetFloat("_CausticsScale", Random.Range(minScale, maxScale));
        mr.material.SetFloat("_CausticsLuminanceMaskStrength", Random.Range(minLum, maxLum));

        if (alternateCaustics)
        {
            if(Random.Range(0, 2) == 0)
            {
                mr.enabled = false;
            }
            else
            {
                mr.enabled = true;
            }
        }
    }

}
