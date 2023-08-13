using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera startCam; // Reference to the start camera
    [SerializeField] private CinemachineVirtualCamera followBallCam; // Reference to the follow ball camera
    private int higherPriority = 11; // Higher priority value for setting camera priority
    private int lowerPriority = 9; // Lower priority value for setting camera priority

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to the game state change event
        GameManager.Instance.OnStateChanged += CameraManager_OnStateChanged;
    }

    private void CameraManager_OnStateChanged(object sender, GameManager.State e)
    {
        // Handle camera priority based on game state changes
        switch (e)
        {
            case GameManager.State.Ready:
                // Set camera priorities for the "Ready" state
                startCam.Priority = higherPriority; // Start camera has higher priority
                followBallCam.Priority = lowerPriority; // Follow ball camera has lower priority
                break;

            case GameManager.State.Playing:
                // Set camera priorities for the "Playing" state
                followBallCam.Priority = higherPriority; // Follow ball camera has higher priority
                startCam.Priority = lowerPriority; // Start camera has lower priority
                break;

            case GameManager.State.GameOver:
                // No camera priority changes needed for the "GameOver" state
                break;
        }
    }
}
