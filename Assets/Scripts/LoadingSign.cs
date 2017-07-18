using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSign : MonoBehaviour {

    public GameObject child;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update () {
        child.transform.Rotate(0, 0, 2);
        transform.position = mainCamera.transform.position + mainCamera.transform.forward * 1.25f;
        transform.LookAt(mainCamera.transform);
	}

}
