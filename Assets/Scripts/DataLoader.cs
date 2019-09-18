using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/* loading data configurations from json*/

public class DataLoader
{

    public SlotData[] LoadData(string path)
    {
        SlotDataCollection loadedData;
        TextAsset pathResource = (TextAsset)Resources.Load(path);
        string jsonData = pathResource.text;
        loadedData = JsonUtility.FromJson<SlotDataCollection>(jsonData);
        return loadedData.SlotDatas;
    }

    public StopData[] LoadDataStop(string path)
    {
        StopDataCollection loadedData;
        TextAsset pathResource = (TextAsset)Resources.Load(path);
        string jsonData = pathResource.text;
        loadedData = JsonUtility.FromJson<StopDataCollection>(jsonData);
        return loadedData.StopDatas;

    }

    [System.Serializable]
    public struct StopData
    {
        public string[] names;
        public int won;
    }

    [System.Serializable]
    public struct SlotData
    {
        public string name;
        public int value;

    }

    [System.Serializable]
    public struct SlotDataCollection
    {
        public SlotData[] SlotDatas;
    }

    [System.Serializable]
    public struct StopDataCollection
    {
        public StopData[] StopDatas;
    }
}



