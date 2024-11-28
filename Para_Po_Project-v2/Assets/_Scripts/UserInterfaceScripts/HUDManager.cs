using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class HUDManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField][Range(0.1f,1f)] private float ParaPoTextFadeInTransition = 1f;
    [SerializeField][Range(1f,10f)] private float ParaPoTextStayTime = 1f;
    [SerializeField][Range(0.5f, 3f)] private float ParaPoTextFadeOutTransition = 1f;
    [SerializeField][Range(0.1f, 5f)] private float SignageTransition = 1f;
    [SerializeField][Range(1f, 10f)] private float SignageStayTime = 1f;

    #region References
    [Header("References")]
    [SerializeField] private Animator PassengerCountAnimator;
    [SerializeField] private Animator LocationAnimator;
    [SerializeField] private Animator ViolationsAnimator;
    [SerializeField] private TextMeshProUGUI passengerCountText;
    [SerializeField] private TextMeshProUGUI gameTimerText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI LandmarkText;
    [SerializeField] private TextMeshProUGUI ViolationPopUpTicketText;
    [SerializeField] private TextMeshProUGUI Violation1;
    [SerializeField] private TextMeshProUGUI Violation2;
    [SerializeField] private TextMeshProUGUI Violation3;
    [SerializeField] private TextMeshProUGUI SpeedText;
    
    //Animation Hashes
    private static readonly int ParaPoFadeIn = Animator.StringToHash("ParaPo_FadeIn");
    private static readonly int ParaPoFadeOut = Animator.StringToHash("PassengerCount");
    private static readonly int SignageDrop = Animator.StringToHash("RevealLocation");
    private static readonly int SignageRise = Animator.StringToHash("Default");
    private static readonly int FirstViolation = Animator.StringToHash("First Violation");
    private static readonly int SecondViolation = Animator.StringToHash("Second Violation");
    private static readonly int ThirdViolation = Animator.StringToHash("Third Violation");
    #endregion

    private int passengerCount;
    private int violationCount = 0;


    private void Start()
    {
        //StartCoroutine(AnimationTest());
    }

    public void passengerIngress()
    {
        passengerCount++;
        UpdatePassengerCount();
    }

    public void passengerEgress()
    {
        passengerCount--;
        UpdatePassengerCount();
    }

    #region HUD UI Update
    private void UpdatePassengerCount()
    {
        passengerCountText.text = Mathf.Clamp(passengerCount,0,999).ToString("##");
    }

    public void UpdateTimerDisplay(Component sender, object data)
    {
        float num = (float)data;

        int min = Mathf.Clamp((int)(num / 60), 0 , 99);
        int sec = (int)(num % 60);

        string format;

        if(min == 0 && sec <= 59)
        {
            format = num.ToString("00.00");
            gameTimerText.text = format;
            return;
        }

        format = min.ToString()+ ":" + sec.ToString();
        gameTimerText.text = format;

    }

    public void UpdatePointsDisplay(Component sender, object data)
    {
        float num = (float)data;

        pointsText.text = num.ToString("######.##") + " pts";
    }

    public void UpdateLocationSignageDisplay(Component sender, object data)
    {
        GameObject landmark = (GameObject)data;

        LandmarkText.text = landmark.name;
    }

    public void UpdateTicketViolationDisplay(Component sender, object data)
    {
        if (data.GetType() == typeof(float))
        {
            return;
        }


        string code = (string)data;
        string format;

        if (code == "na")
        {
            return;
        }

        violationCount++;

        if(code == "VIOLATION_01") //Running over pedestrian
        {
            format = "<=24 CHAR TITLE HERE" + "\n\n" +
                     "INSERT 135 CHARACTER DESCRIPTION HERE";

            violationTicket(format);
            violationTriggerAnimation();
        }
        else if (code == "VIOLATION_02") //crash into building
        {
            format = "<=24 CHAR TITLE HERE" + "\n\n" +
                     "INSERT 135 CHARACTER DESCRIPTION HERE";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_03") //Hitting another car
        {
            format = "<=24 CHAR TITLE HERE" + "\n\n" +
                     "INSERT 135 CHARACTER DESCRIPTION HERE";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_04") //Counterflowing
        {
            format = "<=24 CHAR TITLE HERE" + "\n\n" +
                     "INSERT 135 CHARACTER DESCRIPTION HERE";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_05") //Hitting a sign/post/traffic object
        {
            format = "<=24 CHAR TITLE HERE" + "\n\n" +
                     "INSERT 135 CHARACTER DESCRIPTION HERE";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_06") //Running the red light
        {
            format = "<=24 CHAR TITLE HERE" + "\n\n" +
                     "INSERT 135 CHARACTER DESCRIPTION HERE";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_07") // Speeding
        {
            format = "<=24 CHAR TITLE HERE" + "\n\n" +
                     "INSERT 135 CHARACTER DESCRIPTION HERE";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_08") // Blocking/Obstructing Traffic/Stalling/AFK
        {
            format = "<=24 CHAR TITLE HERE" + "\n\n" +
                     "INSERT 135 CHARACTER DESCRIPTION HERE";

            violationTicket(format);
            violationTriggerAnimation();

        }

    }

    public void UpdateSpeedDisplay(Component sender, object data)
    {
        float num = float.Parse((string)data);

        string format = "Speed: "+ num.ToString("000.00")+"kmph";

        SpeedText.text = format;
    }
    #endregion

    #region Violation Types
    private void violationTicket(string format)
    {
        ViolationPopUpTicketText.text = format;

        switch (violationCount)
        {
            case 1:
                Violation1.text = format;
                break;

            case 2:
                Violation2.text = format;
                break;

            case 3:
                Violation3.text = format;
                break;
        }
    }

    private void violationTriggerAnimation()
    {
        switch (violationCount)
        {
            case 1:
                ViolationsAnimator.CrossFade(FirstViolation, 0f);
                break;

            case 2:
                ViolationsAnimator.CrossFade(SecondViolation, 0f);
                break;

            case 3:
                ViolationsAnimator.CrossFade(ThirdViolation, 0f);
                break;
        }
    }
    #endregion

    #region Animation Functions
    public void TriggerLocationSignageAnimation()
    {
        StartCoroutine(PlayLocationSignageAnimationCoroutine());
    }

    IEnumerator PlayLocationSignageAnimationCoroutine()
    {
        LocationAnimator.CrossFade(SignageDrop, SignageTransition);
        yield return new WaitForSeconds(SignageStayTime);
        LocationAnimator.CrossFade(SignageRise, SignageTransition);
    }
    #endregion



    #region Debug (Comment out or delete entire region once done)
    IEnumerator AnimationTest()
    {
        PassengerCountAnimator.CrossFade(ParaPoFadeIn, ParaPoTextFadeInTransition);
        yield return new WaitForSeconds(ParaPoTextStayTime);
        PassengerCountAnimator.CrossFade(ParaPoFadeOut, ParaPoTextFadeOutTransition);
        yield return new WaitForSeconds(2f);
        PassengerCountAnimator.CrossFade(ParaPoFadeIn, ParaPoTextFadeInTransition);
        yield return new WaitForSeconds(ParaPoTextStayTime);
        PassengerCountAnimator.CrossFade(ParaPoFadeOut, ParaPoTextFadeOutTransition);
        yield return new WaitForSeconds(1f);

        LocationAnimator.CrossFade(SignageDrop, SignageTransition);
        yield return new WaitForSeconds(SignageStayTime);
        LocationAnimator.CrossFade(SignageRise, SignageTransition);
        yield return new WaitForSeconds(2f);
        LocationAnimator.CrossFade(SignageDrop, SignageTransition);
        yield return new WaitForSeconds(SignageStayTime);
        LocationAnimator.CrossFade(SignageRise, SignageTransition);
        yield return new WaitForSeconds(1f);

        ViolationsAnimator.CrossFade(FirstViolation, 0f);
        yield return new WaitForSeconds(5f);
        ViolationsAnimator.CrossFade(SecondViolation, 0f);
        yield return new WaitForSeconds(5f);
        ViolationsAnimator.CrossFade(ThirdViolation, 0f);
        yield return new WaitForSeconds(5f);

    }
    #endregion
}
