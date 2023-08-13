using TMPro;
using UnityEngine;

public class FpsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsTxt;
    private float deltaTime;

    void Update()
    {
        // Calculating fps value
        deltaTime += (Time.deltaTime - deltaTime) * .1f;
        float fps = 1.0f / deltaTime;
        fpsTxt.text = "FPS: " +  Mathf.Ceil(fps).ToString();
    }
}
