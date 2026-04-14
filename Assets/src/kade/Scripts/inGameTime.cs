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
   public event System.Action OnNightfall;

   private float mElapsed = 0f;
   private bool mNewDayTriggered = false;

   // Initializes the UI on start
   void Start()
   {
      updateUI();
   }

   // Advances time every real-world second
   void Update()
   {
      mElapsed += Time.deltaTime;

      if ( mElapsed >= 1f )
      {
         mElapsed -= 1f;
         advanceMinute();
      }
   }

   // Advances the clock by 10 minutes and checks for day and nightfall events
   private void advanceMinute()
   {
      minutes += 10;

      if ( minutes >= 60 )
      {
         minutes = 0;
         hours = ( hours + 1 ) % 24;

         if ( hours == 21 )
         {
            OnNightfall?.Invoke();
         }

         if ( hours == 6 && !mNewDayTriggered )
         {
            day++;
            mNewDayTriggered = true;
            OnNewDay?.Invoke();
         }
         else if ( hours != 6 )
         {
            mNewDayTriggered = false;
         }
      }

      updateUI();
   }

   // Updates the time and day text in the UI
   private void updateUI()
   {
      if ( timeText != null )
      {
         timeText.text = $"{hours:D2}:{minutes:D2}";
      }

      if ( dayText != null )
      {
         dayText.text = $"Day {day}";
      }
   }
}
