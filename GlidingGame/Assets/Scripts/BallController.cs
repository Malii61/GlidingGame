﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

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
    [SerializeField] private float followStickDelayTimer = 30f; 
    private bool isStickTracked = true;
    private Shape shape = Shape.Ball; 

    // Rigidbody
    [SerializeField] private Vector3 netForce; 
    [SerializeField] private float bendingForceMultiplier; 
    private Rigidbody rb; 

    // Gravity
    [SerializeField] private float ballGravityScale; 
    [SerializeField] private float wingedGravityScale; 
    private float globalGravity = -9.81f; 

    // Rotate and movement
    [SerializeField] private float rotationSensitivity = 2f; 
    [SerializeField] private float velocitySensitivity = 2f; 
    [SerializeField] private float rotateYMultiplier; 
    public float maxRotation = 50.0f; 
    public float moveSpeed = 5.0f; 
    public float rotationSpeedDivider = 10.0f; 
    private Vector2 touchStartPos; 
    private float zRotation; 
    private float previousTouchDelta; 
    private bool isFirstMove;
    private Vector3 firstPos; 

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        firstPos = transform.position;
    }

    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged; // Subscribe to game state changes
    }

    private void GameManager_OnStateChanged(object sender, GameManager.State e)
    {
        // Handle game state changes
        switch (e)
        {
            case GameManager.State.Ready:
                isStickTracked = true;
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                transform.position = firstPos;
                transform.rotation = Quaternion.identity;
                zRotation = 0f;
                shape = Shape.Ball;
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
        stickBendingForce = stickBendingForce <= 0.1f ? bendingForceMultiplier / 10f : stickBendingForce * bendingForceMultiplier;

        GameManager.Instance.ChangeState(GameManager.State.Playing);
        rb.AddForce(netForce + (netForce * stickBendingForce), ForceMode.Impulse);
        ballAnimationManager.Play(BallAnimationManager.AnimationState.RotateBall);
    }

    private void Update()
    {
        if (GameManager.Instance.GetState() != GameManager.State.Playing) { return; }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            // Check if a UI element is clicked
            if (EventSystem.current.currentSelectedGameObject != null) { return; }

            if (shape == Shape.Ball)
            {
                ballAnimationManager.Play(BallAnimationManager.AnimationState.OpenWings);
                shape = Shape.Winged;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                touchStartPos = Input.mousePosition;
                previousTouchDelta = touchStartPos.x;
                isFirstMove = true;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            RotateBird(Input.mousePosition.x);
            MoveBird();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (shape == Shape.Winged)
            {
                ballAnimationManager.Play(BallAnimationManager.AnimationState.CloseWings);
                shape = Shape.Ball;
            }
        }
#else
        if (Input.touchCount > 0)
        {
            // Check if a UI element is touched
            if (EventSystem.current.currentSelectedGameObject != null) { return; }

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (shape == Shape.Ball)
                {
                    ballAnimationManager.Play(BallAnimationManager.AnimationState.OpenWings);
                    shape = Shape.Winged;
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    touchStartPos = touch.position;
                    previousTouchDelta = touchStartPos.x;
                    isFirstMove = true;
                }
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                RotateBird(touch.position.x);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (shape == Shape.Winged)
                {
                    ballAnimationManager.Play(BallAnimationManager.AnimationState.CloseWings);
                    shape = Shape.Ball;
                }
            }
            MoveBird();
        }
#endif
    }

    private void RotateBird(float currentTouchPosX)
    {
        float touchDelta = currentTouchPosX - touchStartPos.x;
        float rotationAmount = (touchDelta - previousTouchDelta) / Screen.width * rotationSpeedDivider;
        previousTouchDelta = touchDelta;

        if (isFirstMove)
        {
            isFirstMove = false;
            return;
        }

        // Calculate rotation amount and update Z rotation
        if (Math.Abs(rotationAmount) > maxRotation)
            return;
        zRotation += rotationAmount;
        Quaternion rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + (zRotation * rotateYMultiplier), -zRotation);
        // Rotate bird
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, rotation, Time.deltaTime * rotationSensitivity));

        if (Mathf.Abs(zRotation + rotationAmount) > Mathf.Abs(maxRotation))
            return;
    }

    private void MoveBird()
    {
        // Apply movement based on shape
        float yVelocity = shape == Shape.Winged ? globalGravity * wingedGravityScale : rb.velocity.y;

        // Change velocity by rotation
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(moveSpeed * transform.forward.x + (zRotation * 0.5f), yVelocity, moveSpeed * transform.right.x), Time.deltaTime * velocitySensitivity);
    }

    void LateUpdate()
    {
        if (isStickTracked)
        {
            // If the ball hasn't been thrown yet, follow the stick's top position with a delay
            transform.position = Vector3.Lerp(transform.position, StickTopTransform.position, Time.deltaTime * followStickDelayTimer);
        }
        else
        {
            if (shape == Shape.Ball)
            {
                // Apply gravity to the ball
                Vector3 gravity = globalGravity * ballGravityScale * Vector3.up;
                rb.AddForce(gravity, ForceMode.Force);
            }

            // Adjust animation speed based on ball's movement speed
            float speedMultiplier = rb.velocity.magnitude / 100f;
            if (GameManager.Instance.GetState() != GameManager.State.GameOver && speedMultiplier < 1f)
                speedMultiplier = 1f;
            ballAnimationManager.SetBallRotateSpeedMultiplier(speedMultiplier);
        }
    }

    public Shape GetCurrentShape()
    {
        return shape;
    }

    public void ChangeShape(Shape _shape)
    {
        // Change the shape of the bird
        if (shape == _shape) return;
        if (_shape == Shape.Ball)
        {
            ballAnimationManager.Play(BallAnimationManager.AnimationState.CloseWings);
            shape = Shape.Ball;
        }
    }
}
