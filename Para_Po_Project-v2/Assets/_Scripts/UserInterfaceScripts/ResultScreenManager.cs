using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultScreenManager : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private List<string> playerViolationsCode = new List<string>();
    [Header("References")]
    [SerializeField] private Sprite activeStar;
    [SerializeField] private GameObject masterPanel;
    [SerializeField] private SuccessScreen success;
    [SerializeField] private FailureScreen failure;
    [Header("Game Event")]
    [SerializeField] private GameEvent onTimeScaleChange;
    [SerializeField] private GameEvent onContinueButtonPressed;
    [SerializeField] private GameEvent onTryAgainButtonPressed;
    [SerializeField] private GameEvent onExitButtonPressed;

    [SerializeField] private List<GameObject> starsRef;

    private void Start()
    {
        masterPanel.SetActive(false);
        success.panel.SetActive(false);
        failure.panel.SetActive(false);
    }
    #region UI
    private void DisplayWin()
    {
        success.panel.SetActive(true);
    }

    private void DisplayLose()
    {
        failure.panel.SetActive(true);
    }
    #endregion
    public void gameEnded(Component sender, object data)
    {
        ZaWarudo();
        masterPanel.SetActive(true);

        if(data.GetType() == typeof(string))
        {
            DisplayLose();
            failure.failureTypeTxt.text = (string)data;
            failure.populateViolationsTxt(playerViolationsCode);
            
            return;
        }

        DisplayWin();
        float[] f = (float[])data;
        success.scoreTxt.text = f[0].ToString("####0");
        success.highScoreTxt.text = f[2].ToString("####0");
        int stars = (int)f[1];

        switch(stars)
        {
            case 1:
                starsRef[0].GetComponent<Image>().sprite = activeStar;
                break;
            case 2:
                starsRef[0].GetComponent<Image>().sprite = activeStar;
                starsRef[1].GetComponent<Image>().sprite = activeStar;
                break;
            case 3:
                starsRef[0].GetComponent<Image>().sprite = activeStar;
                starsRef[1].GetComponent<Image>().sprite = activeStar;
                starsRef[2].GetComponent<Image>().sprite = activeStar;
                break;
            default:
                Debug.LogWarning("No stars to display");
                break;
        }

        success.populateViolationsTxt(playerViolationsCode);
    }

    #region Buttons
    public void ContinueButton()
    {
        onContinueButtonPressed.Raise(this, 0);
    }

    public void TryAgainButton()
    {
        onTryAgainButtonPressed.Raise(this, SceneManager.GetActiveScene().name);
    }

    public void ExitButton()
    {
        onExitButtonPressed.Raise(this, "_MainMenu");
    }
    #endregion
    #region Utility
    public void addViolations(Component sender, object data)
    {
        if (data.GetType() == typeof(float))
        {
            return;
        }

        string code = (string)data;
        playerViolationsCode.Add(code);
    }

    public void ZaWarudo()
    {
        onTimeScaleChange.Raise(this, 0f);
    }

    public void TimeResume()
    {
        onTimeScaleChange.Raise(this, 1f);
    }
    #endregion
}

[System.Serializable]
class SuccessScreen
{
    public GameObject panel;
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI highScoreTxt;
    public TextMeshProUGUI[] violationsPos;
    public Image[] stars;

    public void populateViolationsTxt(List<string> playerViolationsCode)
    {
        if(playerViolationsCode.Count == 0)
        {
            violationsPos[0].alignment = TextAlignmentOptions.Center;
            violationsPos[0].text = "NO VIOLATIONS. GOOD JOB!";
            return;
        }

        for (int i = 0; i < playerViolationsCode.Count; i++)
        {
            violationsPos[i].text = getViolationType(playerViolationsCode[i]);
        }
    }

