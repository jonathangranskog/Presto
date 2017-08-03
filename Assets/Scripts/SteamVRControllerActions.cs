using UnityEngine;

// Sends messages to an action manager based on what buttons have been clicked etc
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

    // Register events
    private void OnEnable()
    {
        controller = GetComponent<SteamVR_TrackedController>();
        controller.PadClicked += PadClick;
        controller.TriggerClicked += TriggerClick;
        controller.TriggerUnclicked += TriggerUnclick;
        controller.MenuButtonClicked += MenuClick;
    }

    // Unregister events
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
