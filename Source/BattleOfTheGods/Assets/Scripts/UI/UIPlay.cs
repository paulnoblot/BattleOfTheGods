using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIPlay : MonoBehaviour
{
    [SerializeField] GameObject buttonModel = null;
    [SerializeField] Transform content = null;


    void Start()
    {
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                GameObject newButtonZone = Instantiate(buttonModel, content);
                int id = i;
                int lvl = ((i - 1) * 5 + j + 1);
                newButtonZone.transform.GetChild(0).GetComponent<Text>().text = "Zone " + id;
                newButtonZone.transform.GetChild(1).GetComponent<Text>().text = "lvl " + lvl;
                newButtonZone.GetComponent<Button>().onClick.AddListener(() => { DataManager.singleton.stageLevel = lvl; SceneManager.LoadScene(id); });
            }
        } 
    }
}
