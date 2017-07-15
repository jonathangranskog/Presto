using System;
using System.Collections;
using System.Collections.Generic;
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
        throw new NotImplementedException();
    }

    public override void PadLeftClick()
    {
        throw new NotImplementedException();
    }

    public override void PadRightClick()
    {
        throw new NotImplementedException();
    }

    public override void TriggerPress()
    {
        throw new NotImplementedException();
    }

    public override void TriggerUnpress()
    {
        throw new NotImplementedException();
    }

}
