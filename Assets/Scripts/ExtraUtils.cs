using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A bunch of useful functions that are used in various classes
public static class ExtraUtils {

	public static string ClampName(string name, int n)
    {
        if (name.Length < n)
        {
            return name;
        } else
        {
            string cutoff = name.Substring(0, n - 3);
            cutoff += "...";
            return cutoff;
        }
    }

    public static string ClampFrontName(string name, int n)
    {
        if (name.Length < n)
        {
            return name;
        } else
        {
            string cutoff = name.Substring(name.Length - n + 3, n - 3);
            string result = "..." + cutoff;
            return result;
        }
    }

    // Returns RaycastHit information and true if the ray hit the world space canvas
    public static bool RaycastCanvas(Canvas canvas, Ray ray, out RaycastHit hit)
    {
        
        RectTransform canvasTransform = canvas.gameObject.GetComponent<RectTransform>();
        RaycastHit h = new RaycastHit();
        hit = h;

        if (!canvas.gameObject.activeInHierarchy) return false;
        if (canvasTransform == null) return false;

        Vector3[] canvasCorners = new Vector3[4];
        canvasTransform.GetWorldCorners(canvasCorners);
        Plane canvasPlane = new Plane(canvasCorners[0], canvasCorners[1], canvasCorners[2]);

        float t;

        if (RaycastBoundedPlane(ray, canvasCorners, canvasPlane, out t))
        {
            hit.distance = t;
            hit.point = ray.GetPoint(t);
            hit.normal = canvasPlane.normal;
            return true;
        }

        return false;
    }

    // Ray intersection with a bounded plane (plane defined by four corners)
    public static bool RaycastBoundedPlane(Ray ray, Vector3[] corners, Plane plane, out float t)
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
        }
        else
        {
            t = dist;
            return false;
        }
    }

    // Gets the button element at an intersection point. Point has to be on Canvas.
    public static Button GetButtonAtPosition(Canvas canvas, Vector3 pos)
    {
        IList<Graphic> graphics = GraphicRegistry.GetGraphicsForCanvas(canvas);

        for (int i = 0; i < graphics.Count; i++)
        {
            Graphic graphic = graphics[i];
            RectTransform transform = graphic.rectTransform;
            GameObject obj = transform.gameObject;

            if (obj.activeInHierarchy && obj.GetComponent<Button>() != null)
            {
                Vector3[] corners = new Vector3[4];
                transform.GetWorldCorners(corners);
                if (ExtraUtils.WithinPlane(pos, corners))
                {
                    return obj.GetComponent<Button>();
                }
            }
        }

        return null;
    }

    // Gets the toggle at intersection point
    public static Toggle GetToggleAtPosition(List<GameObject> toggles, Vector3 pos)
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            GameObject obj = toggles[i];
            RectTransform transform = obj.GetComponent<RectTransform>();

            if (obj.activeInHierarchy && obj.GetComponent<Toggle>() != null)
            {
                Vector3[] corners = new Vector3[4];
                transform.GetWorldCorners(corners);
                if (ExtraUtils.WithinPlane(pos, corners))
                {
                    return obj.GetComponent<Toggle>();
                }
            }
        }

        return null;
    }

    // Checks whether a point on the same plane is inside of the square defined by corners
    public static bool WithinPlane(Vector3 point, Vector3[] corners)
    {
        bool tri1 = WithinTriangle(point, corners[0], corners[1], corners[2]);
        bool tri2 = WithinTriangle(point, corners[2], corners[3], corners[0]);
        return tri1 || tri2;
    }

    // Barycentric comparison to see if the point is inside the triangle
    // Source: Christer Ericson's Real-Time Collision Detection
    // https://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates
    public static bool WithinTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
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

    public static void ResetRectTransform(RectTransform transform)
    {
        transform.localScale = Vector3.one;
        transform.anchoredPosition3D = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

}
