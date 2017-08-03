using UnityEngine;
using UnityEngine.UI;

// If a toggle is pressed, run its onValueChanged functions
public class TogglePress : MonoBehaviour {

    public GameObject checkboxObject;
    
    private Toggle toggle;

	void Start () {
        toggle = GetComponent<Toggle>();
	}
	
    public void Toggle()
    {
        toggle.isOn = !toggle.isOn;
        toggle.onValueChanged.Invoke(toggle.isOn);
        checkboxObject.SetActive(toggle.isOn);
    }
}
