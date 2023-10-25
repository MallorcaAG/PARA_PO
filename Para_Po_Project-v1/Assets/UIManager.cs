using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameManager gm;
    public GameObject[] alive;
    public GameObject[] dead;
    public TextMeshProUGUI timerText;
    public GameObject WinPanel;
    public GameObject gameOver;

    void Update()
    {
        updateLivesDisplay();
        updateTimerDisplay();
    }

    void updateLivesDisplay()
    {
        int i = gm.getPlayerLives();

        switch (i)
        {
            case 3:
                alive[0].SetActive(true);
                alive[1].SetActive(true);
                alive[2].SetActive(true);
                dead[0].SetActive(false);
                dead[1].SetActive(false);
                dead[2].SetActive(false);
                break;

            case 2:
                alive[0].SetActive(false);
                alive[1].SetActive(true);
                alive[2].SetActive(true);
                dead[0].SetActive(true);
                dead[1].SetActive(false);
                dead[2].SetActive(false);
                break; 
            
            case 1:
                alive[0].SetActive(false);
                alive[1].SetActive(false);
                alive[2].SetActive(true);
                dead[0].SetActive(true);
                dead[1].SetActive(true);
                dead[2].SetActive(false);
                break; 
            
            case 0:
                alive[0].SetActive(false);
                alive[1].SetActive(false);
                alive[2].SetActive(false);
                dead[0].SetActive(true);
                dead[1].SetActive(true);
                dead[2].SetActive(true);
                break;
        }
    }

    void updateTimerDisplay()
    {
        int t = gm.getTimer();

        timerText.text = t.ToString();
    }

    public void displayWinPanel()
    {
        WinPanel.SetActive(true);
    }

    public void displayGameOverPanel()
    {
        gameOver.SetActive(true);
    }
}
