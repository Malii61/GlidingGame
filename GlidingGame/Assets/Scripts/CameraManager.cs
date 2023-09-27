using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera startCam; 
    [SerializeField] private CinemachineVirtualCamera followBallCam; 
    private readonly int higherPriority = 11; 
    private readonly int lowerPriority = 9; 

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
                startCam.Priority = higherPriority; 
                followBallCam.Priority = lowerPriority;
                break;

            case GameManager.State.Playing:
                followBallCam.Priority = higherPriority;
                startCam.Priority = lowerPriority;
                break;
        }
    }
}
