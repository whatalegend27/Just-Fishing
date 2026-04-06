using UnityEngine;
using Saif.GamePlay;

public class MobileInputBridge : MonoBehaviour
{
    private FishingHook hook;
    private Animator playerAnimator;
    private Transform playerTransform;
    private SpriteRenderer playerSprite;

    public float playerMoveSpeed = 3f; // match your teammate's speed value

    private FishingHook GetHook()
    {
        if (hook == null)
            hook = FindFirstObjectByType<FishingHook>();
        return hook;
    }

    private void FindPlayer()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerAnimator = player.GetComponent<Animator>();
                playerSprite = player.GetComponent<SpriteRenderer>();
            }
        }
    }

    private bool IsCasting()
    {
        FindPlayer();
        return playerAnimator != null && playerAnimator.GetBool("IsCasting");
    }

    void Update()
    {
        // Only move player via mobile buttons when not casting
        // Keyboard movement is still handled by PlayerMovement script
    }

    // ─── LEFT BUTTON ─────────────────────────────────────────────────────────────
    public void OnLeftPress()
    {
        if (IsCasting())
        {
            // Move hook left
            GetHook()?.OnLeftPress();
        }
        else
        {
            // Move player left
            StartCoroutine(MovePlayer(-1f));
        }
    }

    public void OnLeftRelease()
    {
        GetHook()?.OnLeftRelease();
        StopAllCoroutines();

        // Reset walking animation
        if (playerAnimator != null && !IsCasting())
            playerAnimator.SetBool("IsWalking", false);
    }

    // ─── RIGHT BUTTON ────────────────────────────────────────────────────────────
    public void OnRightPress()
    {
        if (IsCasting())
        {
            // Move hook right
            GetHook()?.OnRightPress();
        }
        else
        {
            // Move player right
            StartCoroutine(MovePlayer(1f));
        }
    }

    public void OnRightRelease()
    {
        GetHook()?.OnRightRelease();
        StopAllCoroutines();

        // Reset walking animation
        if (playerAnimator != null && !IsCasting())
            playerAnimator.SetBool("IsWalking", false);
    }

    // Moves the player every frame while button is held
    private System.Collections.IEnumerator MovePlayer(float direction)
    {
        FindPlayer();
        while (true)
        {
            if (playerTransform != null && !IsCasting())
            {
                playerTransform.Translate(Vector3.right * direction * playerMoveSpeed * Time.deltaTime);

                // Flip sprite
                if (playerSprite != null)
                    playerSprite.flipX = direction < 0;

                // Set walking animation
                if (playerAnimator != null)
                    playerAnimator.SetBool("IsWalking", true);
            }
            yield return null;
        }
    }

    // ─── CAST/REEL BUTTON ────────────────────────────────────────────────────────
    public void OnCastReelPress()
    {
        Animator anim = playerAnimator;
        FindPlayer();
        anim = playerAnimator;

        if (anim != null && !anim.GetBool("IsCasting"))
            anim.SetBool("IsCasting", true);

        GetHook()?.OnCastReelPress();
    }

    public void OnCastReelRelease() { GetHook()?.OnCastReelRelease(); }

    // ─── STOP CASTING ────────────────────────────────────────────────────────────
    public void OnStopCasting()
    {
        FindPlayer();
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsCasting", false);
            playerAnimator.SetTrigger("IsReeling 0");
        }
    }
}