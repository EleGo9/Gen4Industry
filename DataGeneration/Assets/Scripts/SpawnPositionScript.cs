using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStruct
{
    public Transform transform = null;
    public float diameter = 0;
    public string info = "";
    public int id = 0;

    public ObjectStruct(Transform transform, float diameter, string info)
    {
        this.transform = transform;
        this.diameter = diameter;
        this.info = info;
    }
    public ObjectStruct(Transform transform, float diameter, string info, int id)
    {
        this.transform = transform;
        this.diameter = diameter;
        this.info = info;
        this.id = id;
    }

    public ObjectStruct() {}
}

public class SpawnPositionScript : MonoBehaviour
{
    // public bool useSand = false;

    public BoxCollider topCollider, botCollider;

    Vector3 pos;

    public int amount = 1;

    public List<GameObject> objects;
    public List<int> IDs;
    

    public PlaneScript plane;

    public List<ObjectStruct> spawnedObjects;

    public float collisionDiameterMultiplier = 4f;

    public string yoloModelsInfoString = "";

    float minDiameter = float.MaxValue;

    private void Start()
    {
        //InitializeObjectPool();
    }

    private void Update()
    {
        SpawnItemsAndUpdatePlane();
    }

    public void SpawnItemsAndUpdatePlane()
    {
        // if (useSand)
        // {
        //     SpawnItemsAndUpdateSand();
        //     return;
        // }

        for (int i = 0; i < amount; i++)
        {
            spawnedObjects[i].transform.gameObject.SetActive(false);
        }

        ShuffleObjects();

        for (int i = 0; i < amount; i++)
        {
            spawnedObjects[i].transform.gameObject.SetActive(true);
            spawnedObjects[i].transform.position = GetSpawnPosition(false);
            spawnedObjects[i].transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        }
        
    }
    public void SpawnItemsAndUpdateSpecialPlane()
    {
        GetSpawnPosition(true);
        Plane p = PlaneFromBasePlane();

        for (int i = 0; i < amount; i++)
        {
            spawnedObjects[i].transform.gameObject.SetActive(false);
        }

        ShuffleObjects();

        for (int i = 0; i < amount; i++)
        {
            spawnedObjects[i].transform.gameObject.SetActive(true);
            spawnedObjects[i].transform.position = GetObjectSpawnPosition(p) + new Vector3(0, spawnedObjects[i].diameter / 2f, 0);
            spawnedObjects[i].transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        }
        AvoidCollisions(p, collisionDiameterMultiplier);
    }

    public void AvoidCollisions(Plane p, float multiplier)
    {
        if (amount > 1)
        {
            Vector3 temp;
            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < amount; j++)
                {
                    if (i == j)
                        continue;
                    temp = p.ClosestPointOnPlane(spawnedObjects[j].transform.position) - p.ClosestPointOnPlane(spawnedObjects[i].transform.position);
                    if (temp.magnitude < spawnedObjects[i].diameter)
                    {
                        Debug.Log("moving object " + i);
                        spawnedObjects[i].transform.position -= temp.normalized * ((spawnedObjects[i].diameter - temp.magnitude) / multiplier);
                    }
                }
            }

        }
    }

    public void InitializeObjectPool()
    {
        spawnedObjects = new List<ObjectStruct>();

        while (IDs.Count < objects.Count)
        {
            IDs.Add(IDs.Count);
        }

        Transform t;
        float diameter = 0;
        string s = "";

        for(int i=0; i < objects.Count; i++)
        {
            for(int j=0; j < amount; j++)
            {
                t = Instantiate(objects[i], transform).transform;
                if(j == 0)
                {
                    ModelInfoGenerator mig = t.gameObject.AddComponent<ModelInfoGenerator>();
                    mig.CalculateInfo(IDs[i]);
                    s = mig.info;
                    diameter = mig.diameter;
                    Destroy(mig);

                    if(diameter < minDiameter)
                    {
                        minDiameter = diameter;
                    }

                    yoloModelsInfoString += s + "\n";
                }
                spawnedObjects.Add(new ObjectStruct(t, diameter, s, IDs[i]));
                t.gameObject.SetActive(false);
            }
        }

        yoloModelsInfoString = yoloModelsInfoString.Substring(0, yoloModelsInfoString.Length - 2); //remove the /n at the end

        //sand.UpdateDisplacement(minDiameter);
    }

    void ShuffleObjects()
    {
        ObjectStruct temp;

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            temp = spawnedObjects[i];
            int randomIndex = Random.Range(i, spawnedObjects.Count);
            spawnedObjects[i] = spawnedObjects[randomIndex];
            spawnedObjects[randomIndex] = temp;
        }
    }

    public Vector3 GetSpawnPosition(bool isForSand)
    {
        float t = Random.Range(0f, 1f);
        float offsetX, offsetY, offsetZ;
        float maxX, maxZ;
        maxX = Mathf.Lerp(topCollider.bounds.extents.x, botCollider.bounds.extents.x, t);
        maxZ = Mathf.Lerp(topCollider.bounds.extents.z, botCollider.bounds.extents.z, t);

        offsetX = Random.Range(-maxX, maxX);
        offsetZ = Random.Range(-maxZ, maxZ);
        offsetY = Mathf.Lerp(topCollider.transform.position.y, botCollider.transform.position.y, t);
        pos = new Vector3(offsetX, offsetY, offsetZ);

        // if (sand != null && isForSand)
        // {
        //     sand.UpdatePosition(pos, false);
        // }

        return pos;
    }

    Plane PlaneFromBasePlane()
    {
        Vector3 vertical = plane.transform.up * plane.transform.localScale.y;
        Vector3 horizontal = plane.transform.right * plane.transform.localScale.x;

        Vector3 p1, p2, p3;
        p1 = plane.transform.position + vertical + horizontal;
        p2 = plane.transform.position + vertical - horizontal;
        p3 = plane.transform.position - vertical + horizontal;
        return new Plane(p1, p2, p3);
        
    }

    Vector3 GetObjectSpawnPosition(Plane p)
    {
        Vector3 output = GetSpawnPosition(false);
        Vector3 cameraPos = Camera.main.transform.position;

        p.Raycast(new Ray(cameraPos, output - cameraPos), out float distance);

        output = cameraPos + (output - cameraPos).normalized * distance;

        return output;
    }

    /*
    private void OnDrawGizmos()
    {
        Vector3 temp;

        Gizmos.color = Color.green;

        for (int i = 0; i < 20; i++)
        {
            for(int j = 0; j < 20; j++)
            {
                temp = sand.transform.position + new Vector3(i * 500 - 5000, 0, j * 500 - 5000);
                Gizmos.DrawSphere(tempPlane.ClosestPointOnPlane(temp),100);
            }
        }
        
        Gizmos.color = Color.red;
    }
    */
}
