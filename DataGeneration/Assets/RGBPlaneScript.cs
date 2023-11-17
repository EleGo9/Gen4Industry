using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGBPlaneScript : MonoBehaviour
{

    Material mat;
    float[] vals;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        vals = new float[3];
    }

    // Update is called once per frame
    void Update()
    {
        /*
        vals[0] = Random.Range(0f, 1f);
        vals[1] = Random.Range(0f, 1f - vals[0]);
        vals[2] = 1 - vals[0] - vals[2];

        ShuffleColors();
        */
        mat.SetColor("_BaseColor", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
    }

    void ShuffleColors()
    {
        float temp;

        for (int i = 0; i < vals.Length; i++)
        {
            temp = vals[i];
            int randomIndex = Random.Range(i, vals.Length);
            vals[i] = vals[randomIndex];
            vals[randomIndex] = temp;
        }
    }
}
