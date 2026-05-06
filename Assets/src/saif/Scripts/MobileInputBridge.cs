using UnityEngine;
using Saif.GamePlay;

public class MobileInputBridge : MonoBehaviour
{
    private FishingHook hook;
    private Animator playerAnimator;
    private Transform playerTransform;
    private SpriteRenderer playerSprite;
    private PlayerMovement playerMovement;

    public float playerMoveSpeed = 5f;
    private float moveDirection = 0f;

    private FishingHook GetHook()
    {
        hook = Object.FindFirstObjectByType<FishingHook>();
        return hook;
    }

    private void FindPlayer()
    {
        if (playerTransform != null) return;
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerAnimator  = player.GetComponent<Animator>();
            playerSprite    = player.GetComponent<SpriteRenderer>();
            playerMovement  = player.GetComponent<PlayerMovement>();
        }
    }

    private bool IsCasting()
    {
        FindPlayer();
        return playerAnimator != null && playerAnimator.GetBool("IsCasting");
    }

    void Update()
    {
        if (moveDirection == 0f) return;

        FindPlayer();
        if (playerTransform == null) return;

        if (!IsCasting())
        {
            playerTransform.Translate(Vector3.right * moveDirection * playerMoveSpeed * Time.deltaTime);

            // Flip sprite based on direction we told it to go — not based on position change
            // moveDirection < 0 means left → flip = true (facing left)
            // moveDirection > 0 means right → flip = false (facing right)
            if (playerSprite != null)
                playerSprite.flipX = moveDirection < 0;

            if (playerAnimator != null)
                playerAnimator.SetBool("IsWalking", true);

            GetHook()?.MobileSetHorizontal(0f);
        }
        else
        {
            GetHook()?.MobileSetHorizontal(moveDirection);
        }
    }

    // ── LEFT BUTTON ───────────────────────────────────────────────────────────
    public void OnLeftPress()
    {
        moveDirection = -1f;
        FindPlayer();
        if (playerMovement != null) playerMovement.enabled = false;
    }

    public void OnLeftRelease()
    {
        moveDirection = 0f;
        FindPlayer();
        if (playerAnimator != null) playerAnimator.SetBool("IsWalking", false);
        GetHook()?.MobileSetHorizontal(0f);
        if (playerMovement != null) playerMovement.enabled = true;
    }

    // ── RIGHT BUTTON ──────────────────────────────────────────────────────────
    public void OnRightPress()
    {
        moveDirection = 1f;
        FindPlayer();
        if (playerMovement != null) playerMovement.enabled = false;
    }

    public void OnRightRelease()
    {
        moveDirection = 0f;
        FindPlayer();
        if (playerAnimator != null) playerAnimator.SetBool("IsWalking", false);
        GetHook()?.MobileSetHorizontal(0f);
        if (playerMovement != null) playerMovement.enabled = true;
    }

    // ── CAST / REEL BUTTON ────────────────────────────────────────────────────
    public void OnCastReelPress()
    {
        FindPlayer();
        if (playerAnimator != null && !playerAnimator.GetBool("IsCasting"))
            playerAnimator.SetBool("IsCasting", true);
        GetHook()?.MobileCastReelPress();
    }

    public void OnCastReelRelease()
    {
        GetHook()?.MobileCastReelRelease();
    }

    // ── STOP CASTING BUTTON ───────────────────────────────────────────────────
    public void OnStopCasting()
    {
        FindPlayer();
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsCasting", false);
            playerAnimator.SetTrigger("IsReeling 0");
        }
    }

    // ── TOOLBOX BUTTON ────────────────────────────────────────────────────────
    public void OnToggleToolbox()
    {
        HandleToolbox handler = Object.FindFirstObjectByType<HandleToolbox>();
        if (handler != null)
        {
            var currentState = handler.GetCurrentState();
            if (currentState is GameplayState)
                handler.OpenToolbox(handler.defaultToolbox);
            else
                handler.SetGameplayState();
        }
        else
        {
            Debug.LogWarning("MobileInputBridge: HandleToolbox not found in scene!");
        }
    }

    // ── HOOK SWAP BUTTONS ─────────────────────────────────────────────────────
    public void OnSelectSmallHook() => HookSelector.instance?.select_small_hook();
    public void OnSelectHeavyHook() => HookSelector.instance?.select_heavy_hook();
    public void OnToggleHook()      => HookSelector.instance?.toggle_hook();
}