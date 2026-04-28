using UnityEngine;
using Saif.GamePlay;

public class DemoController : MonoBehaviour
{
    public enum DemoMode
    {
        Off,
        CatchFish,
        NoCatchFish
    }

    [Header("Demo Settings")]
    public DemoMode mode = DemoMode.Off;

    [Header("References")]
    public Animator playerAnimator;

    [Header("Timing")]
    public float hookSearchInterval = 0.5f;
    public float castDelay = 0.7f;          // Must be greater than FishingHook castAnimationDelay
    public float sinkTime = 2.5f;           // Time allowed for hook to drop down
    public float noCatchWaitTime = 4f;      // In NoCatch mode, how long to wait before reeling
    public float waitBetweenCasts = 2f;

    [Header("Movement")]
    public float horizontalChangeInterval = 0.75f;

    private FishingHook fishingHook;
    private float timer;
    private float hookSearchTimer;
    private float horizontalTimer;
    private float currentHorizontal;

    private bool isCastingCycle;
    private bool pressedCast;
    private bool waitingForFish;
    private bool releasedToReel;

    void Update()
    {
        if (mode == DemoMode.Off)
        {
            StopCastingAnimation();
            return;
        }

        if (playerAnimator == null)
        {
            Debug.LogWarning("DemoController: playerAnimator is not assigned.");
            return;
        }

        TryFindHook();

        if (fishingHook == null)
            return;

        timer += Time.deltaTime;

        if (!isCastingCycle)
        {
            StartDemoCycle();
            return;
        }

        playerAnimator.SetBool("IsCasting", true);

        // Step 1: wait until the hook's internal cast animation delay has passed
        if (!pressedCast && timer >= castDelay)
        {
            fishingHook.MobileCastReelPress();
            pressedCast = true;
            Debug.Log("Demo: Cast started");
            return;
        }

        // Step 2: let the hook sink first
        if (pressedCast && !waitingForFish && !releasedToReel && timer >= sinkTime)
        {
            fishingHook.MobileCastReelRelease();
            waitingForFish = true;
            Debug.Log("Demo: Hook is now waiting for a fish");
            return;
        }

        // Step 3: while waiting, move around horizontally but DO NOT reel yet
        if (waitingForFish && !releasedToReel)
        {
            UpdateHorizontalMovement();

            // Catch mode: only start reeling after fish is attached to the hook
            if (mode == DemoMode.CatchFish)
            {
                if (HookHasCaughtFish())
                {
                    fishingHook.MobileCastReelPress();
                    releasedToReel = true;
                    waitingForFish = false;
                    Debug.Log("Demo: Fish caught, start reeling");
                    return;
                }
            }
            else if (mode == DemoMode.NoCatchFish)
            {
                // No-catch mode: wait a bit, then reel anyway
                if (timer >= sinkTime + noCatchWaitTime)
                {
                    fishingHook.MobileCastReelPress();
                    releasedToReel = true;
                    waitingForFish = false;
                    Debug.Log("Demo: NoCatch timeout reached, start reeling");
                    return;
                }
            }

            return;
        }

        // Step 4: reel upward once catch happened (or timeout in NoCatch mode)
        if (releasedToReel)
        {
            fishingHook.MobileCastReelPress();

            UpdateHorizontalMovement();

            if (!fishingHook.IsHookCast)
            {
                EndDemoCycle();
            }
        }
    }

    void TryFindHook()
    {
        if (fishingHook != null)
            return;

        hookSearchTimer -= Time.deltaTime;
        if (hookSearchTimer > 0f)
            return;

        hookSearchTimer = hookSearchInterval;

        FishingHook[] hooks = Object.FindObjectsByType<FishingHook>(FindObjectsSortMode.None);

        foreach (FishingHook hook in hooks)
        {
            if (hook != null && hook.gameObject.activeInHierarchy)
            {
                fishingHook = hook;
                Debug.Log("DemoController: Found hook -> " + hook.name);
                return;
            }
        }
    }

    void StartDemoCycle()
    {
        if (mode == DemoMode.NoCatchFish)
            DisableFishCatch();
        else
            EnableFishCatch();

        isCastingCycle = true;
        pressedCast = false;
        waitingForFish = false;
        releasedToReel = false;
        timer = 0f;
        horizontalTimer = 0f;
        currentHorizontal = 0f;

        playerAnimator.SetBool("IsCasting", true);
        fishingHook.MobileSetHorizontal(0f);
    }

    void EndDemoCycle()
    {
        fishingHook.MobileSetHorizontal(0f);

        isCastingCycle = false;
        pressedCast = false;
        waitingForFish = false;
        releasedToReel = false;
        timer = -waitBetweenCasts;
        horizontalTimer = 0f;
        currentHorizontal = 0f;

        playerAnimator.SetBool("IsCasting", false);
        fishingHook = null;

        Debug.Log("Demo: Cycle finished");
    }

    void StopCastingAnimation()
    {
        if (playerAnimator != null)
            playerAnimator.SetBool("IsCasting", false);
    }

    void UpdateHorizontalMovement()
    {
        horizontalTimer -= Time.deltaTime;

        if (horizontalTimer <= 0f)
        {
            currentHorizontal = Random.Range(-1f, 1f);
            horizontalTimer = horizontalChangeInterval;
        }

        fishingHook.MobileSetHorizontal(currentHorizontal);
    }

    bool HookHasCaughtFish()
    {
        if (fishingHook == null)
            return false;

        Transform hookTransform = fishingHook.transform;

        for (int i = 0; i < hookTransform.childCount; i++)
        {
            Transform child = hookTransform.GetChild(i);

            if (child != null && child.CompareTag("Fish"))
                return true;
        }

        return false;
    }

    void DisableFishCatch()
    {
        FishToInventory[] fish = Object.FindObjectsByType<FishToInventory>(FindObjectsSortMode.None);

        foreach (FishToInventory f in fish)
        {
            if (f != null)
                f.enabled = false;
        }
    }

    void EnableFishCatch()
    {
        FishToInventory[] fish = Object.FindObjectsByType<FishToInventory>(FindObjectsSortMode.None);

        foreach (FishToInventory f in fish)
        {
            if (f != null)
                f.enabled = true;
        }
    }
}