using UnityEngine; 

// This script manages the toolbox UI and disables gameplay scripts when the toolbox is active.
public class HandleToolbox : MonoBehaviour {
      [Header("Toolbox Settings")]
      [SerializeField] public GameObject tbShow; 
      public GameObject[] toolboxes; 
      public bool tbActive = false; 
      void Start() { 
        foreach(GameObject tb in toolboxes) { 
            tb.SetActive(false); 
        }
    } 
        
        //If T is pressed, toggle the toolbox and disable/enable gameplay scripts accordingly
        void Update() { 
            if (Input.GetKeyDown(KeyCode.T)) { 
                ToggleToolbox(); 
            } 
        } 

        // New method to toggle the toolbox and manage script states
        public void ToggleToolbox() { 
            tbActive = !tbActive; 
            foreach (GameObject tb in toolboxes) { 
                tb.SetActive(false); 
            } 
            if (tbActive && tbShow != null) {
                 tbShow.SetActive(true); 
            } // Disable/enable gameplay scripts 
            
            Time.timeScale = tbActive ? 0f : 1f;       
        } 
}