using UnityEngine;

public class ScreenControl : MonoBehaviour {

    public float scale = 1.0f;

    public void SetTexture(Texture2D tex)
    {
        float aspectRatio = (float)tex.height / tex.width;
        Vector3 size = scale * (new Vector3(1, aspectRatio, 1));
        GetComponent<MeshRenderer>().material.mainTexture = tex;
        transform.localScale = size;
    }
}
