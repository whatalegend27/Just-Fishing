using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    public HealthStats healthStats;
    public ArrestStats arrestStats;

    [Header("Bar Fill Rectangles")]
    public RectTransform healthBarFill;
    public RectTransform hungerBarFill;
    public RectTransform riskBarFill;

    [Header("Bar Background Widths (pixels)")]
    public float healthBarMaxWidth = 200f;
    public float hungerBarMaxWidth = 200f;
    public float riskBarMaxWidth   = 200f;

    [Header("Labels (optional)")]
    public Text healthLabel;
    public Text hungerLabel;
    public Text riskLabel;

    private int m_CachedExhaustion;
    private int m_CachedHunger;
    private int m_CachedRisk;

    void Start()
    {
        if (healthStats != null)
        {
            m_CachedExhaustion = healthStats.healthVal;
            m_CachedHunger     = healthStats.hungerVal;
        }
        if (arrestStats != null)
            m_CachedRisk = arrestStats.riskVal;

        RefreshUI();
    }

    void Update()
    {
        bool changed = false;

        if (healthStats != null)
        {
            if (healthStats.healthVal != m_CachedExhaustion)
            {
                m_CachedExhaustion = healthStats.healthVal;
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
        Debug.Log($"[UIUpdater] RefreshUI — health:{m_CachedExhaustion} hunger:{m_CachedHunger} risk:{m_CachedRisk}");
        SetBar(healthBarFill, healthBarMaxWidth, healthLabel, m_CachedExhaustion);
        SetBar(hungerBarFill, hungerBarMaxWidth, hungerLabel, m_CachedHunger);
        SetBar(riskBarFill,   riskBarMaxWidth,   riskLabel,   m_CachedRisk);
    }

    void SetBar(RectTransform fill, float maxWidth, Text label, int value)
    {
        if (fill != null)
        {
            Vector2 size = fill.sizeDelta;
            size.x = maxWidth * Mathf.Clamp01(value / 100f);
            fill.sizeDelta = size;
            Debug.Log($"[UIUpdater] SetBar — fill:{fill.name} width:{size.x} sizeDelta:{fill.sizeDelta}");
        }
        else
        {
            Debug.LogWarning("[UIUpdater] SetBar — fill RectTransform is null!");
        }

        if (label != null)
            label.text = value.ToString();
    }
}
