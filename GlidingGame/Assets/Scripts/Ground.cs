using UnityEngine;

public class Ground : MonoBehaviour
{
    private bool isCheckable = true;
    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, GameManager.State e)
    {
        switch(e)
        {
            case GameManager.State.Playing:
                isCheckable = true; break;
            case GameManager.State.GameOver:
                isCheckable = false; break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isCheckable && collision.transform.TryGetComponent(out BallController ballController))
        {
            Debug.Log("game over");
            GameManager.Instance.ChangeState(GameManager.State.GameOver);
        }
    }
}
