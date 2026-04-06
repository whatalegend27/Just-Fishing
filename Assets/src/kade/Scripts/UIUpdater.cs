using UnityEngine;
using UnityEngine.UIElements;

public class UIUpdater : MonoBehaviour
{
    public HealthStats healthStats;
    public ArrestStats arrestStats;
    public UIDocument uiDoc;

    private Label m_HungerLabel;
    private Label m_ExhaustionLabel;
    private Label m_RiskLabel;

    private int m_CachedHunger;
    private int m_CachedExhaustion;
    private int m_CachedRisk;

    void Start()
    {
        var root = uiDoc.rootVisualElement;
        m_HungerLabel     = root.Q<Label>("HungerLabel");
        m_ExhaustionLabel = root.Q<Label>("ExhaustionLabel");
        m_RiskLabel       = root.Q<Label>("RiskLabel");

        m_CachedHunger     = healthStats.hungerVal;
        m_CachedExhaustion = healthStats.exhaustionVal;
        m_CachedRisk       = arrestStats.riskVal;

        RefreshUI();
    }

    void Update()
    {
        bool changed = false;

        if (healthStats.hungerVal != m_CachedHunger)
        {
            m_CachedHunger = healthStats.hungerVal;
            changed = true;
        }
        if (healthStats.exhaustionVal != m_CachedExhaustion)
        {
            m_CachedExhaustion = healthStats.exhaustionVal;
            changed = true;
        }
        if (arrestStats.riskVal != m_CachedRisk)
        {
            m_CachedRisk = arrestStats.riskVal;
            changed = true;
        }

        if (changed)
            RefreshUI();
    }

    void RefreshUI()
    {
        if (m_HungerLabel is not null)
            m_HungerLabel.text = m_CachedHunger.ToString();
        if (m_ExhaustionLabel is not null)
            m_ExhaustionLabel.text = m_CachedExhaustion.ToString();
        if (m_RiskLabel is not null)
            m_RiskLabel.text = m_CachedRisk.ToString();
    }
}
