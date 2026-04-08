using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    public HealthStats healthStats;
    public ArrestStats arrestStats;

    [Header("Bar GameObjects (2D squares in scene)")]
    public Transform healthBarFill;
    public Transform hungerBarFill;
    public Transform riskBarFill;

    [Header("Labels (optional)")]
    public Text healthLabel;
    public Text hungerLabel;
    public Text riskLabel;

    private int m_CachedHealth;
    private int m_CachedHunger;
    private int m_CachedRisk;

    private float m_HealthBarLeft, m_HealthBarScaleX, m_HealthBarWorldWidth;
    private float m_HungerBarLeft, m_HungerBarScaleX, m_HungerBarWorldWidth;
    private float m_RiskBarLeft,   m_RiskBarScaleX,   m_RiskBarWorldWidth;

    void Start()
    {
        CacheBarOrigin(healthBarFill, out m_HealthBarLeft, out m_HealthBarScaleX, out m_HealthBarWorldWidth);
        CacheBarOrigin(hungerBarFill, out m_HungerBarLeft, out m_HungerBarScaleX, out m_HungerBarWorldWidth);
        CacheBarOrigin(riskBarFill,   out m_RiskBarLeft,   out m_RiskBarScaleX,   out m_RiskBarWorldWidth);

        if (healthStats != null)
        {
            m_CachedHealth = healthStats.healthVal;
            m_CachedHunger = healthStats.hungerVal;
        }
        if (arrestStats != null)
            m_CachedRisk = arrestStats.riskVal;

        RefreshUI();
    }

    // Records the bar's left edge, original localScale.x, and original world width
    void CacheBarOrigin(Transform fill, out float leftEdge, out float originalScaleX, out float worldWidth)
    {
        if (fill != null)
        {
            originalScaleX = fill.localScale.x;
            // Use the renderer bounds to get true world width
            Renderer r = fill.GetComponent<Renderer>();
            worldWidth = r != null ? r.bounds.size.x : originalScaleX;
            leftEdge   = fill.position.x - worldWidth / 2f;
        }
        else
        {
            originalScaleX = 1f;
            worldWidth     = 1f;
            leftEdge       = 0f;
        }
    }

    void Update()
    {
        bool changed = false;

        if (healthStats != null)
        {
            if (healthStats.healthVal != m_CachedHealth)
            {
                m_CachedHealth = healthStats.healthVal;
                changed = true;
            }
            if (healthStats.hungerVal != m_CachedHunger)
            {
                m_CachedHunger = healthStats.hungerVal;
                changed = true;
            }
        }
        if (arrestStats != null && arrestStats.riskVal != m_CachedRisk)
        {
            m_CachedRisk = arrestStats.riskVal;
            changed = true;
        }

        if (changed)
            RefreshUI();
    }

    void RefreshUI()
    {
        SetBar(healthBarFill, m_HealthBarLeft, m_HealthBarScaleX, m_HealthBarWorldWidth, healthLabel, m_CachedHealth);
        SetBar(hungerBarFill, m_HungerBarLeft, m_HungerBarScaleX, m_HungerBarWorldWidth, hungerLabel, m_CachedHunger);
        SetBar(riskBarFill,   m_RiskBarLeft,   m_RiskBarScaleX,   m_RiskBarWorldWidth,   riskLabel,   m_CachedRisk);
    }

    void SetBar(Transform fill, float leftEdge, float originalScaleX, float worldWidth, Text label, int value)
    {
        if (fill != null)
        {
            float t = Mathf.Clamp01(value / 100f);

            // Scale proportionally from the original scale
            Vector3 s = fill.localScale;
            s.x = originalScaleX * t;
            fill.localScale = s;

            // Pin the left edge by shifting position right as bar shrinks
            Vector3 p = fill.position;
            p.x = leftEdge + worldWidth * t / 2f;
            fill.position = p;
        }

        if (label != null)
            label.text = value.ToString();
    }
}
