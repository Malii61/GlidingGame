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
    private bool isWingsClosed = false;
    private bool isClosing;

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
                    if (isClosing)
                    {
                        isClosing = false;
                        Debug.Log("KAPATILDI DÖNÜLDÜ");
                        return;
                    }
                    animator.Play(OPEN_WINGS);
                }
                else
                {
                    ChangeAnimatorSpeed(isFaster: false);
                }
                break;
            case AnimationState.CloseWings:
                isClosing = true;
                Debug.Log("close wings oynat");
                animator.Play(CLOSE_WINGS);
                break;
        }
    }
    public void OnClosedWings()
    {
        Debug.Log("on closed wings");
        animator.speed = 0f;
        ChangeAnimatorSpeed(isFaster: true);
        isClosing = false;
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

            if ((value < 0 && animator.speed > 0f) || (value > 0 && animator.speed < 1f))
                continue;


            AnimationState state = animator.speed >= 1 ? AnimationState.RotateBall : AnimationState.OpenWings;
            animator.speed = animator.speed > 1 ? 1 : (animator.speed < 0 ? 0 : animator.speed);
            Play(state);
            break;
        }

    }
}
