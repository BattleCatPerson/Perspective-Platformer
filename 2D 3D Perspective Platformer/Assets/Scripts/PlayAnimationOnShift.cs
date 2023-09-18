using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationOnShift : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string animationName;
    void Start()
    {
        GameManager.onShift += StartAnimation; 
    }

    void Update()
    {
        
    }

    public void StartAnimation()
    {
        animator.Play($"Base Layer.{animationName}");
    }
}
