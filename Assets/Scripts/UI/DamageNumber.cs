using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DamageNumber : MonoBehaviour
{
    public float duration = 0.75f;
    public float maxVelocity = 10f;
    public AnimationCurve alphaCurve;
    public AnimationCurve velocityCurve;

    private float timeElapsed = 0f;
    private float velocity = 0f;
    private float randomOffset = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
        randomOffset = Random.Range(-0.1f, 0.1f);
    }

    void LateUpdate()
    {
        timeElapsed += Time.deltaTime;
        UpdateAlpha();
        if (timeElapsed >= duration)
        {
            Destroy(gameObject);
        }
        UpdatePosition();
    }

    private void UpdateAlpha()
    {
        Color color = GetComponent<TextMeshProUGUI>().color;
        color.a = alphaCurve.Evaluate(Mathf.Clamp01(timeElapsed / duration));
        GetComponent<TextMeshProUGUI>().color = color;
    }

    private void UpdatePosition()
    {
        velocity = maxVelocity * velocityCurve.Evaluate(Mathf.Clamp01(timeElapsed / duration)) * (1f - randomOffset);
        transform.position += new Vector3(maxVelocity * randomOffset, velocity * Time.deltaTime, 0f);
    }
}
