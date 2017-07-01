using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSign : MonoBehaviour {

    public GameObject child;

	// Update is called once per frame
	void Update () {
        child.transform.Rotate(0, 0, 2);
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.25f;
        transform.LookAt(Camera.main.transform);
	}

}
