using System.Collections;
using UnityEngine;

public class BallAnimationManager : MonoBehaviour
{
    public enum AnimationState
    {
        Idle,
        RotateBall,
        OpenWings,
        CloseWings,
    }
    [SerializeField] private float speedChangerValue = .8f;
    private const string IDLE = "Idle";
    private const string ROTATE_BALL = "RotateBall";
    private const string OPEN_WINGS = "Armature|1_Open_wings_2";
    private const string CLOSE_WINGS = "Armature|2_Close_wings";

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, GameManager.State e)
    {
        // Set the initial animation state to Idle when the game state changes to ready
        if (e == GameManager.State.Ready)
        {
            Play(AnimationState.Idle);
        }
    }

    public void Play(AnimationState state)
    {
        switch (state)
        {
            case AnimationState.RotateBall:
                animator.Play(ROTATE_BALL);
                break;

            case AnimationState.OpenWings:
                // Play the open wings animation, or adjust animator speed if already playing
                if (animator.speed <= 0)
                {
                    transform.localRotation = Quaternion.identity;
                    animator.speed = 1;
                    animator.Play(OPEN_WINGS);
                }
                else
                {
                    ChangeAnimatorSpeed(isFaster: false);
                }
                break;

            case AnimationState.CloseWings:
                animator.Play(CLOSE_WINGS);
                break;

            case AnimationState.Idle:
                animator.Play(IDLE);
                break;
        }
    }

    public void ChangeAnimatorSpeed(bool isFaster)
    {
        // Change the animator speed to achieve faster or slower animation playback
        float changeValue = isFaster ? speedChangerValue : -speedChangerValue;
        StartCoroutine(ChangeSpeedNumerator(changeValue));
    }

    public void SetBallRotateSpeedMultiplier(float multiplier)
    {
        // Set the animator parameter "Speed" to control the ball rotation speed
        animator.SetFloat("Speed", multiplier);
    }

    private IEnumerator ChangeSpeedNumerator(float value)
    {
        // Coroutine to gradually change the animator speed
        while (true)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            animator.speed += value;

            // Continue adjusting speed if slowing down, otherwise switch to OpenWings
            if (value < 0 && animator.speed > 0f)
                continue;

            Play(AnimationState.OpenWings);
            break;
        }
    }
}
