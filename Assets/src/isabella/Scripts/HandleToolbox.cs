using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 

public class HandleToolbox : MonoBehaviour {
     public MonoBehaviour[] scriptsToDisable;
      public GameObject[] Toolboxes; 
      public GameObject TbToShow; 
      public bool TBActive = false; // 👈 make public for testing 
      void Start() { 
        foreach(GameObject tb in Toolboxes) { 
            tb.SetActive(false); 
        }
    } 
        
        void Update() { 
            if (Input.GetKeyDown(KeyCode.T)) { 
                ToggleToolbox(); // 👈 use new method 
            } 
        } 
        public void ToggleToolbox() { 
            TBActive = !TBActive; 
            foreach (GameObject tb in Toolboxes) { 
                tb.SetActive(false); 
            } 
            if (TBActive && TbToShow != null) {
                 TbToShow.SetActive(true); 
            } // ✅ Disable/enable gameplay scripts 
            foreach (MonoBehaviour script in scriptsToDisable) {
                 script.enabled = !TBActive; 
            } // ✅ Optional: pause the game 
            
            Time.timeScale = TBActive ? 0f : 1f;       
        } 
}