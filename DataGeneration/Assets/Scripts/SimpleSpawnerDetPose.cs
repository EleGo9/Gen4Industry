//using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SimpleSpawnerDetPose : MonoBehaviour
{

    public SpawnPositionScript spawner;
    public Camera cam;

    public RenderTexture rtBinary;
    //public RenderTexture rtDepth;

    List<Rect> rects = new List<Rect>();

    public Light sceneLight;
    Color lightColor;

    int folderCount = -1;
    int dataCount = 0;

    public bool writeToDisk = true;
    public int targetNumber = 50000;
    [SerializeField]
    int currentNumber = 0;
    public int startNumber = 0;

    string[] gt;
    string[] info;

    byte[] bytesDepth;
    byte[] bytesBinary;
    Texture2D texDepth;
    Texture2D texBinary;

    Matrix4x4 m;

    public bool writeModelInfo = true;

    void Start()
    {
        if(cam == null)
        {
            cam = Camera.main;
        }

        if(spawner == null)
        {
            spawner = GetComponent<SpawnPositionScript>();
        }

        spawner.InitializeObjectPool();

        m = cam.worldToCameraMatrix;
        m.SetRow(2, m.GetRow(2) * -1);
        targetNumber++;
        gt = new string[targetNumber];
        info = new string[targetNumber];
        lightColor = sceneLight.color;


        texBinary = new Texture2D(rtBinary.width, rtBinary.height, TextureFormat.RGB24, false);
        //texDepth = new Texture2D(rtDepth.width, rtDepth.height, TextureFormat.RFloat, false);
    }

    void Update()
    {
        if (currentNumber < targetNumber)
        {
            currentNumber++;
        }
        else
        {
            return;
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

        for(int i = 0; i < spawner.amount; i++)
        {
            rects.Add(GUI2dRectWithObject(spawner.spawnedObjects[i].transform.gameObject));
        }

        m = cam.worldToCameraMatrix;
        string num = "" + (dataCount + startNumber);
        string num2 = "" + ((dataCount + startNumber) - 1);
        while (num.Length < 4)
        {
            num = "0" + num;
        }
        while (num2.Length < 4)
        {
            num2 = "0" + num2;
        }

        string path = Application.dataPath + "/Dataset/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);

        }

        if (folderCount == -1)
        {
            folderCount = Directory.GetDirectories(path).Length;
            path += "ds" + folderCount + "/";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!Directory.Exists(path + "/data/"))
            {
                Directory.CreateDirectory(path + "/data/");
            }
            if (!Directory.Exists(path + "/data/01/"))
            {
                Directory.CreateDirectory(path + "/data/01/");
            }
            if (!Directory.Exists(path + "/data/01/rgb/"))
            {
                Directory.CreateDirectory(path + "/data/01/rgb/");

            }
            if (rtBinary != null && !Directory.Exists(path + "/data/01/mask/"))
            {
                Directory.CreateDirectory(path + "/data/01/mask/");
            }
            // if (rtDepth != null && !Directory.Exists(path + "/data/01/depth/"))
            // {
            //     Directory.CreateDirectory(path + "/data/01/depth/");
            // }
            if (!Directory.Exists(path + "/models/"))
            {
                Directory.CreateDirectory(path + "/models/");
                if (writeModelInfo)
                {
                    File.WriteAllText(path + "/models/models_info.yml", spawner.yoloModelsInfoString);
                }
            }
        }
        else
        {
            path += "ds" + folderCount + "/";
        }

        path += "/data/01/";

        float[] temp;

        if(dataCount < targetNumber-1)
            ScreenCapture.CaptureScreenshot(path + "/rgb/" + num + ".png");
            string s = "";
            if (!File.Exists(path + "/rgb/" + num + ".txt"))
            {
                for(int i = 0; i < spawner.amount; i++)
                {
                    temp = GetClampedBounds(rects[i]);

                    s += spawner.spawnedObjects[i].id + " " + temp[0].ToString().Replace(",", ".") + " " + temp[1].ToString().Replace(",", ".") + " "
                        + temp[2].ToString().Replace(",", ".") + " " + temp[3].ToString().Replace(",", ".") + "\n";
                }

                s = s.Substring(0, s.Length - 2);

                File.WriteAllText(path + "/rgb/" + num + ".txt", s);
            }
        
        if (rtBinary != null && dataCount > 0)
        {
            RenderTexture.active = rtBinary;
            texBinary.ReadPixels(new Rect(0, 0, rtBinary.width, rtBinary.height), 0, 0);
            RenderTexture.active = null;

            bytesBinary = texBinary.EncodeToPNG();

            File.WriteAllBytes(path + "/mask/" + num2 + ".png", bytesBinary);
        }
        // if (rtDepth != null && dataCount > 0)
        // {
        //     RenderTexture.active = rtDepth;
        //     texDepth.ReadPixels(new Rect(0, 0, rtDepth.width, rtDepth.height), 0, 0);
        //     RenderTexture.active = null;

        //     bytesDepth = texDepth.EncodeToPNG();

        //     File.WriteAllBytes(path + "/depth/" + num2 + ".png", bytesDepth);
        // }
        
        int l = currentNumber - 1;
        gt[l] = "";
        gt[l] += (dataCount + startNumber) + ":\n";


        for (int i=0; i < spawner.amount; i++)
        {

            Vector3 objPos = spawner.spawnedObjects[i].transform.position;
            Quaternion objRot = spawner.spawnedObjects[i].transform.rotation;

            objPos = Quaternion.Inverse(Camera.main.transform.rotation) * (objPos - Camera.main.transform.position) + Camera.main.transform.position;
            objRot = Quaternion.Inverse(Camera.main.transform.rotation) * objRot;

            Rect r = GUI2dRectWithObject(spawner.spawnedObjects[i].transform.gameObject);

            gt[l] += "- cam_R_m2c: [";
            Matrix4x4 mm = Matrix4x4.Rotate(objRot);
            
            mm[0, 0] *= -1;
            mm[1, 1] *= -1;
            mm[1, 2] *= -1;
            mm[2, 0] *= -1;

            for (int j = 0; j < 3; j++)
            {
                for(int k = 0; k < 3; k++)
                {
                    gt[l] += mm[j,k].ToString().Replace(",", ".");
                    if(j == 2 && k == 2)
                    {
                        gt[l] += "]\n";
                    }
                    else
                    {
                        gt[l] += ", ";
                    }
                }
            }
            gt[l] += "  cam_t_m2c: [";
            gt[l] += StringNum(objPos.x) + ", " + StringNum(-objPos.y) + ", " + StringNum(objPos.z).ToString() + "]\n";
            gt[l] += "  obj_bb: [";
            
            gt[l] += ((int)r.x).ToString() + ", " + ((int)r.y).ToString() + ", " + ((int)r.width).ToString() + ", " + ((int)r.height).ToString() + "]\n";

            if(i < spawner.amount - 1)
            {
                gt[l] += "  obj_id: " + spawner.spawnedObjects[i].id + "\n";
            }
            else
            {
                gt[l] += "  obj_id: " + spawner.spawnedObjects[i].id;
            }

        }
        info[l] = "";
        info[l] += (dataCount + startNumber) + ":\n";

        info[l] += "  cam_K: [" + StringNum(Camera.main.focalLength * Screen.width  / Camera.main.sensorSize.x) + ", 0.0, " + StringNum(Screen.width / 2f)  + ", 0.0, " +
                                  StringNum(Camera.main.focalLength * Screen.height / Camera.main.sensorSize.y) + ", "      + StringNum(Screen.height / 2f) + ", 0.0, 0.0, 1.0]\n";
        info[l] += "  depth_scale: 1.0";
        
        dataCount++;

        if(dataCount == targetNumber-1)
        {
            File.WriteAllLines(path + "gt.yml", gt);
            File.WriteAllLines(path + "info.yml", info);
        }
    }

    private float[] GetClampedBounds(Rect r)
    {
        float[] output = new float[4];

        float centerX, centerY, sizeX, sizeY, temp;

        centerX = r.center.x / Screen.width;
        centerY = r.center.y / Screen.height;
        sizeX = r.width / Screen.width;
        sizeY = r.height / Screen.height;

        output[0] = centerX;
        output[1] = centerY;
        output[2] = sizeX;
        output[3] = sizeY;
        return output;
    }

    private string StringNum(float x)
    {
        string s = x.ToString().Replace(",", ".");
        if (!s.Contains("."))
        {
            s += ".0";
        }

        return s;
    }

    private void MoveItems()
    {
        spawner.SpawnItemsAndUpdatePlane();

        sceneLight.transform.localRotation = Quaternion.Euler(Random.Range(-65f, 65f), 0f, 0f);
        sceneLight.transform.Rotate(Vector3.up, Random.Range(0f, 360f), Space.World);

        Color c = lightColor;
        c.r *= Random.Range(0.8f, 1.2f);
        c.g *= Random.Range(0.8f, 1.2f);
        c.b *= Random.Range(0.8f, 1.2f);

        sceneLight.color = c;
    }

    public Rect GUI2dRectWithObject(GameObject go)
    {
        //Vector3[] vertices = go.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] vertices = go.GetComponentInChildren<MeshFilter>().mesh.vertices;

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


    public void PrintSensorHorizontal()
    {
        Debug.Log(Camera.main.focalLength * 640f / Camera.main.sensorSize.x);
        Debug.Log(Camera.main.focalLength * 480f / Camera.main.sensorSize.y);
    }

    public void PrintSensorVertical()
    {
        Debug.Log(Camera.main.focalLength * 480f / Camera.main.sensorSize.x);
        Debug.Log(Camera.main.focalLength * 640f / Camera.main.sensorSize.y);
    }

}
