using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI levelNumber, musicText;
    public GameObject fadeIn, fadeOut, tutorial;
    void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
            musicText.text = (AudioManager.instance.au.volume == 1) ? "Music: ON" : "Music: OFF";
        StartCoroutine(fadeOUT());
    }

    public void restart()
    {
        StartCoroutine(fadeIN(1));
    }

    public void home()
    {
        StartCoroutine(fadeIN(0));
    }

    public void ok()
    {
        tutorial.SetActive(false);
    }

    public void musicPlayer()
    {
        musicText.text = (AudioManager.instance.au.volume == 0) ? "Music: ON" : "Music: OFF";
        AudioManager.instance.au.volume = (AudioManager.instance.au.volume == 0) ? 1 : 0;
    }

    public void quit()
    {
        Application.Quit();
    }

    private IEnumerator fadeIN(int i)
    {
        fadeIn.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(i);
    }

    private IEnumerator fadeOUT()
    {
        yield return new WaitForSeconds(1f);
        fadeOut.SetActive(false);
    }
}
