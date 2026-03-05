using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed  = 5f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private Vector2 movement;

    private Vector2 screenBounds;

    private float playerHalfWidth;

    private float xPosLastFrame;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        playerHalfWidth = spriteRenderer.bounds.extents.x;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        ClampMovement();
        FlipCharacterX();
    }

    void FlipCharacterX(){
        if (transform.position.x > xPosLastFrame){
            spriteRenderer.flipX = false;
        }
        else if (transform.position.x < xPosLastFrame){
            spriteRenderer.flipX = true;
        }
        xPosLastFrame = transform.position.x;
    }

    private void ClampMovement(){

    }

    private void HandleMovement(){
        bool IsCasting = animator.GetBool("IsCasting");
        float input = Input.GetAxis("Horizontal");
        if (!IsCasting){
            movement.x = input * speed * Time.deltaTime;
            transform.Translate(movement);
        }
        if (input != 0){
            animator.SetBool("IsWalking", true);
        } else{
            animator.SetBool("IsWalking", false);
        }
        if (Input.GetKeyDown(KeyCode.X)){
            animator.SetBool("IsCasting", true);
        }
        else if (Input.GetKeyDown(KeyCode.R)){
            animator.SetBool("IsCasting", false);
            animator.SetTrigger("IsReeling 0");
        }
    }
}
