using UnityEngine;
using UnityEngine.UI;

public class ButtonFormat : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Image image = GetComponent<Image>();
        image.alphaHitTestMinimumThreshold = 0.1f;
    }
}
