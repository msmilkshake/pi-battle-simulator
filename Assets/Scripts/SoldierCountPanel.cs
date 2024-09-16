using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SoldierCountPanel : MonoBehaviour
{ 
    public Headquarters hq;
    public TMP_Text text;

    private void Start()
    {
        text.SetText("" + hq.maxNpcs);
    }

    void Update()
    {
        text.SetText("Soldiers Alive: " + hq.GetCurrentNumberOfSoldiers());
    }
}
