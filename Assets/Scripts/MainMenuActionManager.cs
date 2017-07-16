using System;
using UnityEngine;

public class MainMenuActionManager : ActionManager {

    public GameObject menuManagerObject;

    private MainMenuManager menuManager;
    private Ray ray;

    private void Start()
    {
        menuManager = menuManagerObject.GetComponent<MainMenuManager>();
        CreateCursor();
    }

    private void Update()
    {
        ray = new Ray(transform.position, transform.forward);
        cursorRenderer.enabled = false;

        RaycastHit hit = new RaycastHit();

        if (menuManager.Raycast(ray, out hit))
        {
            cursorRenderer.enabled = true;
            cursor.transform.position = hit.point;
            cursor.transform.forward = hit.normal;
        }

    }

    public override void MenuClick()
    {

    }

    public override void PadLeftClick()
    {

    }

    public override void PadRightClick()
    {

    }

    public override void TriggerPress()
    {
        menuManager.TriggerAction(ray);
    }

    public override void TriggerUnpress()
    {

    }

}
