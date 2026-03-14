using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FishingLine : MonoBehaviour
{
    [Header("Line Settings")]
    [SerializeField] private int nodeCount = 20;          // Number of rope nodes (more = smoother)
    [SerializeField] private float lineWidth = 0.025f;
    [SerializeField] private Color lineColor = Color.white;
    [SerializeField] private Color tensionColor = Color.red; // Color when under high tension
    [SerializeField] private int solverIterations = 30;   // Constraint solver passes per frame

    [Header("Physics")]
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float damping = 0.97f;       // Velocity retention per frame (0-1)

    private Vector3[] currentPositions;
    private Vector3[] previousPositions;
    private float restSegmentLength;

    private LineRenderer lineRenderer;
    private Transform startAnchor;
    private Transform endAnchor;
    private bool isActive;

    public float CurrentLength { get; private set; }

    public float Tension { get; private set; }

    public void Initialize(Transform start, Transform end, int nodes = -1)
    {
        startAnchor = start;
        endAnchor = end;

        if (nodes > 0) nodeCount = nodes;

        EnsureLineRenderer();
        lineRenderer.positionCount = nodeCount;

        currentPositions  = new Vector3[nodeCount];
        previousPositions = new Vector3[nodeCount];

        SeedPositions();
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
        if (lineRenderer != null)
            lineRenderer.positionCount = 0;
    }

    void LateUpdate()
    {
        if (!isActive) return;

        // If either anchor was destroyed, tear down gracefully
        if (startAnchor == null || endAnchor == null)
        {
            Deactivate();
            return;
        }

        Simulate();
        ApplyConstraints();
        RenderLine();
        MeasureTension();
    }

    void OnDestroy()
    {
        Deactivate();
    }

    private void SeedPositions()
    {
        for (int i = 0; i < nodeCount; i++)
        {
            float t = (float)i / (nodeCount - 1);
            Vector3 pos = Vector3.Lerp(startAnchor.position, endAnchor.position, t);
            currentPositions[i]  = pos;
            previousPositions[i] = pos;
        }

        restSegmentLength = Vector3.Distance(startAnchor.position, endAnchor.position) / (nodeCount - 1);
    }

    private void Simulate()
    {
        float dt2 = Time.deltaTime * Time.deltaTime;

        for (int i = 1; i < nodeCount - 1; i++)          // Skip pinned endpoints
        {
            Vector3 velocity = (currentPositions[i] - previousPositions[i]) * damping;
            previousPositions[i] = currentPositions[i];

            currentPositions[i] += velocity;
            currentPositions[i] += new Vector3(0f, gravity, 0f) * dt2;
        }
    }

    private void ApplyConstraints()
    {
        // Dynamic segment length: never shorter than the current anchor span
        // divided by segments, so the rope can stretch taut when pulled.
        float anchorDist = Vector3.Distance(startAnchor.position, endAnchor.position);
        float segLen = Mathf.Max(restSegmentLength, anchorDist / (nodeCount - 1));

        for (int iter = 0; iter < solverIterations; iter++)
        {
            // Pin endpoints every iteration
            currentPositions[0] = startAnchor.position;
            currentPositions[nodeCount - 1] = endAnchor.position;

            for (int i = 0; i < nodeCount - 1; i++)
            {
                Vector3 delta = currentPositions[i + 1] - currentPositions[i];
                float dist = delta.magnitude;
                if (dist < 0.0001f) continue;

                float error = (dist - segLen) / dist;
                Vector3 correction = (delta * error) * 0.5f;

                if (i != 0)
                    currentPositions[i] += correction;
                if (i + 1 != nodeCount - 1)
                    currentPositions[i + 1] -= correction;
            }
        }
    }

    private void RenderLine()
    {
        lineRenderer.SetPositions(currentPositions);
    }

    private void MeasureTension()
    {
        float total = 0f;
        for (int i = 0; i < nodeCount - 1; i++)
            total += Vector3.Distance(currentPositions[i], currentPositions[i + 1]);

        CurrentLength = total;

        float rest = restSegmentLength * (nodeCount - 1);
        Tension = rest > 0f ? Mathf.Max(0f, (total - rest) / rest) : 0f;
    }

    private void EnsureLineRenderer()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
                lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth   = lineWidth;
        lineRenderer.material   = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor   = lineColor;
        lineRenderer.useWorldSpace = true;
    }
}