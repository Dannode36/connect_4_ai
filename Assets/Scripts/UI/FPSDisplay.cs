using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    public TMP_Text fpsTxt;

    public float pollingTime = 1f;
    float time;
    int frameCount;

    void Update()
    {
        time += Time.deltaTime;
        frameCount++;
        if (time > pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);
            fpsTxt.text = frameRate.ToString();
            time -= pollingTime;
            frameCount = 0;
        }
    }
}