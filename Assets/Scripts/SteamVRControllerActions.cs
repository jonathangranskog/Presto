using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamVRControllerActions : MonoBehaviour {

    private ActionManager actionManager;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_TrackedController controller;

    private void Start()
    {
        actionManager = GetComponent<ActionManager>();
    }

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void OnEnable()
    {
        controller = GetComponent<SteamVR_TrackedController>();
        controller.PadClicked += PadClick;
        controller.TriggerClicked += TriggerClick;
        controller.TriggerUnclicked += TriggerUnclick;
        controller.MenuButtonClicked += MenuClick;
    }

    private void OnDisable()
    {
        controller.PadClicked -= PadClick;
        controller.TriggerClicked -= TriggerClick;
        controller.TriggerUnclicked -= TriggerUnclick;
        controller.MenuButtonClicked -= MenuClick;
    }

    private void PadClick(object sender, ClickedEventArgs e)
    {
        if (e.padX < 0)
        {
            actionManager.PadLeftClick();
        } else
        {
            actionManager.PadRightClick();
        }
    }

    private void TriggerClick(object sender, ClickedEventArgs e)
    {
        actionManager.TriggerPress();
    }

    private void TriggerUnclick(object sender, ClickedEventArgs e)
    {
        actionManager.TriggerUnpress();
    }

    private void MenuClick(object sender, ClickedEventArgs e)
    {
        actionManager.MenuClick();
    }

}
