using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAnimationPlayer : MonoBehaviour
{
    [System.Serializable]
    public class LevelAnimation
    {
        public string levelName;          
        public Animation animationComponent;  
        public string clipName;                
    }

    [Tooltip("Assign one animation per level (Level 1 to Level 21)")]
    [SerializeField] private LevelAnimation[] levelAnimations = new LevelAnimation[21];

    private int currentLevelIndex;

    void Start()
    {
         CheckValidIndex();
    }

    private void Update()
    {
        if(currentLevelIndex == DataManager.Instance.CurrentIterator)
        {
            return;
        }

        CheckValidIndex();
    }

    private void CheckValidIndex()
    {
        currentLevelIndex = DataManager.Instance.CurrentIterator;

        if (currentLevelIndex < 0 || currentLevelIndex >= levelAnimations.Length)
        {
            Debug.LogWarning($"CurrentIterator {currentLevelIndex} is out of bounds");
            return;
        }

        PlayLevelAnimation(levelAnimations[currentLevelIndex]);
    }

    private void PlayLevelAnimation(LevelAnimation levelAnim)
    {
        if (levelAnim.animationComponent != null && !string.IsNullOrEmpty(levelAnim.clipName))
        {
            levelAnim.animationComponent.Play(levelAnim.clipName);
            Debug.Log($"Playing clip {levelAnim.clipName} on {levelAnim.animationComponent.gameObject.name} for {levelAnim.levelName}");
        }
        else
        {
            Debug.LogWarning($"Animation component or clipName missing for {levelAnim.levelName}");
        }
    }
}
