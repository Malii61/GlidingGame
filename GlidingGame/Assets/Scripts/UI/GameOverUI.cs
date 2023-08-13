using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button tryAgainBtn; // Reference to the "Try Again" button

    private void Awake()
    {
        // Add a click event listener to the "Try Again" button
        tryAgainBtn.onClick.AddListener(() => GameManager.Instance.ChangeState(GameManager.State.Ready));
    }

    private void Start()
    {
        // Subscribe to the game state change event and set the UI inactive initially
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        gameObject.SetActive(false);
    }

    private void GameManager_OnStateChanged(object sender, GameManager.State e)
    {
        // Handle game state changes to show or hide the game over UI
        switch (e)
        {
            case GameManager.State.Ready:
                // Hide the game over UI when the game state changes to ready
                gameObject.SetActive(false);
                break;

            case GameManager.State.GameOver:
                // Show the game over UI when the game state changes to game over
                gameObject.SetActive(true);
                break;
        }
    }
}
