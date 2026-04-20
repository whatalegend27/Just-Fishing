using UnityEngine;
using TMPro;
using System.Collections;

public class XPPopup : MonoBehaviour
{
   public TMP_Text xpText;
   public float displayDuration = 2f;

   private PlayerLevel mPlayerLevel;

   void Awake()
   {
      mPlayerLevel = FindAnyObjectByType<PlayerLevel>();
      if ( mPlayerLevel != null )
         mPlayerLevel.OnXPGained += showPopup;
      gameObject.SetActive( false );
   }

   void OnDestroy()
   {
      if ( mPlayerLevel != null )
         mPlayerLevel.OnXPGained -= showPopup;
   }

   private void showPopup( int xp )
   {
      if ( xpText != null )
         xpText.text = $"+{xp} XP";

      gameObject.SetActive( true );
      StopAllCoroutines();
      StartCoroutine( hideAfterDelay() );
   }

   private IEnumerator hideAfterDelay()
   {
      yield return new WaitForSeconds( displayDuration );
      gameObject.SetActive( false );
   }
}
