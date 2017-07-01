using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour {

    public GameObject pageManagerObject;
    public Material cursorMaterial;
    public float rayCastMaxDist = 20.0f;

    private PageManager pageManager;
    private bool menuOpen = false;
    private bool triggerHeld = false;
    private GameObject cursor;
    private MeshRenderer cursorRenderer;
    private int cursorRaycastMask;
    
    
    void Start()
    {
        pageManager = pageManagerObject.GetComponent<PageManager>();
        CreateCursor();
        int screenLayer = 1 << 31;
        int menuLayer = 1 << 30;
        cursorRaycastMask = menuLayer | screenLayer;
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit = new RaycastHit();
        
        if ((triggerHeld || menuOpen) && Physics.Raycast(ray, out hit, rayCastMaxDist, cursorRaycastMask))
        {
            cursorRenderer.enabled = true;
            cursor.transform.position = hit.point;
            cursor.transform.forward = hit.normal;
        } else
        {
            cursorRenderer.enabled = false;
        }
    }

    public void TriggerPress()
    {
        // TODO: Menu interaction
        triggerHeld = true;
    }

    public void TriggerUnpress()
    {
        triggerHeld = false;
    }

    public void PadRightClick()
    {
        // TODO: If page manager loading, interrupt!
        pageManager.NextPage();
    }

    public void PadLeftClick()
    {
        // TODO: If page manager loading, interrupt!
        pageManager.PreviousPage();
    }

    public void MenuClick()
    {
        if (menuOpen)
        {
            CloseMenu();
        } else
        {
            OpenMenu();
        }
        menuOpen = !menuOpen;
        Debug.Log("Menu button clicked!");
    }

    private void OpenMenu()
    {
        // TODO: Create a menu via MenuManager if menu has not been created
        // else make menu visible and put in pointer or gaze direction
    }

    private void CloseMenu()
    {
        // TODO: Hide Menu via MenuManager
    }

    private void CreateCursor()
    {
        GameObject cursorObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cursorRenderer = cursorObj.GetComponent<MeshRenderer>();
        cursorObj.transform.localScale = 0.025f * (new Vector3(1, 0.01f, 1));
        cursorRenderer.enabled = false;
        cursorRenderer.material = cursorMaterial;
        cursorObj.transform.Rotate(90, 0, 0);
        Destroy(cursorObj.GetComponent<CapsuleCollider>());
        cursor = new GameObject();
        cursorObj.transform.parent = cursor.transform;
    }

}
