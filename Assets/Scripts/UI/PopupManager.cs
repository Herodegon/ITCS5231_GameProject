using UnityEngine;
using TMPro;

public class PopupManager : MonoBehaviour
{
    public GameObject damagePopupPrefab;
    public float verticalOffset = 1f;

    // Update is called once per frame
    void Update()
    {
        return;
    }

    public void GenDamagePopup(float damage, Vector3 position)
    {
        GameObject popup = Instantiate(damagePopupPrefab, position + new Vector3(0f, verticalOffset, 0f), Quaternion.identity);
        popup.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        popup.transform.SetParent(transform);
    }
}
