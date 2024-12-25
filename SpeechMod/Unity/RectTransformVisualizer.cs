using UnityEngine;

namespace SpeechMod.Unity;

[RequireComponent(typeof(LineRenderer))]
public class RectTransformVisualizer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private RectTransform rectTransform;

    void Awake()
    {
        // Get the RectTransform and LineRenderer components
        rectTransform = GetComponent<RectTransform>();
        lineRenderer = GetComponent<LineRenderer>();

        // Set LineRenderer properties for drawing the rectangle
        lineRenderer.positionCount = 5; // 4 corners + 1 to close the loop
        lineRenderer.loop = true;       // Loop the line to close the shape
        lineRenderer.useWorldSpace = false; // Use local space to match the RectTransform space
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.colorGradient = new Gradient{mode = GradientMode.Fixed, alphaKeys = new []{new GradientAlphaKey(1,0f)}, colorKeys = new []{new GradientColorKey(Color.red, 0f)}};
        lineRenderer.startWidth = 2f;
        lineRenderer.endWidth = 2f;
    }

    void Update()
    {
        // Update the corners and redraw the boundary each frame
        DrawRectTransformBounds();
    }

    void DrawRectTransformBounds()
    {
        // Get the local corners of the RectTransform (in local space of the canvas)
        Vector3[] corners = new Vector3[4];
        rectTransform.GetLocalCorners(corners);

        // Set the positions of the LineRenderer to the corners
        for (int i = 0; i < 4; i++)
        {
            lineRenderer.SetPosition(i, corners[i]);
        }

        // Close the loop (back to the first corner)
        lineRenderer.SetPosition(4, corners[0]);
    }
}