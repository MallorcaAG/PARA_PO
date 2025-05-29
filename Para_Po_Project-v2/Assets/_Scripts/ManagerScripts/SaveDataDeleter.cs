using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataDeleter : MonoBehaviour
{
    private SceneLoadManager slm;

    private DataManager data;

    private void Start()
    {
        slm = SceneLoadManager.Instance;
        
        data = DataManager.Instance;
    }

    public void DeleteSaveData()
    {
        Levels[] l = slm.getLevels();

        for (int i = 0; i < l.Length; i++)
        {
            data.CurrentLevel = l[i];
            data.delete();
        }
    }
}
