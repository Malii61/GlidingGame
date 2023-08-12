using System.Collections;
using UnityEngine;

public class BallAnimationManager : MonoBehaviour
{
    public enum AnimationState
    {
        RotateBall,
        OpenWings,
        CloseWings,
    }
    [SerializeField] private float speedChangerValue = .8f;
    private const string ROTATE_BALL = "RotateBall";
    private const string OPEN_WINGS = "Armature|1_Open_wings_2";
    private const string CLOSE_WINGS = "Armature|2_Close_wings";

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Play(AnimationState state)
    {
        switch (state)
        {
            case AnimationState.RotateBall:
                animator.Play(ROTATE_BALL);
                break;
            case AnimationState.OpenWings:
                if (animator.speed <= 0)
                {
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
        }
    }

    public void OnClosedWings()
    {
        Play(AnimationState.RotateBall);
    }
    public void ChangeAnimatorSpeed(bool isFaster)
    {
        float changeValue = isFaster ? speedChangerValue : -speedChangerValue;
        StartCoroutine(ChangeSpeedNumerator(changeValue));
    }
    public void SetBallRotateSpeedMultiplier(float multiplier)
    {
        animator.SetFloat("Speed", multiplier);
    }

    private IEnumerator ChangeSpeedNumerator(float value)
    {
        while (true)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            animator.speed += value;

            if (value < 0 && animator.speed > 0f)
                continue;

            Play(AnimationState.OpenWings);
            break;
        }

    }
}
