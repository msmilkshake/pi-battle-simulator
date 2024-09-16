using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeSinceLevelLoad : MonoBehaviour
{
    public TMP_Text timeText;
    public EscapeMenu menu;

    void Update()
    {
        if (!menu.IsGameOver)
        {
            float timeSinceLoad = Time.timeSinceLevelLoad;
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeSinceLoad);
            timeText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }
}