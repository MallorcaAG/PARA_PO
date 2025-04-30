using Codice.Client.Common.GameUI;
using System.Collections;
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
        success.scoreTxt.text = f[0].ToString("F4");
        success.highScoreTxt.text = f[2].ToString("F4");
        for (int i = 0; i < (int)f[1]; i++)
        {
            if(success.stars[i] != null)
                success.stars[i].sprite = activeStar;
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
                     "Article 365 of the Revised Penal Code covers charges of Homicide Through Reckless Imprudence for traffic violations that result in fatalities";
        }
        else if (code == "VIOLATION_02") //crash into building
        {
            return "<=Article 365 of R.A #4136 : " + "\t" +
                     "This article addresses reckless or negligent driving that results in damage to property or injury to people. Penalties are based on the severity of the injuries or whether the act resulted in death.";
        }
        else if (code == "VIOLATION_03") //Hitting another car
        {
            return "<=Article 365 of R.A #4136 : " + "\t" +
                     "This article addresses reckless or negligent driving that results in damage to property or injury to people.";
        }
        else if (code == "VIOLATION_04") //Counterflowing
        {
            return "<=R.A 4136 : " + "\t" +
                     "Restrictions on overtaking and passing. - The driver of a vehicle shall not drive to the left side of the center line of a highway in overtaking or passing another vehicle proceeding in the same direction, unless such left side is clearly visible, and is free of oncoming traffic for a sufficient distance ahead to permit such overtaking or passing to be made in safety.";
        }
        else if (code == "VIOLATION_05") //Hitting a sign/post/traffic object
        {
            return "<=R.A. 4136, Section 48 : " + "\t" +
                     "Reckless driving. - No person shall operate a motor vehicle on any highway recklessly or without reasonable caution considering the width, traffic, grades, crossing, curvatures, visibility and other conditions of the highway and the conditions of the atmosphere and weather, or so as to endanger the property or the safety or rights of any person or so as to cause excessive or unreasonable damage to the highway.";
        }
        else if (code == "VIOLATION_06") //Running the red light
        {
            return "<=R.A. 4136, Section 48 : " + "\t" +
                     "Reckless driving. - No person shall operate a motor vehicle on any highway recklessly or without reasonable caution considering the width, traffic, grades, crossing, curvatures, visibility and other conditions of the highway and the conditions of the atmosphere and weather, or so as to endanger the property or the safety or rights of any person or so as to cause excessive or unreasonable damage to the highway.";
        }
        else if (code == "VIOLATION_07") // Speeding
        {
            return "<=R.A. 4136, Section 35 : " + "\t" +
                     "Any person driving a motor vehicle on a highway shall drive the same at a careful and prudent speed, not greater nor less than is reasonable and proper, having due regard for the traffic, the width of the highway, and of any other condition then and there existing";
        }
        else if (code == "VIOLATION_08") // Blocking/Obstructing Traffic/Stalling/AFK
        {
            return "<=R.A 4136 Section 54 : " + "\t" +
                     "Obstruction of traffic. - No person shall drive his motor vehicle in such a manner as to obstruct or impede the passage of any vehicle, nor, while discharging or taking on passengers or loading or unloading freight, obstruct the free passage of other vehicles on the highway.";
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
            return "<=Article 365 of R.A 4136" + "\n\n" +
                     "Article 365 of the Revised Penal Code covers charges of Homicide Through Reckless Imprudence for traffic violations that result in fatalities";
        }
        else if (code == "VIOLATION_02") //crash into building
        {
            return "<=R.A 4136, Article 365" + "\n\n" +
                     "This article addresses reckless or negligent driving that results in damage to property or injury to people. Penalties are based on the severity of the injuries or whether the act resulted in death.";
        }
        else if (code == "VIOLATION_03") //Hitting another car
        {
            return "<=R.A 4136, Article 365" + "\n\n" +
                     "This article addresses reckless or negligent driving that results in damage to property or injury to people.";
        }
        else if (code == "VIOLATION_04") //Counterflowing
        {
            return "<=R.A 4136" + "\n\n" +
                     "Restrictions on overtaking and passing. - The driver of a vehicle shall not drive to the left side of the center line of a highway in overtaking or passing another vehicle proceeding in the same direction, unless such left side is clearly visible, and is free of oncoming traffic for a sufficient distance ahead to permit such overtaking or passing to be made in safety.";
        }
        else if (code == "VIOLATION_05") //Hitting a sign/post/traffic object
        {
            return "<=R.A. 4136, Section 48" + "\n\n" +
                     "Reckless driving. - No person shall operate a motor vehicle on any highway recklessly or without reasonable caution considering the width, traffic, grades, crossing, curvatures, visibility and other conditions of the highway and the conditions of the atmosphere and weather, or so as to endanger the property or the safety or rights of any person or so as to cause excessive or unreasonable damage to the highway.";
        }
        else if (code == "VIOLATION_06") //Running the red light
        {
            return "<=R.A. 4136, Section 48" + "\n\n" +
                     "Reckless driving. - No person shall operate a motor vehicle on any highway recklessly or without reasonable caution considering the width, traffic, grades, crossing, curvatures, visibility and other conditions of the highway and the conditions of the atmosphere and weather, or so as to endanger the property or the safety or rights of any person or so as to cause excessive or unreasonable damage to the highway.";
        }
        else if (code == "VIOLATION_07") // Speeding
        {
            return "<=R.A. 4136, Section 35" + "\n\n" +
                     "Any person driving a motor vehicle on a highway shall drive the same at a careful and prudent speed, not greater nor less than is reasonable and proper, having due regard for the traffic, the width of the highway, and of any other condition then and there existing";
        }
        else if (code == "VIOLATION_08") // Blocking/Obstructing Traffic/Stalling/AFK
        {
            return "<=R.A 4136, Section 54" + "\n\n" +
                     "Obstruction of traffic. - No person shall drive his motor vehicle in such a manner as to obstruct or impede the passage of any vehicle, nor, while discharging or taking on passengers or loading or unloading freight, obstruct the free passage of other vehicles on the highway.";
        }

        return "NA";
    }
}