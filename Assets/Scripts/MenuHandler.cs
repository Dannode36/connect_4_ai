using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
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
}
