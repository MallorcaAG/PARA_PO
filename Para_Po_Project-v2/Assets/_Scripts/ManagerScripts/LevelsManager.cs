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
    [SerializeField] private Image[] map;
    [SerializeField] private Image[] landmarkImgUI;
    [SerializeField] private Image[] starsUI;
    [SerializeField] private RawImage VehiclesShowroomUI;
    [SerializeField] private GameObject leftArrowNav;
    [SerializeField] private GameObject rightArrowNav;
    [SerializeField] private TextMeshProUGUI playerBestScoreText;

    [Header("Asset References")]
    [SerializeField] private List<Levels> levels;
    [SerializeField] private List<RenderTexture> vehicles;
    [SerializeField] private Sprite BadStar;
    [SerializeField] private Sprite GoodStar;
    private List<Levels> unlockedLevels;
    private int i;
    private bool init = false;
    private DataManager dm;
    public List<Levels> Levels { get { return levels; } }

    private void Start()
    {
        dm = DataManager.Instance;

        populateUnlockedLevelsArray();

        i = 0;

        UpdateUI();

        init = true;

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if(init)
        {
            UpdateUI();
            return;
        }
    }

    private void populateUnlockedLevelsArray()
    {
        unlockedLevels = new List<Levels>();

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
        Debug.Log(unlockedLevels[i]);
        Levels currentLvl = unlockedLevels[i];

        levelNameText.text = currentLvl.name;
        routeNameText.text = currentLvl.RouteName;
        map[0].sprite = currentLvl.getRouteMap();
        map[1].sprite = currentLvl.getRouteLine();
        for (int j = 0; j < landmarkImgUI.Length; j++)
        {
            landmarkImgUI[j].sprite = currentLvl.RouteLandmarkImages[j];
        }
        resetStarDisplay();
        for(int j = 0; j < currentLvl.Stars; j++)
        {
            starsUI[j].sprite = GoodStar;
        }
        //TrikeShowroom, JeepneyShowroom, BusShowroom;
        switch(currentLvl.VehicleType)
        {
            case VehicleType.TRICYCLE:
                VehiclesShowroomUI.texture = vehicles[0];
                break;

            case VehicleType.JEEPNEY:
                VehiclesShowroomUI.texture = vehicles[1];
                break;

            case VehicleType.BUS:
                VehiclesShowroomUI.texture = vehicles[2];
                break;

            default:
                VehiclesShowroomUI.texture = vehicles[1];
                break;
        }
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
        UpdateUI();
    }
    public void RightArrowPressed()
    {
        i++;
        clampIterator();
        UpdateUI();
    }
    private void checkCurrentArrayPos()
    {
        if(i == 0)
        {
            if(unlockedLevels.Count == 1)
            {
                rightArrowNav.SetActive(false);
            }
            else
            {
                rightArrowNav.SetActive(true);
            }

            leftArrowNav.SetActive(false);

        }
        else if(i == unlockedLevels.Count - 1)
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

    public void LoadLevel()
    {
        Levels currentLvl = levels[i];

        dm.CurrentLevel = currentLvl;
        dm.CurrentIterator = i;
        if(!(i + 1 >  levels.Count - 1))
        {
            dm.NextLevel = levels[i + 1];
        }
        
            SceneLoadManager.Instance.LoadLevelsScenes(currentLvl, i);
    }
}
