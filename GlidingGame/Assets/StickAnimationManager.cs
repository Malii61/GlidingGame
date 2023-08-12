using System;
using UnityEngine;

public class StickAnimationManager : MonoBehaviour
{
    private const string BEND_STICK = "Armature|Bend_Stick";
    private const string RELEASE_STICK = "Armature|Release_Stick";
    private Animator animator;
    private float playFrameTimer;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAt(float _time)
    {
        playFrameTimer = _time;
        // �stenilen frame den animasyonu oynat
        animator.Play(BEND_STICK, 0, playFrameTimer);
    }
    public void ReleaseStick()
    {
        float playTime = CalculateReleasePosition();
        animator.Play(RELEASE_STICK, 0, playTime);
    }

    private float CalculateReleasePosition()
    {
        // �ubu�un dik oldu�u pozisyon animasyonun %23 �ne denk geldi�i i�in bu y�zdeyle �arp�yoruz.
        return (1 - playFrameTimer) * 23 / 100;
    }
}
