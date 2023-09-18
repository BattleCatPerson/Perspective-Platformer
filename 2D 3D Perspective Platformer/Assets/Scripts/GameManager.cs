using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager currentGameManager;
    public bool victory;
    public bool shifted;

    public static Action onShift;
    public static Action onVictory;

    private void Awake()
    {
        onShift = null;
        onVictory = null;
        currentGameManager = this;
    }
}
