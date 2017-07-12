using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour {

    public GameObject pageManagerObject;
    public Material cursorMaterial;
    public float rayCastMaxDist = 20.0f;
    public GameObject menuObject;
    
    private PageManager pageManager;
    private bool triggerHeld = false;
    private GameObject cursor;
    private MeshRenderer cursorRenderer;
    private int cursorRaycastMask;
    private MenuManager menu;
    
    void Start()
    {
        pageManager = pageManagerObject.GetComponent<PageManager>();
        menu = menuObject.GetComponent<MenuManager>();
        CreateCursor();
        int screenLayer = 1 << 31;
        int menuLayer = 1 << 30;
        cursorRaycastMask = menuLayer | screenLayer;
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit = new RaycastHit();

        cursorRenderer.enabled = false;

        if (menu.isOpen())
        {
            if (menu.RaycastCanvas(ray, out hit))
            {
                cursorRenderer.enabled = true;
                cursor.transform.position = hit.point;
                cursor.transform.forward = hit.normal;
            }
        } else if (triggerHeld)
        {
            if (Physics.Raycast(ray, out hit, rayCastMaxDist, cursorRaycastMask))
            {
                cursorRenderer.enabled = true;
                cursor.transform.position = hit.point;
                cursor.transform.forward = hit.normal;
            }
        }

    }

    public void TriggerPress()
    {
        if (menu.isOpen())
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit = new RaycastHit();
            if (menu.RaycastCanvas(ray, out hit))
            {
                Button button = menu.GetButtonAtPosition(hit.point);
                if (button != null)
                {
                    button.onClick.Invoke();
                }
            }
        }

        triggerHeld = true;
    }

    public void TriggerUnpress()
    {
        triggerHeld = false;
    }

    public void PadRightClick()
    {
        if (menu.isOpen()) menu.NextPage();
        else pageManager.NextPage();
    }

    public void PadLeftClick()
    {
        if (menu.isOpen()) menu.PreviousPage();
        else pageManager.PreviousPage();
    }

    public void MenuClick()
    {
        if (pageManager.loading) pageManager.Interrupt();
        else menu.Toggle();
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
        cursor.name = "Cursor";
        cursorObj.transform.parent = cursor.transform;
    }

}
