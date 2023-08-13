using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum State
    {
        Ready,
        Playing,
        GameOver,
    }
    private State state;
    public event EventHandler<State> OnStateChanged;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        
    }
    // Güncel state i güncelleyen fonksiyon
    public void ChangeState(State _state)
    {
        state = _state;
        OnStateChanged?.Invoke(this, state);
    }
    public State GetState()
    {
        return state;
    }
}
