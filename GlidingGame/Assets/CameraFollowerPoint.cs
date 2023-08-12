using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowerPoint : MonoBehaviour
{
    private Quaternion firstRotation;
    private bool isPositionLocked;
    private Vector3 lockedPosition;
    private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        firstRotation = transform.rotation;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, GameManager.State e)
    {
        switch (e)
        {
            case GameManager.State.Ready:
                transform.position = startPosition;
                isPositionLocked = false; break;
            case GameManager.State.GameOver: 
                lockedPosition = transform.position;
                isPositionLocked = true;
                break;
        }
    }
    private void LateUpdate()
    {
        transform.rotation = firstRotation;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isPositionLocked)
        {
            transform.position = lockedPosition;
        }
    }
}
