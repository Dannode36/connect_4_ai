using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;

public class MenuHandler : MonoBehaviour
{
    public GameObject MenuCanvas;
    public GameObject GameOptionsCanvas;
    public GameObject SettingsCanvas;
    public TMP_InputField FirstNameInput;
    public TMP_InputField SecondNameInput;

    Stack<GameObject> previousMenu = new Stack<GameObject>();

    public void SetGamemode(bool single)
    {
        GameManager.single = single;
    }

    public void LoadMainScene()
    {
        SceneManager.LoadSceneAsync(1);
        print(FirstNameInput.text);
        GameManager.PlayerOneName = FirstNameInput.text;
        GameManager.PlayerTwoName = SecondNameInput.text;
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void PreLoadGame(GameObject activeCanvas)
    {
        SecondNameInput.interactable = !GameManager.single;
        previousMenu.Push(activeCanvas);
        activeCanvas.SetActive(false);
        GameOptionsCanvas.SetActive(true);

    }

    public void EnterSettingsMenu(GameObject activeCanvas)
    {
        previousMenu.Push(activeCanvas);
        activeCanvas.SetActive(false);
        SettingsCanvas.SetActive(true);
    }

    public void GoBackMenu(GameObject activeCanvas)
    {
        activeCanvas.SetActive(false);
        previousMenu.Pop().SetActive(true);
    }
}
