using UnityEngine;
using UnityEngine.InputSystem;

public class HelperClass
{
    public static Vector3 GetMouseWorldPosition(Camera cam, float distanceFromCamera, LayerMask layerMask)
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, distanceFromCamera, layerMask))
        {
            return hit.point;
        }
        else
        {
            // If no hit, return a point at the specified distance along the ray
            return ray.GetPoint(distanceFromCamera);
        }
    }

    public static LineRenderer InitRenderLine(GameObject parent, int points, float width = 0.05f)
    {
        LineRenderer line = parent.AddComponent<LineRenderer>();
        line.positionCount = points;
        line.startWidth = width;
        line.endWidth = width;
        line.material = new Material(Shader.Find("Sprites/Default"));
        return line;
    }
}
