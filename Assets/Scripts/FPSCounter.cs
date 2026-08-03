using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    private TMP_Text fpsText;

    private float frameCount = 0;
    private float dt = 0.0f;
    private float fps = 0.0f;
    private float updateRate = 2.0f;

    private void Awake()
    {
        fpsText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        frameCount++;
        dt += Time.unscaledDeltaTime;

        if (dt > 1.0f / updateRate)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0f / updateRate;

            if (fpsText != null)
            {
                fpsText.text = "FPS: " + Mathf.RoundToInt(fps);
            }
        }
    }
}