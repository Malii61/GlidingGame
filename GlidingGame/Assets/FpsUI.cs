using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsTxt;
    private float deltaTime;
    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * .1f;
        float fps = 1.0f / deltaTime;
        fpsTxt.text = Mathf.Ceil(fps).ToString();
    }
}
