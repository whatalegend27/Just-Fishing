using UnityEngine;

public class DaySkipButton : MonoBehaviour
{
   public inGameTime gameTime;
   public GameObject confirmPopup;

   // Shows the confirmation popup when the button is clicked
   public void onButtonClicked()
   {
      if ( confirmPopup != null )
      {
         confirmPopup.SetActive( true );
         Time.timeScale=0f;
      }
   }

   // Increments the day and closes the popup when confirmed
   public void onConfirm()
   {
      if ( gameTime != null )
      {
         gameTime.incrementDay();
      }

      if ( confirmPopup != null )
      {
         confirmPopup.SetActive( false );
      }
      Time.timeScale=1f;
   }

   // Closes the popup without changing the day when cancelled
   public void onCancel()
   {
      if ( confirmPopup != null )
      {
         confirmPopup.SetActive( false );
      }
      Time.timeScale=1f;
   }
}
