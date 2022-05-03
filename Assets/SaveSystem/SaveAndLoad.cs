using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
    public List<int> list1 = new List<int>();

    public Vector3 xyz = new Vector3();

    private SaveData saveData;

    public void Save()
    {
        if (!Directory.Exists(Application.dataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.dataPath + "/saves");
        }

        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Application.dataPath + "/saves/SaveData.dat");

        CopySaveData();
        bf.Serialize(file, saveData);
        file.Close();
    }

    public void CopySaveData()
    {
        saveData.list1.Clear();

        foreach (int i in list1)
        {
            saveData.list1.Add(i);
        }

        saveData.position = SaveVector3(xyz);
    }

    public static Vector3SaveData SaveVector3(Vector3 vector3)
    {
        Vector3SaveData vector3SaveData = new Vector3SaveData();

        vector3SaveData.x = vector3.x;
        vector3SaveData.y = vector3.y;
        vector3SaveData.z = vector3.z;

        return vector3SaveData;
    }
}

[System.Serializable]
public class SaveData
{
    public Vector3SaveData position;
    public List<int> list1 = new List<int>();
}

[System.Serializable]
public class Vector3SaveData
{
    public float x;
    public float y;
    public float z;
}

