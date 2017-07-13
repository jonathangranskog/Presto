using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour {

    private enum HitObject
    {
        None,
        Menu,
        Timer,
        Screen
    }

    public GameObject pageManagerObject;
    public Material cursorMaterial;
    public float rayCastMaxDist = 20.0f;
    public GameObject menuObject;
    public GameObject timerObject;
    
    private PageManager pageManager;
    private bool triggerHeld = false;
    private GameObject cursor;
    private MeshRenderer cursorRenderer;
    private int cursorRaycastMask;
    private MenuManager menu;
    private TimerCounter timer;
    private Ray ray;
    private HitObject hitObj;
    
    void Start()
    {
        pageManager = pageManagerObject.GetComponent<PageManager>();
        menu = menuObject.GetComponent<MenuManager>();
        timer = timerObject.GetComponent<TimerCounter>();
        CreateCursor();
        int screenLayer = 1 << 31;
        cursorRaycastMask = screenLayer;
    }

    private void Update()
    {
        ray = new Ray(transform.position, transform.forward);
        cursorRenderer.enabled = false;
        hitObj = HitObject.None;

        RaycastHit menuHit = new RaycastHit(); bool hitMenu;
        if (hitMenu = menu.Raycast(ray, out menuHit))
        {
            cursorRenderer.enabled = true;
            cursor.transform.position = menuHit.point;
            cursor.transform.forward = menuHit.normal;
            hitObj = HitObject.Menu;
        }

        RaycastHit timerHit = new RaycastHit(); bool hitTimer;
        if (hitTimer = timer.Raycast(ray, out timerHit))
        {
            if (!hitMenu || timerHit.distance < menuHit.distance)
            {
                cursorRenderer.enabled = true;
                cursor.transform.position = timerHit.point;
                cursor.transform.forward = timerHit.normal;
                hitObj = HitObject.Timer;
            }
        }

        if (triggerHeld)
        {
            RaycastHit screenHit = new RaycastHit();
            if (Physics.Raycast(ray, out screenHit, rayCastMaxDist, cursorRaycastMask))
            {
                if ((!hitMenu || screenHit.distance < menuHit.distance) &&
                    (!hitTimer || screenHit.distance < timerHit.distance))
                {
                    cursorRenderer.enabled = true;
                    cursor.transform.position = screenHit.point;
                    cursor.transform.forward = screenHit.normal;
                    hitObj = HitObject.Screen;
                }   
            }
        }

    }

    public void TriggerPress()
    {
        if (hitObj == HitObject.Menu)
        {
            menu.TriggerAction(ray);
        } else if (hitObj == HitObject.Timer)
        {
            timer.TriggerAction(ray);
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
        else if (hitObj == HitObject.Timer) timer.StepForward();
        else pageManager.NextPage();
    }

    public void PadLeftClick()
    {
        if (menu.isOpen()) menu.PreviousPage();
        else if (hitObj == HitObject.Timer) timer.StepBack();
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
