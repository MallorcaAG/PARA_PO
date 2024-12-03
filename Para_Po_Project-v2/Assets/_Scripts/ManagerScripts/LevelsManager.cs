using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelsManager : Singleton<LevelsManager>
{

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI routeNameText;
    [SerializeField] private Image map;
    [SerializeField] private Image[] landmarkImgUI;
    [SerializeField] private Image[] starsUI; 
    [SerializeField] private Camera TrikeShowroom, JeepneyShowroom, BusShowroom;
    [SerializeField] private GameObject leftArrowNav;
    [SerializeField] private GameObject rightArrowNav;
    [SerializeField] private TextMeshProUGUI playerBestScoreText;

    [Header("Asset References")]
    [SerializeField] private List<Levels> levels;
    [SerializeField] private Sprite BadStar;
    [SerializeField] private Sprite GoodStar;

    private List<Levels> unlockedLevels;
    private int i;

    private void Start()
    {
        populateUnlockedLevelsArray();
        
        i = 0;

        UpdateUI();
    }

    private void populateUnlockedLevelsArray()
    {
        foreach(Levels level in levels)
        {
            if(level.IsUnlocked)
            {
                unlockedLevels.Add(level);
            }
        }

        /*Debug*/if(unlockedLevels.Count == 0)
        {
            Debug.Log("bruh\nur shit is empty\n"+unlockedLevels+"\nCount: "+unlockedLevels.Count);
        }
    }

    public void UpdateUI()
    {
        Levels currentLvl = levels[i];

        levelNameText.text = currentLvl.name;
        routeNameText.text = currentLvl.RouteName;
        map.sprite = currentLvl.RouteMap;
        for(int j = 0; j < currentLvl.RouteLandmarkImages.Length; j++)
        {
            landmarkImgUI[j].sprite = currentLvl.RouteLandmarkImages[j];
        }
        resetStarDisplay();
        for(int j = 0; j < currentLvl.Stars; j++)
        {
            starsUI[j].sprite = GoodStar;
        }
        //TrikeShowroom, JeepneyShowroom, BusShowroom;
        checkCurrentArrayPos();
        playerBestScoreText.text = currentLvl.HighScore.ToString("F4");
    }

    private void resetStarDisplay()
    {
        for (int j = 0; j < starsUI.Length; j++)
        {
            starsUI[j].sprite = BadStar;
        }
    }

    #region Navigation
    public void LeftArrowPressed()
    {
        i--;
        clampIterator();
    }
    public void RightArrowPressed()
    {
        i++;
        clampIterator();
    }
    private void checkCurrentArrayPos()
    {
        if(i == 0)
        {
            leftArrowNav.SetActive(false);
            rightArrowNav.SetActive(true);

        }
        else if(i == unlockedLevels.Count)
        {
            leftArrowNav.SetActive(true);
            rightArrowNav.SetActive(false);
        }
        else
        {
            leftArrowNav.SetActive(true);
            rightArrowNav.SetActive(true);
        }
    }

    private void clampIterator()
    {
        if (i <= 0)
        {
            i = 0;
        }
        else if (i >= unlockedLevels.Count)
        {
            i = unlockedLevels.Count;
        }
    }
    #endregion

    
}
