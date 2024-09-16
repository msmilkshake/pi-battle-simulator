using UnityEngine;

public class EscapeMenu : MonoBehaviour
{
    public GameObject menu;
    private bool _isGameOver;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_isGameOver)
        {
            menu.SetActive(!menu.activeSelf);
        }
    }

    public void SetMenuStatus(bool status)
    {
        menu.SetActive(status);
    }

    public void SetGameOver()
    {
        _isGameOver = true;
        menu.SetActive(false);
    }

    public bool IsGameOver => _isGameOver;
}
