using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int targetFPS = 60;
    void Awake()
    {
        //Gemini taught me this code
        QualitySettings.vSyncCount = 0; // Disable VSync
        Application.targetFrameRate = targetFPS; // Set frame rate
    }
}
