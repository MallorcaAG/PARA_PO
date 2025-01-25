using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private bool getInput = true;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameEvent onTimeScaleChange;

    private bool paused = false;

    private void Start()
    {
        Unpause();
    }

    private void Update()
    {
        if(getInput)
        {
            handleInput();
        }
    }

    public void deactivateInput()
    {
        gameObject.SetActive(false);
        getInput = false;
    }

    public void Unpause()
    {
        paused = false;
        panel.SetActive(false);
        onTimeScaleChange.Raise(this, 1f);
    }

    public void Pause()
    {
        paused = true;
        panel.SetActive(true);
        onTimeScaleChange.Raise(this, 0f);
    }

    private void handleInput()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!paused)
            {
                Pause();

                return;
            }

            Unpause();
        }
    }
}
