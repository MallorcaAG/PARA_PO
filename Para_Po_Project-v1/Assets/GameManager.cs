using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{


    [SerializeField] [Range(1,3)] private int playerLives = 3;
    [SerializeField] [Range(5,60)] private float currentTime = 30;
    [SerializeField] private string nameOfNextScene;
    

    [Header("Script References")]
    public UIManager UI;


    private bool playerWins = false;
    private bool timerRunning = true;
    private bool zawarudo = false;

    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            currentTime -= Time.deltaTime;
        }

        checkForLoseCondition();

        if(playerWins)
        {
            WinGameSequence();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Trigger Enter");

            playerWins = true;
        }
        
    }

    void checkForLoseCondition()
    {
        //Timer Ran Out
        if(currentTime<=0) { /*Debug.Log("Player lost");*/currentTime = 0; GameOverSequence(); }
        //Player ran over several pedestrians
        if (playerLives == 0) { /*Debug.Log("Player lost");*/ GameOverSequence(); }
    }

    public void DeductPlayerLives()
    {
        playerLives--;

        Debug.Log("Player lives remainging: "+ playerLives);
    }

    public int getPlayerLives()
    {
        return playerLives;
    }

    public int getTimer()
    {
        return (int)currentTime;
    }

    void GameOverSequence()
    {
        timerRunning = false;
        Time.timeScale = 0;
        UI.displayGameOverPanel();

        if(Input.GetKey(KeyCode.Space)) { ReloadScene(); }
    }

    void WinGameSequence()
    {
        timerRunning = false;
        if(!zawarudo)
        {
            Time.timeScale = 0.2f;
        }
        
        StartCoroutine(delayPause());
        UI.displayWinPanel();

        if (Input.GetKey(KeyCode.Space)) 
        { 
            SceneManager.LoadScene(nameOfNextScene, LoadSceneMode.Single); 
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    IEnumerator delayPause()
    {
        yield return new WaitForSecondsRealtime(5);
        Time.timeScale = 0;
        zawarudo = true;
    }
}
