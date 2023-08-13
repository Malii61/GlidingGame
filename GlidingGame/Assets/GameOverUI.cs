using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button tryAgainBtn;

    private void Awake()
    {
        tryAgainBtn.onClick.AddListener(() => GameManager.Instance.ChangeState(GameManager.State.Ready));
    }
    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        gameObject.SetActive(false);
    }

    private void GameManager_OnStateChanged(object sender, GameManager.State e)
    {
        switch (e)
        {
            case GameManager.State.Ready:    gameObject.SetActive(false); break;
            case GameManager.State.GameOver: gameObject.SetActive(true); break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
