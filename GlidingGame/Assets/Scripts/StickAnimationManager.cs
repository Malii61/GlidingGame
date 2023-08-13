using System;
using UnityEngine;

public class StickAnimationManager : MonoBehaviour
{
    private const string BEND_STICK = "Armature|Bend_Stick"; // Animation state for bending the stick
    private const string RELEASE_STICK = "Armature|Release_Stick"; // Animation state for releasing the stick
    private Animator animator; // Reference to the Animator component
    private float playFrameTimer; // Timer to control the frame to play

    private void Awake()
    {
        animator = GetComponent<Animator>(); // Get the Animator component on Awake
    }

    public void PlayAt(float _time)
    {
        playFrameTimer = _time;
        // Play the animation from the desired frame
        animator.Play(BEND_STICK, 0, playFrameTimer);
    }

    public void ReleaseStick()
    {
        float playTime = CalculateReleasePosition(); // Calculate the release position
        animator.Play(RELEASE_STICK, 0, playTime); // Play the release stick animation at the calculated position
    }

    private float CalculateReleasePosition()
    {
        // Calculate the position in the release animation based on stick bending position
        // The stick's upright position corresponds to 23% of the animation, so we calculate accordingly
        return (1 - playFrameTimer) * 23 / 100;
    }
}
