using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private bool getInput = true;
    [SerializeField] private GameObject panel; // The main pause menu panel
    [SerializeField] private GameObject optionsPanel; // The options panel
    [SerializeField] private GameEvent onTimeScaleChange;
    [SerializeField] private GameEvent onTryAgainButtonPressed;
    [SerializeField] private GameEvent onExitButtonPressed;

    private bool paused = false;

    private void Start()
    {
        Unpause();
    }

    private void Update()
    {
        if (getInput)
        {
            HandleInput();
        }
    }

    public void DeactivateInput()
    {
        gameObject.SetActive(false);
        getInput = false;
    }

    public void Unpause()
    {
        paused = false;
        panel.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f;
        onTimeScaleChange.Raise(this, 1f);
    }

    public void Pause()
    {
        paused = true;
        panel.SetActive(true); // Show the pause menu
        Time.timeScale = 0f;
        onTimeScaleChange.Raise(this, 0f);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsPanel.activeSelf)
            {
              
                CloseOptions();
                return;
            }

            if (!paused)
            {
                Pause();
                return;
            }

            Unpause();
        }
    }

   
    public void ResumeGame()
    {
        Unpause();
    }

    public void RestartGame()
    {
        onTryAgainButtonPressed.Raise(this, SceneManager.GetActiveScene().name);
    }

    
    public void OpenOptions()
    {
        Debug.Log("Opening options...");

       
        panel.SetActive(false);

     
        optionsPanel.SetActive(true);
    }

  
    public void CloseOptions()
    {
        Debug.Log("Closing options...");

 
        optionsPanel.SetActive(false);

  
        panel.SetActive(true);
    }


    public void ExitGame()
    {
        onExitButtonPressed.Raise(this, "_MainMenu");
    }
}
