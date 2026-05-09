using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DamageNumber : MonoBehaviour
{
    public float duration = 0.75f;
    public float maxVelocity = 5f;
    public float randomOffsetMin = 0.85f; // Lowest possible offset value
    public float randomOffsetMax = 1.15f; // Highest possible offset value
    public AnimationCurve alphaCurve;
    public AnimationCurve heightCurve;
    public AnimationCurve horizontalCurve;

    [Header("Debug Trajectory Cloud")]
    [SerializeField] private bool drawDebugTrajectoryCloud = true;
    [SerializeField] private int debugTrajectoryCount = 12;
    [SerializeField] private int debugSamplesPerTrajectory = 24;
    [SerializeField] private int debugSeed = 5231;
    [SerializeField] private Color debugTrajectoryColor = new(1f, 0.75f, 0f, 0.45f);
    [SerializeField] private float debugPointRadius = 0.02f;

    
    private float timeElapsed = 0f;
    private Vector3 spawnPosition = Vector3.zero;
    private Vector3 randomOffset = Vector3.zero;
    private Vector3 randomDirection = Vector3.one;
    private TextMeshProUGUI textMesh;
    private System.Action<DamageNumber> releaseCallback;
    private Transform cameraTransform;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(float damage, System.Action<DamageNumber> onRelease)
    {
        releaseCallback = onRelease;
        timeElapsed = 0f;

        randomDirection = new(Mathf.RoundToInt(Random.value) * 2 - 1, 
                              1f, 
                              Mathf.RoundToInt(Random.value) * 2 - 1);
        randomOffset = randomDirection * Random.Range(randomOffsetMin, randomOffsetMax);
        spawnPosition = transform.position;

        if (textMesh != null)
        {
            textMesh.text = damage.ToString("0.##");
            Color color = textMesh.color;
            color.a = alphaCurve.Evaluate(0f);
            textMesh.color = color;
        }

        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
        }

        if (cameraTransform != null)
        {
            transform.rotation = cameraTransform.rotation;
        }
    }

    void LateUpdate()
    {
        timeElapsed += Time.deltaTime;
        UpdateAlpha();
        if (timeElapsed >= duration)
        {
            ReturnToPool();
            return;
        }
        UpdatePosition();
    }

    private void UpdateAlpha()
    {
        if (textMesh == null) return;
        Color color = textMesh.color;
        color.a = alphaCurve.Evaluate(Mathf.Clamp01(timeElapsed / duration));
        textMesh.color = color;
    }

    private void UpdatePosition()
    {
        float t = Mathf.Clamp01(timeElapsed / duration);
        Vector3 curveOffset = EvaluateCurveOffset(randomOffset, t);
        transform.position = spawnPosition + curveOffset;
    }

    private Vector3 EvaluateCurveOffset(Vector3 offsetBasis, float normalizedTime)
    {
        float height = heightCurve != null ? heightCurve.Evaluate(normalizedTime) : normalizedTime;
        float horizontal = horizontalCurve != null ? horizontalCurve.Evaluate(normalizedTime) : normalizedTime;
        return new Vector3(offsetBasis.x * horizontal, offsetBasis.y * height, 0f);
    }

    private Vector3 GenerateDebugRandomOffset(System.Random random)
    {
        float x = random.Next(0, 2) == 0 ? -1f : 1f;
        float z = random.Next(0, 2) == 0 ? -1f : 1f;
        Vector3 debugDirection = new Vector3(x, 1f, z);
        float magnitude = Mathf.Lerp(randomOffsetMin, randomOffsetMax, (float)random.NextDouble());
        return debugDirection * magnitude;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDebugTrajectoryCloud || duration <= 0f)
        {
            return;
        }

        int trajectoryCount = Mathf.Max(1, debugTrajectoryCount);
        int sampleCount = Mathf.Max(2, debugSamplesPerTrajectory);
        System.Random random = new(debugSeed);
        Vector3 origin = Application.isPlaying ? spawnPosition : transform.position;

        Color previousColor = Gizmos.color;
        Gizmos.color = debugTrajectoryColor;

        for (int i = 0; i < trajectoryCount; i++)
        {
            Vector3 debugOffset = GenerateDebugRandomOffset(random);
            Vector3 previousPoint = origin + EvaluateCurveOffset(debugOffset, 0f);

            for (int sample = 1; sample <= sampleCount; sample++)
            {
                float t = sample / (float)sampleCount;
                Vector3 currentPoint = origin + EvaluateCurveOffset(debugOffset, t);
                Gizmos.DrawLine(previousPoint, currentPoint);
                Gizmos.DrawSphere(currentPoint, debugPointRadius);
                previousPoint = currentPoint;
            }
        }

        Gizmos.color = previousColor;
    }

    private void ReturnToPool()
    {
        System.Action<DamageNumber> callback = releaseCallback;
        releaseCallback = null;
        callback?.Invoke(this);
    }
}
