using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour
{
    public EscapeMenu esc;
    public GameObject fog;
    public GameObject rain;
    public GameObject thunder;

    public float environmentLightMultiplierBad = 0.3f;
    public float environmentLightMultiplierNormal = 1f;

    private WeibullDistribution _lifeTime;
    private int _currentWeatherIndex = -1;
    private GameObject[] weatherObjects;

    // Start is called before the first frame update
    void Start()
    {
        _lifeTime = new WeibullDistribution(0, 35, 12);
        weatherObjects = new GameObject[3];
        weatherObjects[0] = fog;
        weatherObjects[1] = rain;
        weatherObjects[2] = thunder;
        StartCoroutine(HandleWeather());
    }

    private IEnumerator HandleWeather()
    {
        while (!esc.IsGameOver)
        {
            float time = _lifeTime.Get();
            int i = Random.Range(0, 4);
            Debug.Log("chose weather " + i + " for " + time + " seconds");
            ActivateWeatherByIndex(i);
            yield return new WaitForSeconds(time);
        }
    }

    //0-fog, 1-rain, 2-thunder, 3-sunny
    private void ActivateWeatherByIndex(int i)
    {
        if (i == _currentWeatherIndex) return;
        if (_currentWeatherIndex is >= 0 and < 3)
            DeactivateWeatherByIndex(_currentWeatherIndex);
        switch (i)
        {
            case < 3:
                RenderSettings.ambientIntensity = environmentLightMultiplierBad;
                weatherObjects[i].gameObject.SetActive(true);
                weatherObjects[i].GetComponent<ParticleSystem>().Play();
                if (i!=0)
                    weatherObjects[i].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                break;
            case 3:
                RenderSettings.ambientIntensity = environmentLightMultiplierNormal;
                break;
        }

        _currentWeatherIndex = i;
    }

    private void DeactivateWeatherByIndex(int i)
    {
        weatherObjects[i].GetComponent<ParticleSystem>().Stop();
        if (i!=0)
            weatherObjects[i].transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        weatherObjects[i].gameObject.SetActive(false);
    }
}