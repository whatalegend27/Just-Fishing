using UnityEngine;
using TMPro;

public class inGameTime : MonoBehaviour
{
    public int hours = 6;
    public int minutes = 0;
    public int day = 1;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dayText;

    public event System.Action OnNewDay;

    private float _elapsed = 0f;
    private bool m_NewDayTriggered = false;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        _elapsed += Time.deltaTime;

        if (_elapsed >= 1f)
        {
            _elapsed -= 1f;
            AdvanceMinute();
        }
    }

    private void AdvanceMinute()
    {
        minutes++;

        if (minutes >= 60)
        {
            minutes = 0;
            hours = (hours + 1) % 24;

            if (hours == 6 && !m_NewDayTriggered)
            {
                day++;
                m_NewDayTriggered = true;
                OnNewDay?.Invoke();
            }
            else if (hours != 6)
            {
                m_NewDayTriggered = false;
            }
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (timeText != null)
            timeText.text = $"{hours:D2}:{minutes:D2}";

        if (dayText != null)
            dayText.text = $"Day {day}";
    }
}
