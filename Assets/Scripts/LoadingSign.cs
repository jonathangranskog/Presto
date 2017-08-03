using UnityEngine;

// Loading wheel while a PDF is converted
public class LoadingSign : MonoBehaviour {

    public GameObject child;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    // Keep in front of camera and rotate slowly
    void Update () {
        child.transform.Rotate(0, 0, 2);
        transform.position = mainCamera.transform.position + mainCamera.transform.forward * 1.25f;
        transform.LookAt(mainCamera.transform);
	}

}
