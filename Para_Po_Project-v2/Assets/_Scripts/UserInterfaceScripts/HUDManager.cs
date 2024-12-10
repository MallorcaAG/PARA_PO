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
            format = "<=Article 365 of R.A No. 4136" + "\n\n" +
                     "Article 365 of the Revised Penal Code covers charges of Homicide Through Reckless Imprudence for traffic violations that result in fatalities";

            violationTicket(format);
            violationTriggerAnimation();
        }
        else if (code == "VIOLATION_02") //crash into building
        {
            format = "<=Article 365 of R.A No. 4136" + "\n\n" +
                     "This article addresses reckless or negligent driving that results in damage to property or injury to people. Penalties are based on the severity of the injuries or whether the act resulted in death.";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_03") //Hitting another car
        {
            format = "<=Article 365 of R.A No. 4136" + "\n\n" +
                     "This article addresses reckless or negligent driving that results in damage to property or injury to people.";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_04") //Counterflowing
        {
            format = "<=R.A 4136" + "\n\n" +
                     "Restrictions on overtaking and passing. - The driver of a vehicle shall not drive to the left side of the center line of a highway in overtaking or passing another vehicle proceeding in the same direction, unless such left side is clearly visible, and is free of oncoming traffic for a sufficient distance ahead to permit such overtaking or passing to be made in safety.";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_05") //Hitting a sign/post/traffic object
        {
            format = "<=R.A. 4136, Section 48" + "\n\n" +
                     "Reckless driving. - No person shall operate a motor vehicle on any highway recklessly or without reasonable caution considering the width, traffic, grades, crossing, curvatures, visibility and other conditions of the highway and the conditions of the atmosphere and weather, or so as to endanger the property or the safety or rights of any person or so as to cause excessive or unreasonable damage to the highway.";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_06") //Running the red light
        {
            format = "<=R.A. 4136, Section 48" + "\n\n" +
                     "Reckless driving. - No person shall operate a motor vehicle on any highway recklessly or without reasonable caution considering the width, traffic, grades, crossing, curvatures, visibility and other conditions of the highway and the conditions of the atmosphere and weather, or so as to endanger the property or the safety or rights of any person or so as to cause excessive or unreasonable damage to the highway.";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_07") // Speeding
        {
            format = "<=R.A. 4136, Section 35" + "\n\n" +
                     "Any person driving a motor vehicle on a highway shall drive the same at a careful and prudent speed, not greater nor less than is reasonable and proper, having due regard for the traffic, the width of the highway, and of any other condition then and there existing";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_08") // Blocking/Obstructing Traffic/Stalling/AFK
        {
            format = "<=R.A 4136 Section 54" + "\n\n" +
                     "Obstruction of traffic. - No person shall drive his motor vehicle in such a manner as to obstruct or impede the passage of any vehicle, nor, while discharging or taking on passengers or loading or unloading freight, obstruct the free passage of other vehicles on the highway.";

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
