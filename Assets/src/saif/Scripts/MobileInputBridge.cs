using UnityEngine; // Standard Unity engine library
using Saif.GamePlay; // Accessing the custom fishing logic namespace
using System.Collections; // Required for Coroutines (smooth movement over time)

public class MobileInputBridge : MonoBehaviour
{
    // ─── VARIABLES ──────────────────────────────────────────────────────────────
    private FishingHook hook; // Reference to the active hook in the water
    private Animator playerAnimator; // Reference to the player's animation controller
    private Transform playerTransform; // Reference to the player's position
    private SpriteRenderer playerSprite; // Used to flip the player sprite left/right
    private HandleToolbox toolboxHandler; // Reference to teammate's toolbox script

    public float playerMoveSpeed = 3f; // Variable to control walking speed from Inspector

    private Coroutine moveCoroutine; // Stores the active movement routine so we can stop it

    // ─── HELPERS ────────────────────────────────────────────────────────────────

    // Dynamically finds the hook. Important because HookSelector swaps hook objects.
    private FishingHook GetHook()
    {
        // Finds the script in the scene every time we need it to avoid Null errors
        hook = Object.FindFirstObjectByType<FishingHook>();
        return hook; // Return the found hook
    }

    // Locates the player and its components if they haven't been found yet
    private void FindPlayer()
    {
        if (playerTransform != null) return; // Skip if we already have the reference
        GameObject player = GameObject.FindWithTag("Player"); // Search by tag
        if (player != null)
        {
            playerTransform = player.transform; // Save position reference
            playerAnimator  = player.GetComponent<Animator>(); // Save animation reference
            playerSprite    = player.GetComponent<SpriteRenderer>(); // Save visual reference
        }
    }

    // Checks the Animator to see if the player is currently in the fishing state
    private bool IsCasting()
    {
        FindPlayer(); // Ensure we have the animator reference
        // Returns true if the "IsCasting" parameter in the Animator is checked
        return playerAnimator != null && playerAnimator.GetBool("IsCasting");
    }

    // ── LEFT BUTTON ───────────────────────────────────────────────────────────
    // Triggered when the user touches the Left UI Button
    public void OnLeftPress()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine); // Stop any existing movement
        moveCoroutine = StartCoroutine(MovePlayer(-1f)); // Start moving left (-1 direction)
    }

    // Triggered when the user lets go of the Left UI Button
    public void OnLeftRelease()
    {
        // Kill the movement loop
        if (moveCoroutine != null) { StopCoroutine(moveCoroutine); moveCoroutine = null; }
        FindPlayer(); // Ensure reference
        if (playerAnimator != null) playerAnimator.SetBool("IsWalking", false); // Stop walk animation
        GetHook()?.MobileSetHorizontal(0f); // Tell the hook to stop moving sideways
    }

    // ── RIGHT BUTTON ──────────────────────────────────────────────────────────
    // Triggered when the user touches the Right UI Button
    public void OnRightPress()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine); // Stop existing movement
        moveCoroutine = StartCoroutine(MovePlayer(1f)); // Start moving right (1 direction)
    }

    // Triggered when the user lets go of the Right UI Button
    public void OnRightRelease()
    {
        // Kill the movement loop
        if (moveCoroutine != null) { StopCoroutine(moveCoroutine); moveCoroutine = null; }
        FindPlayer(); // Ensure reference
        if (playerAnimator != null) playerAnimator.SetBool("IsWalking", false); // Stop walk animation
        GetHook()?.MobileSetHorizontal(0f); // Tell the hook to stop moving sideways
    }

    // The core movement logic that runs every frame while a button is held
    private IEnumerator MovePlayer(float direction)
    {
        FindPlayer(); // Get player references
        while (true) // Loop until the coroutine is manually stopped
        {
            if (playerTransform != null)
            {
                // DECISION LOGIC: Are we walking or moving the hook?
                if (!IsCasting())
                {
                    // STATE: Walking on the pier
                    playerTransform.Translate(Vector3.right * direction * playerMoveSpeed * Time.deltaTime);
                    if (playerSprite  != null) playerSprite.flipX = direction < 0; // Flip sprite based on direction
                    if (playerAnimator != null) playerAnimator.SetBool("IsWalking", true); // Play animation
                    GetHook()?.MobileSetHorizontal(0f); // Ensure hook stays still while walking
                }
                else
                {
                    // STATE: Hook is in water, move the hook instead of the player
                    GetHook()?.MobileSetHorizontal(direction); // Send direction to hook script
                }
            }
            yield return null; // Wait for the next frame
        }
    }

    // ── CAST / REEL BUTTON ────────────────────────────────────────────────────
    public void OnCastReelPress()
    {
        FindPlayer(); // Get animator
        // If we aren't casting yet, start the animation
        if (playerAnimator != null && !playerAnimator.GetBool("IsCasting"))
            playerAnimator.SetBool("IsCasting", true);

        GetHook()?.MobileCastReelPress(); // Trigger the actual hook physics/casting
    }

    public void OnCastReelRelease()
    {
        GetHook()?.MobileCastReelRelease(); // Stop reeling or finalize cast
    }

    // ── STOP CASTING BUTTON ───────────────────────────────────────────────────
    public void OnStopCasting()
    {
        FindPlayer(); // Get references
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsCasting", false); // Switch animator back to "Idle/Walk"
            playerAnimator.SetTrigger("IsReeling 0"); // Play the reel-in animation finish
        }
    }

    // ── TOOLBOX BUTTON ────────────────────────────────────────────────────────
    /* ORAL EXAM ANSWER: "This is where I integrated my UI with my teammate's 
       State Pattern. My code detects if we are in the 'GameplayState' or 
       'ToolboxState' and calls her specific methods to toggle the UI." */
    public void OnToggleToolbox()
    {
        // 1. Find the teammate's manager script
        HandleToolbox handler = Object.FindFirstObjectByType<HandleToolbox>();
        
        if (handler != null)
        {
            // 2. Query the current state from her manager
            var currentState = handler.GetCurrentState();

            // 3. Logic Toggle: If playing, open toolbox. If in toolbox, go back to play.
            if (currentState is GameplayState) // Uses 'is' keyword for clean type checking
            {
                // Uses her 'defaultToolbox' variable she added for mobile compatibility
                handler.OpenToolbox(handler.defaultToolbox);
            }
            else
            {
                // Returns the game to the Gameplay State
                handler.SetGameplayState();
            }
        }
        else
        {
            // Error handling for debugging
            Debug.LogWarning("MobileInputBridge: HandleToolbox script not found in scene!");
        }
    }

    // ── HOOK SWAP BUTTONS ─────────────────────────────────────────────────────
    // These link to the Singleton Instance of the HookSelector manager
    public void OnSelectSmallHook() => HookSelector.instance?.select_small_hook();
    public void OnSelectHeavyHook() => HookSelector.instance?.select_heavy_hook();
    public void OnToggleHook()      => HookSelector.instance?.toggle_hook();
}