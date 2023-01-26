using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject pauseMenu;
    public void StartLoading()
    {
        pauseButton.SetActive(false);
        pauseMenu.SetActive(false);
        this.gameObject.SetActive(true);
    }

    public void EndLoading()
    {
        pauseButton.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
