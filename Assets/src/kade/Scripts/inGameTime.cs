using UnityEngine;
using TMPro;

public class inGameTime : MonoBehaviour
{
    public int hours = 6;
    public int minutes = 0;

    public TextMeshProUGUI timeText;

    private float _elapsed = 0f;

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
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (timeText != null)
            timeText.text = $"{hours:D2}:{minutes:D2}";
    }
}
