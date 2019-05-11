using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomResolution : MonoBehaviour
{
    [SerializeField]
    float divider = 2f;

    void Awake()
    {
        #if UNITY_STANDALONE
        int width = (int)(720 / divider);
        int height = (int)(1280 / divider);
        bool isFullScreen = false;
        int desiredFPS = 60;

        Screen.SetResolution(width, height, isFullScreen, desiredFPS);
        #endif
    }
}
