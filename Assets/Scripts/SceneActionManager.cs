using UnityEngine;

public class SceneActionManager : ActionManager {

    private enum HitObject
    {
        None,
        Menu,
        Timer,
        Screen
    }

    public GameObject pageManagerObject;
    public float rayCastMaxDist = 20.0f;
    public GameObject menuObject;
    public GameObject timerObject;

    private PageManager pageManager;
    private bool triggerHeld = false;
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

    override public void TriggerPress()
    {
        if (hitObj == HitObject.Menu)
        {
            menu.TriggerAction(ray);
        }
        else if (hitObj == HitObject.Timer)
        {
            timer.TriggerAction(ray);
        }

        triggerHeld = true;
    }

    override public void TriggerUnpress()
    {
        triggerHeld = false;
    }

    override public void PadRightClick()
    {
        if (menu.isOpen()) menu.NextPage();
        else if (hitObj == HitObject.Timer) timer.RightPadStep();
        else pageManager.NextPage();
    }

    override public void PadLeftClick()
    {
        if (menu.isOpen()) menu.PreviousPage();
        else if (hitObj == HitObject.Timer) timer.LeftPadStep();
        else pageManager.PreviousPage();
    }

    override public void MenuClick()
    {
        if (pageManager.loading) pageManager.Interrupt();
        else menu.Toggle();
    }
}
