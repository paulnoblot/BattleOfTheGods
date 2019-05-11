using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject[] panels = null;

    [Header("Buttons")]
    [SerializeField] Button buttonplay = null;

    void Start()
    {
        foreach (int id in DataManager.singleton.GetTeam())
            if (id != 0)
            {
                buttonplay.interactable = true;
                return;
            }
        buttonplay.interactable = false;
    }

    public void GoToMainMenu()
    {
        panels[0].SetActive(true);
        for (int i = 1; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }

        foreach (int id in DataManager.singleton.GetTeam())
            if (id != 0)
            {
                buttonplay.interactable = true;
                return;
            }

        buttonplay.interactable = false;
    }

    public void GoToPanel(int _idPanel)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == _idPanel);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetPlayerPrefs()
    {
        DataManager.singleton.ResetPlayerPrefs();
    }
}
