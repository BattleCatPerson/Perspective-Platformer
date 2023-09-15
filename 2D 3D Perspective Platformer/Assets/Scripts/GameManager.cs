using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager currentGameManager;
    public bool victory;

    public static Action onShift;
    public static Action onVictory;

    private void Awake()
    {
        currentGameManager = this;
    }
}
