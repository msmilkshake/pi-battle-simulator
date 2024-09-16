using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Script to handle the main menu
public class MainMenu : MonoBehaviour
{
    public GameObject blackscreen;
    public GameObject settingsMenu;
    public MusicController musicController1;
    public MusicController musicController2;
    public void ChangeToMainLevel()
    {
        StartCoroutine(FadeBlackScreen("Scenes/Main"));
        GetComponentInParent<CanvasGroup>().interactable = false;
    }

    public void ChangeToStartMenu()
    {
        StartCoroutine(FadeBlackScreen("Scenes/StartMenu"));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    

    private IEnumerator FadeBlackScreen(string sceneName)
    {
        yield return new WaitForSeconds(0.4f);
        blackscreen.SetActive(true);
        CanvasGroup cg = blackscreen.GetComponent<CanvasGroup>();
        musicController1.FadeOut(3f);
        if(musicController2!=null)
            musicController2.FadeOut(3f);
        float fadeTime = 4f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
            // Debug.Log(cg.alpha);
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }
}
