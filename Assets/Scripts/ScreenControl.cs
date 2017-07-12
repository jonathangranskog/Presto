using UnityEngine;

public class ScreenControl : MonoBehaviour {

    public float scale = 1.0f;

    public void SetTexture(Texture2D tex)
    {
        float aspectRatio = (float)tex.height / tex.width;

        Vector3 size = Vector3.one;

        if (aspectRatio > 1.0f)
        {
            size = scale * (new Vector3(1.0f / aspectRatio, 1, 1));
        } else
        {
            size = scale * (new Vector3(1, aspectRatio, 1));
        }
        GetComponent<MeshRenderer>().material.mainTexture = tex;
        transform.localScale = size;
    }
}
