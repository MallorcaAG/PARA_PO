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

    private void Update()
    {
        
    }

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
        string str = (string)data;

        LandmarkText.text = str;
    }

    //Violation Ticket Display
    //. . .
    //. . .
    //. . .

    public void UpdateSpeedDisplay(Component sender, object data)
    {
        float num = (float)data;

        string format = "Speed: "+ num.ToString("000.00")+"kmph";

        SpeedText.text = format;
    }


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
