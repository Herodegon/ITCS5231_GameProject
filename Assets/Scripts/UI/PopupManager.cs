using UnityEngine;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    public GameObject damagePopupPrefab;
    public Vector3 popupOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private int initialPoolSize = 16;
    [SerializeField] private int maxPoolSize = 64;

    private readonly Queue<DamageNumber> availablePopups = new Queue<DamageNumber>();
    private int totalPoolCount = 0;

    void Start()
    {
        PrewarmPool();
    }

    public void GenDamagePopup(float damage, Vector3 position)
    {
        DamageNumber popup = AcquirePopup();
        if (popup == null) return;

        Transform popupTransform = popup.transform;
        popupTransform.SetParent(transform, false);
        popupTransform.position = position + popupOffset;
        popup.gameObject.SetActive(true);
        popup.Initialize(damage, ReleasePopup);
    }

    private void PrewarmPool()
    {
        if (damagePopupPrefab == null) return;
        int targetPoolSize = Mathf.Max(0, initialPoolSize);
        for (int i = 0; i < targetPoolSize; i++)
        {
            DamageNumber popup = CreatePopupInstance();
            if (popup == null) break;
            ReleasePopup(popup);
        }
    }

    private DamageNumber AcquirePopup()
    {
        while (availablePopups.Count > 0)
        {
            DamageNumber popup = availablePopups.Dequeue();
            if (popup != null)
            {
                return popup;
            }
            totalPoolCount = Mathf.Max(0, totalPoolCount - 1);
        }

        if (maxPoolSize > 0 && totalPoolCount >= maxPoolSize)
        {
            return null;
        }

        return CreatePopupInstance();
    }

    private DamageNumber CreatePopupInstance()
    {
        if (damagePopupPrefab == null) return null;

        GameObject popupObject = Instantiate(damagePopupPrefab, transform);
        DamageNumber popup = popupObject.GetComponent<DamageNumber>();
        if (popup == null)
        {
            popup = popupObject.AddComponent<DamageNumber>();
        }
        popupObject.SetActive(false);
        totalPoolCount++;
        return popup;
    }

    private void ReleasePopup(DamageNumber popup)
    {
        if (popup == null) return;

        popup.gameObject.SetActive(false);
        popup.transform.SetParent(transform, false);
        availablePopups.Enqueue(popup);
    }

    private void OnDestroy()
    {
        availablePopups.Clear();
        totalPoolCount = 0;
    }
}
