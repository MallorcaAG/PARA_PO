using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public GameObject[] panels;


    public void LevelSelectionButton()
    {
        panels[1].SetActive(true);
        panels[2].SetActive(false);

    }

    public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void InstructionsButton()
    {
        panels[1].SetActive(false);
        panels[2].SetActive(true);
    }

    public void QuitButton()
    {
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
    }
}
