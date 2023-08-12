using UnityEngine;

public class StickController : MonoBehaviour
{
    [SerializeField] private StickAnimationManager stickAnimationManager;
    [SerializeField] private float touchSensitivity = 10f;
    private Vector2 touchStartedPos;
    private Vector2 touchEndedPos;
    private Vector2 lastTouchPos;
    private float stickPosValue = 0f;
    private bool isBallThrowed = false;
    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, GameManager.State e)
    {
        switch (e)
        {
            case GameManager.State.Ready:
                isBallThrowed = false; break;
            case GameManager.State.Playing: 
                isBallThrowed = true; break;
        }
    }

    private void Update()
    {
        if (isBallThrowed) { return; }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStartedPos = touch.position;
                lastTouchPos = touchStartedPos;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                touchEndedPos = touch.position;
                stickPosValue += CalculateMoveDirection(lastTouchPos, touchEndedPos);
                stickPosValue = ClampValue(stickPosValue, 0f, 1f);
                stickAnimationManager.PlayAt(stickPosValue);
                lastTouchPos = touchEndedPos;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                stickAnimationManager.ReleaseStick();
                BallController.Instance.ThrowBall(stickPosValue);
                stickPosValue = 0;
            }
        }
    }
    private float CalculateMoveDirection(Vector2 start, Vector2 end)
    {
        float distance = Mathf.Abs(end.x - start.x);
        if (distance < touchSensitivity)
        {
            return 0f; // Hareket yok
        }
        else
        {
            return Mathf.Sign(end.x - start.x) > 0 ? -.1f : .1f; // Sa�a (+1) veya sola (-1) hareket
        }
    }
    private float ClampValue(float value, float min, float max)
    {
        return Mathf.Clamp(value, min, max);
    }
}
