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
            if (!paused)
            {
                Pause();
                return;
            }

            Unpause();
        }
    }

    // Resume the game (unpauses)
    public void ResumeGame()
    {
        Unpause();
    }

    // Restart the game (reloads the current scene)
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Open the options panel and close the current pause menu panel
    public void OpenOptions()
    {
        Debug.Log("Opening options...");

        // Disable the pause menu panel
        panel.SetActive(false);

        // Enable the options panel
        optionsPanel.SetActive(true);
    }

    // Close the options panel and return to the pause menu
    public void CloseOptions()
    {
        Debug.Log("Closing options...");

        // Disable the options panel
        optionsPanel.SetActive(false);

        // Re-enable the pause menu panel
        panel.SetActive(true);
    }

    // Exit the game (loads a different scene, like a main menu or quits)
    public void ExitGame()
    {
        SceneManager.LoadScene("_MainMenu"); // Replace with the appropriate scene name
    }
}
