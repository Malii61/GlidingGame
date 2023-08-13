using UnityEngine;

public class CameraFollowerPoint : MonoBehaviour
{
    [SerializeField] private Transform ballTransform; // Reference to the ball's transform
    private Vector3 lockedPosition; // The position where the camera is locked when game over
    private Quaternion lockedRotation; // The rotation where the camera is locked when game over
    private bool isGameOver; // Flag to track if the game is over

    private void Update()
    {
        // Update the camera's position based on the ball's position, unless the game is over
        transform.position = isGameOver ? lockedPosition : ballTransform.position;
    }

    private void LateUpdate()
    {
        // Calculate the camera's rotation based on the ball's rotation, but only if the game is not over
        Quaternion rotation = !isGameOver ? Quaternion.Euler(transform.rotation.eulerAngles.x, ballTransform.rotation.eulerAngles.y, 0f) : lockedRotation;

        // Smoothly interpolate the camera's rotation towards the calculated rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 3f);
    }

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
                isGameOver = false; // Reset the game over flag when the game state changes to ready
                break;

            case GameManager.State.GameOver:
                // Lock the camera's position and rotation when the game state changes to game over
                lockedPosition = transform.position;
                lockedRotation = transform.rotation;
                isGameOver = true; // Set the game over flag
                break;
        }
    }
}
