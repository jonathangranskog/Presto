using UnityEngine;
using UnityEngine.UI;

public class TogglePress : MonoBehaviour {

    public GameObject checkboxObject;

    private Toggle toggle;

	// Use this for initialization
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
