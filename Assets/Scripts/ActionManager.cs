using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour {

    public GameObject pageManagerObject;

    private PageManager pageManager;
    private bool menuOpen = false;
    private bool triggerHeld = false;
    private GameObject cursor;
    private MeshRenderer cursorRenderer;

    void Start()
    {
        pageManager = pageManagerObject.GetComponent<PageManager>();
        cursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cursorRenderer = cursor.GetComponent<MeshRenderer>();
        cursor.transform.localScale = 0.05f * Vector3.one;
        cursorRenderer.enabled = false;
        Destroy(cursor.GetComponent<SphereCollider>());
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            if (triggerHeld)
            {
                cursorRenderer.enabled = true;
                cursor.transform.position = hit.point;
            }
        } else if (triggerHeld)
        {
            cursorRenderer.enabled = false;
        }
    }

    public void TriggerPress()
    {
        // TODO: Create laser pointer if it does not hit menu
        Debug.Log("Trigger pressed!");
        triggerHeld = true;
        cursorRenderer.enabled = true;
    }

    public void TriggerUnpress()
    {
        triggerHeld = false;
        cursorRenderer.enabled = false;
    }

    public void PadRightClick()
    {
        pageManager.NextPage();
    }

    public void PadLeftClick()
    {
        pageManager.PreviousPage();
    }

    public void MenuClick()
    {
        // TODO: Open or close menu
        Debug.Log("Menu button clicked!");
    }
}
