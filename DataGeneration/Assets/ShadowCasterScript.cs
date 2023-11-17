using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCasterScript : MonoBehaviour
{
    public Transform t;
	public Transform target;
    public Transform light;
    public float sizeMult = 0.75f;

    void Start()
    {
        if (t == null)
            t = this.transform;
    }
    // Update is called once per frame
    void Update()
    {
        float distA = Vector3.Distance(t.position, light.position);
        float distB = Vector3.Distance(t.position, target.position);
        float temp = (distA + distB) / distA * sizeMult;
        float angle = Vector3.Angle(t.position, light.position);
        //temp *= Mathf.Cos(angle * Mathf.Deg2Rad);
        Vector3 tempV = Vector3.Project(t.position - light.position, light.forward);
        //tempV = t.position + (light.position + tempV) / 2f;
        //tempV = light.position + (t.position - light.position).normalized * tempV.magnitude;
        //tempV = light.position + (t.position - light.position).normalized * tempV.magnitude - (light.position + tempV - t.position) / 2f;
        tempV = t.position - 1.41f * (light.position + tempV - t.position) * tempV.magnitude / Mathf.Cos(angle * Mathf.Deg2Rad);
        
        this.transform.localScale = new Vector3(temp,temp,temp);
        this.transform.position = tempV;
        
    }

    void OnDrawGizmos()
    {
        Vector3 temp = Vector3.Project(t.position - light.position, light.forward);
        Gizmos.DrawRay(light.position, temp);
        Gizmos.DrawLine(light.position + temp, t.position);
        //Gizmos.DrawSphere(-t.position + light.position - temp + (t.position - light.position).normalized * temp.magnitude, 0.3f);
        //Gizmos.DrawSphere(light.position + (t.position - light.position).normalized * temp.magnitude - (light.position + temp - t.position)/2f, 0.3f);
        Gizmos.DrawSphere(t.position - (light.position + temp - t.position) / 2f, 0.3f);
        Gizmos.DrawRay(light.position, light.position + temp - t.position);
        //Gizmos.DrawSphere(t.position + 2 * temp.magnitude * (t.position - light.position).normalized - temp, 0.3f);
    }
}
