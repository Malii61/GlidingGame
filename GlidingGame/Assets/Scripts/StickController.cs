using UnityEngine;
using UnityEngine.EventSystems;

public class StickController : MonoBehaviour
{
    [SerializeField] private StickAnimationManager stickAnimationManager; // Reference to the StickAnimationManager
    [SerializeField] private float touchSensitivity = 10f; // Sensitivity for touch movement
    private Vector2 touchStartedPos; // Starting position of touch
    private Vector2 touchEndedPos; // Ending position of touch
    private Vector2 lastTouchPos; // Last recorded touch position
    private float stickPosValue = 0f; // Current stick position value (0 to 1)
    private bool isBallThrowed = false; // Flag to track if the ball is thrown

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged; // Subscribe to the game state change event
    }

    private void GameManager_OnStateChanged(object sender, GameManager.State e)
    {
        // Handle game state changes
        switch (e)
        {
            case GameManager.State.Ready:
                isBallThrowed = false; // Reset the ball thrown flag when the game state changes to ready
                break;

            case GameManager.State.Playing:
                isBallThrowed = true; // Set the ball thrown flag when the game state changes to playing
                break;
        }
    }

    private void Update()
    {
        if (isBallThrowed) { return; } // Do nothing if the ball is thrown

#if UNITY_EDITOR
        // Handle touch input for non-editor platforms
        if (EventSystem.current.currentSelectedGameObject != null) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            touchStartedPos = Input.mousePosition;
            lastTouchPos = touchStartedPos;
        }
        else if (Input.GetMouseButton(0))
        {
            touchEndedPos = Input.mousePosition;
            stickPosValue += CalculateMoveDirection(lastTouchPos, touchEndedPos);
            stickPosValue = ClampValue(stickPosValue, 0f, 1f);
            stickAnimationManager.PlayAt(stickPosValue);
            lastTouchPos = touchEndedPos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            stickAnimationManager.ReleaseStick();
            BallController.Instance.ThrowBall(stickPosValue);
            stickPosValue = 0;
        }
#else
        // Handle touch input for the editor
        if (Input.touchCount > 0)
        {
            if (EventSystem.current.currentSelectedGameObject != null) { return; }

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
#endif
    }

    private float CalculateMoveDirection(Vector2 start, Vector2 end)
    {
        float distance = Mathf.Abs(end.x - start.x);
        if (distance < touchSensitivity)
        {
            return 0f; // No movement
        }
        else
        {
            return Mathf.Sign(end.x - start.x) > 0 ? -0.1f : 0.1f; // Move to the right (+1) or left (-1)
        }
    }

    private float ClampValue(float value, float min, float max)
    {
        return Mathf.Clamp(value, min, max);
    }
}
