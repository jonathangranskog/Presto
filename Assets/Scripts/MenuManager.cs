using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public GameObject canvasObject;

    private Canvas canvas;
    private RectTransform canvasTransform;

    private void Start()
    {
        canvas = canvasObject.GetComponent<Canvas>();
        canvasTransform = canvasObject.GetComponent<RectTransform>();
    }

    public bool RaycastUI(Ray ray, out RaycastHit hit)
    {
        RaycastHit h = new RaycastHit();
        hit = h;

        Vector3[] canvasCorners = new Vector3[4];
        canvasTransform.GetWorldCorners(canvasCorners);
        Plane canvasPlane = new Plane(canvasCorners[0], canvasCorners[1], canvasCorners[2]);

        float t;
        if (canvasPlane.Raycast(ray, out t))
        {
            IList<Graphic> graphics = GraphicRegistry.GetGraphicsForCanvas(canvas);

            for (int i = 0; i < graphics.Count; i++)
            {
                Graphic graphic = graphics[i];
                RectTransform transform = graphic.rectTransform;
                GameObject obj = transform.gameObject;
                if (obj.GetComponent<Button>() != null)
                {
                    Vector3[] corners = new Vector3[4];
                    transform.GetWorldCorners(corners);
                    Plane plane = new Plane(corners[0], corners[1], corners[2]);

                    if (RaycastBoundedPlane(ray, corners, plane, out t))
                    {
                        hit.distance = t;
                        hit.point = ray.GetPoint(t);
                        hit.normal = plane.normal;
                        return true;
                    }
                }
            }

            return false;
        } else
        {
            return false;
        }
    }

    private bool RaycastBoundedPlane(Ray ray, Vector3[] corners, Plane plane, out float t)
    {
        float dist = -1.0f;

        if (plane.Raycast(ray, out dist))
        {
            t = dist;
            Vector3 point = ray.GetPoint(dist);
            if (WithinPlane(point, corners))
                return true;
            else
                return false;
        } else
        {
            t = dist;
            return false;
        }
    }

    private bool WithinPlane(Vector3 point, Vector3[] corners)
    {
        bool tri1 = WithinTriangle(point, corners[0], corners[1], corners[2]);
        bool tri2 = WithinTriangle(point, corners[2], corners[3], corners[0]);
        return tri1 || tri2;
    }

    private bool WithinTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 e0 = b - a;
        Vector3 e1 = c - a;
        Vector3 e2 = p - a;

        float d00 = Vector3.Dot(e0, e0);
        float d01 = Vector3.Dot(e0, e1);
        float d11 = Vector3.Dot(e1, e1);
        float d20 = Vector3.Dot(e2, e0);
        float d21 = Vector3.Dot(e2, e1);
        float denom = d00 * d11 - d01 * d01;
        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;

        if (u >= 0 && u <= 1.0f && v >= 0 && v <= 1.0f && u + v <= 1.0f)
        {
            return true;
        }

        return false;
    }

}
