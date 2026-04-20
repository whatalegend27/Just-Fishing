using UnityEngine;
using Saif.GamePlay;
using System.Collections;

public class MobileInputBridge : MonoBehaviour
{
    private FishingHook hook;
    private Animator playerAnimator;
    private Transform playerTransform;
    private SpriteRenderer playerSprite;
    private HandleToolbox toolboxHandler;

    public float playerMoveSpeed = 3f;

    private Coroutine moveCoroutine;

    private FishingHook GetHook()
    {
        // Always search fresh — HookSelector destroys and respawns the hook on swap
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
        }
    }

    private bool IsCasting()
    {
        FindPlayer();
        return playerAnimator != null && playerAnimator.GetBool("IsCasting");
    }

    // ── LEFT BUTTON ───────────────────────────────────────────────────────────
    // Canvas: EventTrigger → PointerDown → OnLeftPress
    //                        PointerUp   → OnLeftRelease
    public void OnLeftPress()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MovePlayer(-1f));
    }

    public void OnLeftRelease()
    {
        if (moveCoroutine != null) { StopCoroutine(moveCoroutine); moveCoroutine = null; }
        FindPlayer();
        if (playerAnimator != null) playerAnimator.SetBool("IsWalking", false);
        GetHook()?.MobileSetHorizontal(0f);
    }

    // ── RIGHT BUTTON ──────────────────────────────────────────────────────────
    // Canvas: EventTrigger → PointerDown → OnRightPress
    //                        PointerUp   → OnRightRelease
    public void OnRightPress()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MovePlayer(1f));
    }

    public void OnRightRelease()
    {
        if (moveCoroutine != null) { StopCoroutine(moveCoroutine); moveCoroutine = null; }
        FindPlayer();
        if (playerAnimator != null) playerAnimator.SetBool("IsWalking", false);
        GetHook()?.MobileSetHorizontal(0f);
    }

    private IEnumerator MovePlayer(float direction)
    {
        FindPlayer();
        while (true)
        {
            if (playerTransform != null)
            {
                if (!IsCasting())
                {
                    // Not casting → move the player
                    playerTransform.Translate(Vector3.right * direction * playerMoveSpeed * Time.deltaTime);
                    if (playerSprite  != null) playerSprite.flipX = direction < 0;
                    if (playerAnimator != null) playerAnimator.SetBool("IsWalking", true);
                    GetHook()?.MobileSetHorizontal(0f);
                }
                else
                {
                    // Casting → move the hook sideways instead
                    GetHook()?.MobileSetHorizontal(direction);
                }
            }
            yield return null;
        }
    }

    // ── CAST / REEL BUTTON ────────────────────────────────────────────────────
    // One button — press starts cast, hold reels in, release lets canReel activate
    // Canvas: EventTrigger → PointerDown → OnCastReelPress
    //                        PointerUp   → OnCastReelRelease
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
    // Canvas: Button → OnClick → OnStopCasting
    public void OnStopCasting()
    {
        FindPlayer();
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsCasting", false);
            playerAnimator.SetTrigger("IsReeling 0"); // match your Animator trigger name exactly
        }
    }

    public void OnToggleToolbox()
    {
        // 1. Find her script in the scene
        HandleToolbox handler = Object.FindFirstObjectByType<HandleToolbox>();
        
        if (handler != null)
        {
            // 2. Check the current state manually
            // Since IToolboxState is the interface, we check what the instance is
            var currentState = handler.GetCurrentState();

            if (currentState.GetType().Name == "GameplayState")
            {
                // If we are playing, tell her script to switch to toolbox
                handler.SetToolboxState();
            }
            else
            {
                // If the toolbox is open, tell her script to go back to gameplay
                handler.SetGameplayState();
            }
        }
    }

    // ── HOOK SWAP BUTTONS ─────────────────────────────────────────────────────
    // Canvas: SmallHook Button → OnClick → OnSelectSmallHook
    //         HeavyHook Button → OnClick → OnSelectHeavyHook
    //         Toggle Button    → OnClick → OnToggleHook
    public void OnSelectSmallHook() => HookSelector.instance?.select_small_hook();
    public void OnSelectHeavyHook() => HookSelector.instance?.select_heavy_hook();
    public void OnToggleHook()      => HookSelector.instance?.toggle_hook();
}