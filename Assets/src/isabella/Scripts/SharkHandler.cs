using UnityEngine;

public class SharkHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Animator animator;

    void mouseDown()
    {
        animator.SetBool("IsBlushing", true);
    }
}
