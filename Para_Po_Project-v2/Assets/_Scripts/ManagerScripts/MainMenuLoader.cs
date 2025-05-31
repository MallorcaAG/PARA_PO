using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoader : MonoBehaviour
{
    [SerializeField] private string MainMenuSceneString;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(MainMenuSceneString);
    }
}
