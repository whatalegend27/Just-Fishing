using System;
using UnityEngine;

public class SharkHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //[SerializeField] private Animator animator;

    void OnMouseDown()
    {
        Console.WriteLine("Flirt was clicked!");
        //animator.SetBool("IsBlushing", true);
    }
}
