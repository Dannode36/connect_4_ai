using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;

public class MenuHandler : MonoBehaviour
{
    public GameObject MenuCanvas;
    public GameObject PreLoadCanvas;
    public TMP_InputField FirstNameInput;
    public TMP_InputField SecondNameInput;

    public GameObject SettingsCanvas;

    RenderPipelineAsset rpa;

    private void Start()
    {
        rpa = GraphicsSettings.renderPipelineAsset;
    }

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

    public void PreLoadGame()
    {
        MenuCanvas.SetActive(false);
        PreLoadCanvas.SetActive(true);

        SecondNameInput.interactable = !GameManager.single;
    }

    public void OpenSettings()
    {
        MenuCanvas.SetActive(false);
        SettingsCanvas.SetActive(true);
    }

    public void CloseSettings()
    {
        MenuCanvas.SetActive(true);
        SettingsCanvas.SetActive(false);
    }

    ////
    
    public void ChangeAntialisingMode(int val)
    {
        Debug.LogWarning("Num: " + val);
    }
    public void ChangeShadowQuality(int i)
    {

    }
}
