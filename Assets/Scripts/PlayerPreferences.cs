using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerPreferences : MonoBehaviour
{
    private string _team;

    private int _defaultSoldiers=40;

    private int _defaultSpawnrate=1;
    // Start is called before the first frame update
    void Start()
    {
        _team = tag;
        PlayerPrefs.SetInt(_team+"soldiers",_defaultSoldiers);
        PlayerPrefs.SetInt(_team+"spawnrate",_defaultSpawnrate);
        //Panel -> RespawnRate -> Dropdown
        transform.GetChild(2).GetComponentInChildren<TMP_Dropdown>().value=_defaultSpawnrate;
        //Panel -> NrSoldiers -> Input -> TextArea -> Text
        transform.GetChild(1).GetChild(1).GetChild(0).GetComponentInChildren<TMP_Text>().text=_defaultSoldiers.ToString();
    }

    public void SetNumberOfSoldiers()
    {
        string inputVal = transform.GetChild(1).GetComponentInChildren<TMP_InputField>().text;
        int n = inputVal != "" ? int.Parse(inputVal) : 0;
        PlayerPrefs.SetInt(_team+"soldiers",n);
    }

    public void SetRespawnRate(int nr)
    {
        PlayerPrefs.SetInt(_team+"spawnrate",nr);
    }
}
