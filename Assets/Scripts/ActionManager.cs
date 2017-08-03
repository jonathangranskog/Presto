using UnityEngine;

// Base class for different ActionManagers
// ie managers that receive messages from controllers and do something
abstract public class ActionManager : MonoBehaviour {

    public Material cursorMaterial;

    protected GameObject cursor;
    protected MeshRenderer cursorRenderer;
    protected int cursorRaycastMask;
    
    abstract public void TriggerPress();

    abstract public void TriggerUnpress();

    abstract public void PadRightClick();

    abstract public void PadLeftClick();

    abstract public void MenuClick();

    protected void CreateCursor()
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
