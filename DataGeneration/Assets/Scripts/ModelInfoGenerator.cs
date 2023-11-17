using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ModelInfoGenerator : MonoBehaviour
{
    public MeshFilter mf;

    public float meshScale = 1f;

    public float minX, minY, minZ;
    public float sizeX, sizeY, sizeZ;
    public float diameter;

    public int ID = 1;
    public string info = "";

    Vector3 pos = Vector3.zero;

    private void Start()
    {
        CalculateInfo();
    }

    public void CalculateInfo(int id = -1)
    {
        minX = minY = minZ = float.MaxValue;
        sizeX = sizeY = sizeZ = float.MinValue;
        Vector3[] vertices;
        if (mf == null)
            mf = GetComponent<MeshFilter>();
        vertices = mf.sharedMesh.vertices;


        foreach (Vector3 v in vertices)
        {
            if (v.x < minX)
            {
                minX = v.x;
            }
            if (v.y < minY)
            {
                minY = v.y;
            }
            if (v.z < minZ)
            {
                minZ = v.z;
            }

            if (v.x > sizeX)
            {
                sizeX = v.x;
            }
            if (v.y > sizeY)
            {
                sizeY = v.y;
            }
            if (v.z > sizeZ)
            {
                sizeZ = v.z;
            }


        }


        pos.x = (minX + sizeX) / 2f;
        pos.y = (minY + sizeY) / 2f;
        pos.z = (minZ + sizeZ) / 2f;

        sizeX -= minX;
        sizeY -= minY;
        sizeZ -= minZ;

        diameter = Vector3.Distance(Vector3.zero, new Vector3(sizeX, sizeY, sizeZ));

        GenerateString(id);
    }

    public void GenerateString()
    {

        //2: {diameter: 56100, min_x: -5600.85, min_y: -12503.5, min_z: -24758.89, size_x: 11199.75, size_y: 25064.08, size_z: 46837.09}
    
        info = ID + ": {";
        info += "diameter: " + (diameter * meshScale).ToString().Replace(",", ".") + ", ";
        info += "min_x: " + (minX * meshScale).ToString().Replace(",", ".") + ", ";
        info += "min_y: " + (minY * meshScale).ToString().Replace(",", ".") + ", ";
        info += "min_z: " + (minZ * meshScale).ToString().Replace(",", ".") + ", ";
        info += "size_x: " + (sizeX * meshScale).ToString().Replace(",", ".") + ", ";
        info += "size_y: " + (sizeY * meshScale).ToString().Replace(",", ".") + ", ";
        info += "size_z: " + (sizeZ * meshScale).ToString().Replace(",", ".") + "}";
    }
    public void GenerateString(int id)
    {
        if(id < 0)
        {
            GenerateString();
            return;
        }

        //2: {diameter: 56100, min_x: -5600.85, min_y: -12503.5, min_z: -24758.89, size_x: 11199.75, size_y: 25064.08, size_z: 46837.09}

        info = id + ": {";
        info += "diameter: " + (diameter * meshScale).ToString().Replace(",", ".") + ", ";
        info += "min_x: " + (minX * meshScale).ToString().Replace(",", ".") + ", ";
        info += "min_y: " + (minY * meshScale).ToString().Replace(",", ".") + ", ";
        info += "min_z: " + (minZ * meshScale).ToString().Replace(",", ".") + ", ";
        info += "size_x: " + (sizeX * meshScale).ToString().Replace(",", ".") + ", ";
        info += "size_y: " + (sizeY * meshScale).ToString().Replace(",", ".") + ", ";
        info += "size_z: " + (sizeZ * meshScale).ToString().Replace(",", ".") + "}";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(pos, diameter / 2f);
    }
}

[CustomEditor(typeof(ModelInfoGenerator))]
public class ModelInfoGeneratorEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ModelInfoGenerator s = (ModelInfoGenerator)target;
        if (GUILayout.Button("Generate String"))
        {
            s.GenerateString();
        }
        if (GUILayout.Button("Calculate Info"))
        {
            s.CalculateInfo();
        }
    }
}
