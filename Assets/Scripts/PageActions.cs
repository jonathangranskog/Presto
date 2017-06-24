using UnityEngine;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class PageActions : MonoBehaviour {

    public GameObject pageManagerObject;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_TrackedController controller;
    private PageManager pageManager;
    
	void Awake () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        pageManager = pageManagerObject.GetComponent<PageManager>();
	}

    private void OnEnable()
    {
        controller = GetComponent<SteamVR_TrackedController>();
        controller.PadClicked += PadClick;
    }

    private void OnDisable()
    {
        controller.PadClicked -= PadClick;
    }

    private void PadClick(object sender, ClickedEventArgs e)
    {
        if (e.padX < 0)
        {
            pageManager.PreviousPage();
        } else
        {
            pageManager.NextPage();
        }
    }
}
