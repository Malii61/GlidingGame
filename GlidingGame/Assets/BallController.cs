using System;
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
    [SerializeField] private float bendingForceMultiplier;
    private Rigidbody rb;

    //gravity
    [SerializeField] private float ballGravityScale;
    [SerializeField] private float wingedGravityScale;
    private float globalGravity = -9.81f;

    private bool isFirstTouch = true;
    [SerializeField] private float rotationSensitivity = 3f;
    public float maxRotation = 50.0f;
    public float rotationSpeed = 1.0f;
    public float moveSpeed = 5.0f;
    public float rotationSpeedDivider = 10.0f; // Ekran genişliğine bölün
    private Vector2 touchStartPos;
    private float initialRotationZ;
    [SerializeField] private float smoothness = 5f;

    [SerializeField] private float wingedZSpeed;
    private float previousRotationAmount;
    private float adjustedRotationAmount;

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
        stickBendingForce = stickBendingForce <= 0.1f ? bendingForceMultiplier / 10f : stickBendingForce * bendingForceMultiplier;

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
                    float adjustedWingedZSpeed = (rb.velocity.z < wingedZSpeed && rb.velocity.z > wingedZSpeed / 2f) ? rb.velocity.z : wingedZSpeed;
                    rb.velocity = new Vector3(rb.velocity.x, 0, adjustedWingedZSpeed);
                    touchStartPos = touch.position;
                    initialRotationZ = 0;
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                float touchDelta = touch.position.x - touchStartPos.x;
                float rotationAmount = touchDelta / Screen.width * rotationSpeedDivider;
                adjustedRotationAmount = rotationAmount - previousRotationAmount;
                previousRotationAmount = rotationAmount;
                float clampedRotationAmount = Mathf.Clamp(rotationAmount, -maxRotation, maxRotation);
                initialRotationZ += clampedRotationAmount;
                transform.rotation = Quaternion.Euler(0, 0, -initialRotationZ);

                touchStartPos = touch.position;
                // Adjust velocity based on rotation
                float targetVelocityX = Mathf.Sin(clampedRotationAmount * Mathf.Deg2Rad) * moveSpeed;
                Vector3 targetVelocity = new Vector3(targetVelocityX, rb.velocity.y, rb.velocity.z);
                rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * smoothness);

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
                transform.rotation = Quaternion.identity;
                //rb.velocity = currentSpeed;
            }
        }
    }
    private void RotateAndMoveBird()
    {
        Quaternion rotation = transform.rotation;
        float angleZ = rotation.eulerAngles.z;

        float targetVelocityX = -Mathf.Sign(Mathf.Sin(angleZ * Mathf.Deg2Rad)) * moveSpeed;

        // Daha smooth bir geçiş sağlamak için Lerp kullanıyoruz
        Vector3 targetVelocity = new Vector3(targetVelocityX, rb.velocity.y, rb.velocity.z);
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * smoothness);

    }

    void LateUpdate()
    {
        float gravityScale = shape == Shape.Ball ? ballGravityScale : wingedGravityScale;
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Impulse);

        float speedMultiplier = rb.velocity.magnitude / 100f;
        if (GameManager.Instance.GetState() != GameManager.State.GameOver && speedMultiplier < 1f)
            speedMultiplier = 1f;
        ballAnimationManager.SetBallRotateSpeedMultiplier(speedMultiplier);

        if (isStickTracked)
        {
            // Eğer topu daha fırlatmadıysak top çubuğun üst konumunu takip eder.
            transform.position = Vector3.Lerp(transform.position, StickTopTransform.position, Time.deltaTime * delayTimer);
        }
    }

}
