using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    
    [SerializeField] private CinemachineVirtualCamera startCam;
    [SerializeField] private CinemachineVirtualCamera followBallCam;
    private int higherPriority = 11;
    private int lowerPriority = 9;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnStateChanged += CameraManager_OnStateChanged;
    }

    private void CameraManager_OnStateChanged(object sender, GameManager.State e)
    {
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
            case GameManager.State.GameOver:
                break;
        }
    }

}
