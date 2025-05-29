using log4net.Core;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{

    public static void SaveLevel(Levels level)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + level.name + ".para";
        FileStream stream = new FileStream(path , FileMode.Create);

        SaveData data = new SaveData(level);
        
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData LoadLevel(Levels level)
    {
        string path = Application.persistentDataPath + "/" + level.name + ".para";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream (path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            Debug.Log("Found "+ level.name + " Level in " + path);

            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found in "+ path);
            return null;
        }

    }

    public static void DeleteSaveData(Levels level)
    {
        string path = Application.persistentDataPath + "/" + level.name + ".para";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
                
            Debug.Log("Found " + level.name + " Level in " + path 
                + "\n DELETING NOW. . .");

            File.Delete(path);

            RefreshEditorProjectWindow();

            if (!File.Exists(path))
            {
                Debug.Log("Save file not found in " + path);
            }
        }
        else
        {
            Debug.Log("Save file not found in " + path);
        }

        void RefreshEditorProjectWindow()
        {
        #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
        #endif
        }

    }
}