    private string getViolationType(string code)
    {
        if (code == "VIOLATION_01") //Running over pedestrian
        {
            return "<=Article 365 of R.A #4136 : " + "\t" +
                     "Reckless Imprudence - Ran over pedestrian";
        }
        else if (code == "VIOLATION_02") //crash into building
        {
            return "<=Article 365 of R.A #4136 : " + "\t" +
                     "Reckless Imprudence - Property Damage";
        }
        else if (code == "VIOLATION_03") //Hitting another car
        {
            return "<=Article 365 of R.A #4136 : " + "\t" +
                     "Reckless Driving";
        }
        else if (code == "VIOLATION_04") //Counterflowing
        {
            return "<=R.A 4136 : " + "\t" +
                      "Counterflowing";
        }
        else if (code == "VIOLATION_05") //Hitting a sign/post/traffic object
        {
            return "<=R.A. 4136, Section 48 : " + "\t" +
                     "Reckless driving";
        }
        else if (code == "VIOLATION_06") //Running the red light
        {
            return "<=R.A. 4136, Section 48 : " + "\t" +
                     "Beating the red light";
        }
        else if (code == "VIOLATION_07") // Speeding
        {
            return "<=R.A. 4136, Section 35 : " + "\t" +
                      "Overspeeding";
        }
        else if (code == "VIOLATION_08") // Blocking/Obstructing Traffic/Stalling/AFK
        {
            return "<=R.A 4136 Section 54 : " + "\t" +
                     "Obstruction of traffic";
        }
        else if (code == "VIOLATION_09") // Blowing of Horn
        {
            return "<=R.A 4136 Section 54 : " + "\t" +
                     "Blowing of Horn";
        }

        return "NA";
    }
}

[System.Serializable]
class FailureScreen
{
    public GameObject panel;
    public TextMeshProUGUI failureTypeTxt;
    public TextMeshProUGUI[] violationsPos;



    public void populateViolationsTxt(List<string> playerViolationsCode)
    {
        if (playerViolationsCode.Count == 0)
        {
            violationsPos[0].text = "NO VIOLATIONS. GOOD JOB!";
            return;
        }

        for (int i = 0; i < violationsPos.Length; i++)
        {
            violationsPos[i].text = getViolationType(playerViolationsCode[i]);
        }
    }

    private string getViolationType(string code)
    {
        if (code == "VIOLATION_01") //Running over pedestrian
        {
            return "<=Article 365 of R.A #4136 : " + "\t" +
                     "Reckless Imprudence - Ran over pedestrian";
        }
        else if (code == "VIOLATION_02") //crash into building
        {
            return "<=Article 365 of R.A #4136 : " + "\t" +
                     "Reckless Imprudence - Property Damage";
        }
        else if (code == "VIOLATION_03") //Hitting another car
        {
            return "<=Article 365 of R.A #4136 : " + "\t" +
                     "Reckless Driving";
        }
        else if (code == "VIOLATION_04") //Counterflowing
        {
            return "<=R.A 4136 : " + "\t" +
                      "Counterflowing";
        }
        else if (code == "VIOLATION_05") //Hitting a sign/post/traffic object
        {
            return "<=R.A. 4136, Section 48 : " + "\t" +
                     "Reckless driving";
        }
        else if (code == "VIOLATION_06") //Running the red light
        {
            return "<=R.A. 4136, Section 48 : " + "\t" +
                     "Beating the red light";
        }
        else if (code == "VIOLATION_07") // Speeding
        {
            return "<=R.A. 4136, Section 35 : " + "\t" +
                      "Overspeeding";
        }
        else if (code == "VIOLATION_08") // Blocking/Obstructing Traffic/Stalling/AFK
        {
            return "<=R.A 4136 Section 54 : " + "\t" +
                     "Obstruction of traffic";
        }
        else if (code == "VIOLATION_09") // Blowing of Horn
        {
            return "<=R.A 4136 Section 54 : " + "\t" +
                     "Blowing of Horn";
        }

        return "NA";
    }
}