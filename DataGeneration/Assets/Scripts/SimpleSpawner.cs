//using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class SimpleSpawner : MonoBehaviour
{
    public BoxCollider spawnArea;
    Bounds spawnAreaBounds;
    public int amount = 5;
    public int id = 1;
    public int startingNum = 0;
    public int increaseAmount = 2;
    int currentNum;
    
    bool isTrain = true;
    public GameObject spawnObject;
    List<Transform> objs = new List<Transform>();

    List<Rect> rects = new List<Rect>();

    public Light sceneLight;
    Color lightColor;

    int folderCount = -1;

    [SerializeField]
    int dataCount = 0;

    public bool writeToDisk = true;
    public int targetNumber = 50000;
    public int testCount = 10;
    int currentNumber = 0;

    void Start()
    {
        spawnArea.enabled = true;
        spawnAreaBounds = spawnArea.bounds;
        spawnArea.enabled = false;
        for(int i=0; i < amount; i++)
        {
            objs.Add(Instantiate(spawnObject, transform).transform);
        }
        lightColor = sceneLight.color;

        currentNum = startingNum;
    }
    
    void Update()
    {
        if (dataCount < targetNumber)
        {
            currentNumber++;
        }
        else
        {
            if (isTrain)
            {
                isTrain = false;
                currentNum = startingNum;
                targetNumber = testCount;
                dataCount = 0;
                Debug.Log("creating testing set");
            }
            else
            {
                Debug.Log("done!");
                return;
            }
            
        }
        
        MoveItems();

        if (writeToDisk)
        {
            GenerateDataset();
        }

    }

    private void GenerateDataset()
    {
        rects.Clear();
        foreach (Transform t in objs)
        {
                rects.Add(GUI2dRectWithObject(t.gameObject));
        }
        

        string path = Application.dataPath + "/Dataset/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            
        }
        
        if (folderCount == -1)
        {
            folderCount = Directory.GetDirectories(path).Length;
            path += folderCount + "/";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!Directory.Exists(path + "/train/"))
            {
                Directory.CreateDirectory(path + "/train/");
            }


            if (!Directory.Exists(path + "/test/"))
            {
                Directory.CreateDirectory(path + "/test/");
            }
        }
        else
        {
            path += folderCount + "/";
        }
        if (isTrain)
        {
            ScreenCapture.CaptureScreenshot(path + "/train/image_" + currentNum + ".png");
            string s = "";
            if (!File.Exists(path + "/train/image_" + currentNum + ".txt"))
            {
                foreach (Rect r in rects)
                {
                    s += id + " " + Mathf.Clamp01(r.center.x / Screen.width).ToString().Replace(",", ".") + " " + Mathf.Clamp01(r.center.y / Screen.height).ToString().Replace(",", ".") + " "
                        + Mathf.Clamp01(r.width / Screen.width).ToString().Replace(",", ".") + " " + Mathf.Clamp01(r.height / Screen.height).ToString().Replace(",", ".") + "\n";
                }
                s = s.Substring(0, s.Length - 2);

                File.WriteAllText(path + "/train/image_" + currentNum + ".txt", s);
            }
        }
        else
        {
            ScreenCapture.CaptureScreenshot(path + "/test/image_" + currentNum + ".png");
            string s = "";
            if (!File.Exists(path + "/test/image_" + currentNum + ".txt"))
            {
                foreach (Rect r in rects)
                {
                    s += id + " " + Mathf.Clamp01(r.center.x / Screen.width).ToString().Replace(",", ".") + " " + Mathf.Clamp01(r.center.y / Screen.height).ToString().Replace(",", ".") + " "
                        + Mathf.Clamp01(r.width / Screen.width).ToString().Replace(",", ".") + " " + Mathf.Clamp01(r.height / Screen.height).ToString().Replace(",", ".") + "\n";
                }
                s = s.Substring(0, s.Length - 2);

                File.WriteAllText(path + "/test/image_" + currentNum + ".txt", s);
            }
        }
        

        dataCount++;
        currentNum += increaseAmount;

    }

    private void MoveItems()
    {
        float offsetX, offsetY, offsetZ;
        foreach (Transform t in objs)
        {
            offsetX = UnityEngine.Random.Range(-spawnAreaBounds.extents.x, spawnAreaBounds.extents.x);
            offsetY = Random.Range(-spawnAreaBounds.extents.y, spawnAreaBounds.extents.y);
            offsetZ = Random.Range(-spawnAreaBounds.extents.z, spawnAreaBounds.extents.z);
            t.position = spawnArea.transform.position + new Vector3(offsetX, offsetY, offsetZ);
            t.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        }

        sceneLight.transform.rotation = Quaternion.Euler(Random.Range(0f,180f),Random.Range(0f,360f),0f);
        Color c = lightColor;
        c.r *= Random.Range(0.8f, 1.2f);
        c.g *= Random.Range(0.8f, 1.2f);
        c.b *= Random.Range(0.8f, 1.2f);
        sceneLight.color = c;
    }

    public Rect GUI2dRectWithObject(GameObject go)
    {
        Vector3[] vertices = go.GetComponent<MeshFilter>().mesh.vertices;

        float x1 = float.MaxValue, y1 = float.MaxValue, x2 = 0.0f, y2 = 0.0f;

        foreach (Vector3 vert in vertices)
        {
            Vector2 tmp = WorldToGUIPoint(go.transform.TransformPoint(vert));

            if (tmp.x < x1) x1 = tmp.x;
            if (tmp.x > x2) x2 = tmp.x;
            if (tmp.y < y1) y1 = tmp.y;
            if (tmp.y > y2) y2 = tmp.y;
        }

        Rect bbox = new Rect(x1, y1, x2 - x1, y2 - y1);
        return bbox;
    }

    public static Vector2 WorldToGUIPoint(Vector3 world)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(world);
        screenPoint.y = (float)Screen.height - screenPoint.y;
        return screenPoint;
    }
}
