using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.ObjectModel;

public class CanvasManger : MonoBehaviour
{
    ReadOnlyCollection<string> winMessages = new(new List<string>()
    {
        "won against", 
        "destroyed", 
        "demolished", 
        "rekt", 
        "was victorious against", 
        "came first against", 
        "was smarter than", 
        "was better than",
        "owned",
        "beat"
    });

    public TMP_Text WinTitle;
    public TMP_Text TurnInfo;
    public GameObject ResetButton;

    public void DisplayTurnInfo(string text)
    {
        TurnInfo.text = text;
    }

    public void DisplayWinTitle(string winnerName, string looserName)
    {
        System.Random random = new System.Random();
        int index = random.Next(winMessages.Count);
        
        WinTitle.text = $"{winnerName} {winMessages[index]} {looserName}!";
    }

    public void DisplayWinText(string text)
    {
        WinTitle.text = $"{text}!";
    }

    public void ShowResetButton(bool active)
    {
        ResetButton.SetActive(active);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
