using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public enum Shape
    {
        Ball,
        Winged,
    }

    public static BallController Instance { get; private set; }

    [SerializeField] private BallAnimationManager ballAnimationManager;
    [SerializeField] private Transform StickTopTransform;
    [SerializeField] private float delayTimer = 30f;
    private bool isStickTracked = true;
    private Shape shape = Shape.Ball;

    //rigidbody
    [SerializeField] private Vector3 netForce;
    private Rigidbody rb;

    //gravity
    [SerializeField] private float ballGravityScale;
    [SerializeField] private float wingedGravityScale;
    private float globalGravity = -9.81f;

    private bool isFirstTouch = true;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }
    private void GameManager_OnStateChanged(object sender, GameManager.State e)
    {
        switch (e)
        {
            case GameManager.State.Ready:
                isStickTracked = true;
                rb.isKinematic = true;
                isFirstTouch = true;
                break;

            case GameManager.State.Playing:
                isStickTracked = false;
                rb.isKinematic = false;
                break;
        }
    }
    public void ThrowBall(float stickBendingForce)
    {
        Debug.Log(stickBendingForce);
        stickBendingForce = stickBendingForce <= 0.1f ? 100f : stickBendingForce * 1000;

        GameManager.Instance.ChangeState(GameManager.State.Playing);
        rb.AddForce(netForce + (netForce * stickBendingForce), ForceMode.Impulse);
        ballAnimationManager.Play(BallAnimationManager.AnimationState.RotateBall);
    }
    private void Update()
    {
        if (GameManager.Instance.GetState() != GameManager.State.Playing) { return; }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (shape == Shape.Ball && !isFirstTouch)
                {
                    ballAnimationManager.Play(BallAnimationManager.AnimationState.OpenWings);
                    shape = Shape.Winged;
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                //Kanatla hareket kodu

            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (isFirstTouch)
                {
                    isFirstTouch = false;
                    return;
                }
                ballAnimationManager.Play(BallAnimationManager.AnimationState.CloseWings);
                shape = Shape.Ball;
            }
        }
    }
    private void FixedUpdate()
    {
        float gravityScale = shape == Shape.Ball ? ballGravityScale : wingedGravityScale;
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Impulse);


        float speedMultiplier = rb.velocity.magnitude / 100f;
        if (GameManager.Instance.GetState() != GameManager.State.GameOver && speedMultiplier < 1f)
            speedMultiplier = 1f;
        ballAnimationManager.SetBallRotateSpeedMultiplier(speedMultiplier);
    }
    void LateUpdate()
    {
        if (isStickTracked)
        {
            // Eðer topu daha fýrlatmadýysak top çubuðun üst konumunu takip eder.
            transform.position = Vector3.Lerp(transform.position, StickTopTransform.position, Time.deltaTime * delayTimer);
        }
    }

}
