using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    [SerializeField] private GameObject PARA_PO;
    [SerializeField] private Animator Para_PoAnimator;
    [SerializeField] private GameEvent onParaPo; 

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
    private static readonly int ParaPoFadeOut = Animator.StringToHash("ParaPo_FadeOut");
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

    public void PassengerCountChanged(Component sender, object data)
    {
        passengerCount = (int)data;

        Debug.LogWarning("Passenger count data received: "+passengerCount);

        UpdatePassengerCount();
    }

    #region HUD UI Update
    private void UpdatePassengerCount()
    {
        passengerCountText.text = Mathf.Clamp(passengerCount,0,999).ToString("00");
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

        format = min.ToString("00")+ ":" + sec.ToString("00");
        gameTimerText.text = format;

    }

    public void UpdatePointsDisplay(Component sender, object data)
    {
        float num = (float)data;

        pointsText.text = num.ToString("####0.##") + " pts";
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
                     "Reckless Imprudence - Ran over pedestrian";

            violationTicket(format);
            violationTriggerAnimation();
        }
        else if (code == "VIOLATION_02") //crash into building
        {
            format = "<=Article 365 of R.A No. 4136" + "\n\n" +
                     "Reckless Imprudence - Property Damage";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_03") //Hitting another car
        {
            format = "<=Article 365 of R.A No. 4136" + "\n\n" +
                     "Reckless Driving";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_04") //Counterflowing
        {
            format = "<=R.A 4136" + "\n\n" +
                     "Counterflowing";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_05") //Hitting a sign/post/traffic object
        {
            format = "<=R.A. 4136, Section 48" + "\n\n" +
                     "Reckless driving";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_06") //Running the red light
        {
            format = "<=R.A. 4136, Section 48" + "\n\n" +
                     "Beating the red light";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_07") // Speeding
        {
            format = "<=R.A. 4136, Section 35" + "\n\n" +
                     "Overspeeding";

            violationTicket(format);
            violationTriggerAnimation();

        }
        else if (code == "VIOLATION_08") // Blocking/Obstructing Traffic/Stalling/AFK
        {
            format = "<=R.A 4136 Section 54" + "\n\n" +
                     "Obstruction of traffic";

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

    public void TriggerParaPoAnimation()
    {
        if (!PARA_PO.activeSelf)
        {
            PARA_PO.SetActive(true); 
        }

        StartCoroutine(PlayParaPoAnimationCoroutine());
    }

    private IEnumerator PlayParaPoAnimationCoroutine()
    {
        Para_PoAnimator.CrossFade(ParaPoFadeIn, ParaPoTextFadeInTransition);
        yield return new WaitForSeconds(ParaPoTextStayTime);
        Para_PoAnimator.CrossFade(ParaPoFadeOut, ParaPoTextFadeOutTransition);
        yield return new WaitForSeconds(ParaPoTextFadeOutTransition); 
        PARA_PO.SetActive(false); 
    }

    public void TriggerPassengerCapacityIndicator(Component sender, object data)
    {
        if (!((bool)data)) return;
        if (passengerCountText.color == Color.red) return;
        StartCoroutine(IndicatePassengerCapacity());
    }

    private IEnumerator IndicatePassengerCapacity()
    {
        passengerCountText.color = Color.red;
        yield return new WaitForSeconds(1f);
        passengerCountText.color = Color.white;
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
