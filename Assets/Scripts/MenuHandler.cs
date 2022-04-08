using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;
public class MenuHandler : MonoBehaviour
{
    //var Render = GraphicsSettings.renderPipelineAsset;

    public GameObject MenuCanvas;
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
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadSceneAsync(0);
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
        rpa.
    }
    public void ChangeShadowQuality(int i)
    {

    }
}
