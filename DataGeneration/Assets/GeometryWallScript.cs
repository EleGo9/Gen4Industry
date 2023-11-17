using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryWallScript : MonoBehaviour
{
    public MeshRenderer mr;
    public int MinResolution = 10, maxResolution = 20;
    public List<GameObject> objects;
    public List<Material> materials;

    List<Transform> pool;
    Bounds b;
    int resolution;
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        b = mr.bounds;

        pool = new List<Transform>();
        GameObject temp;
        foreach(GameObject go in objects)
        {
            for(int i = 0; i <= maxResolution; i++)
            {
                for (int j = 0; j <= maxResolution; j++)
                {
                    temp = Instantiate(go, this.transform);
                    temp.SetActive(false);
                    pool.Add(temp.transform);
                }  
            }
        }

        UpdateGeometryWall();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGeometryWall();
    }

    void UpdateGeometryWall()
    {
        resolution = Random.Range(MinResolution, maxResolution + 1);
        Vector3 X, Y;
        X = Vector3.Project(b.min, transform.right);
        Y = Vector3.Project(b.min, transform.forward);
        Vector3 origin = b.center - X - Y;
        X *= 2;
        Y *= 2;

        ShufflePool();

        int index = 0;
        Vector3 temp;
        for (int i = 0; i <= resolution; i++)
        {
            for (int j = 0; j <= resolution; j++)
            {
                temp = X * i / resolution + Y * j / resolution;
                pool[index].gameObject.SetActive(true);
                pool[index].position = origin + temp;
                pool[index].rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                pool[index].GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Count)];
                index++;
            }
        }
    }

    void ShufflePool()
    {
        Transform temp;

        foreach(Transform t in pool)
        {
            t.gameObject.SetActive(false);
        }

        for (int i = 0; i < pool.Count; i++)
        {
            temp = pool[i];
            int randomIndex = Random.Range(i, pool.Count);
            pool[i] = pool[randomIndex];
            pool[randomIndex] = temp;
        }
    }
    /*
    private void OnDrawGizmos()
    {
        mr = GetComponent<MeshRenderer>();
        Bounds b = mr.bounds;
        Vector3 v = b.min;
        //Gizmos.DrawSphere(v, 1000);
        //Gizmos.DrawSphere(b.max, 1000);
        Vector3 X, Y;
        X = Vector3.Project(b.min, transform.right);
        Y = Vector3.Project(b.min, transform.forward);
        Vector3 origin = b.center - X - Y;
        X *= 2;
        Y *= 2;

        Gizmos.DrawLine(origin, origin + X);
        Gizmos.DrawLine(origin, origin + Y);

        Gizmos.DrawSphere(v, 1000);

        Vector3 temp;
        for(int i = 0; i <= resolution; i++)
        {
            for(int j = 0; j <= resolution; j++)
            {
                temp = X * i / resolution + Y * j / resolution;
                Gizmos.DrawSphere(origin + temp, 100);
            }
        }
        
    }
    */
}
