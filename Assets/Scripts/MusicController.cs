using System.Collections;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource _audioSource;
    public float introOffset;
    public float outroOffset;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.time = 0;
        _audioSource.Play();

    }

    void Update()
    {
        if (_audioSource.time >= outroOffset)
        {
            _audioSource.time = introOffset;
        }
    }
    
    public void FadeOut(float fadeOutTime)
    {
        if(_audioSource!=null && _audioSource.gameObject.activeSelf)
            StartCoroutine(FadeOutCoroutine(fadeOutTime));
    }

    private IEnumerator FadeOutCoroutine(float fadeOutTime)
    {
        float startVolume = _audioSource.volume;

        while (_audioSource.volume > 0)
        {
            _audioSource.volume -= startVolume * Time.deltaTime / fadeOutTime;
            
            yield return null;
        }

        _audioSource.Stop();
        _audioSource.volume = startVolume;
    }
}
