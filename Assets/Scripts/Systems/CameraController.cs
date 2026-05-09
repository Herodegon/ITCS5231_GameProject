using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;

    [SerializeField] private Vector3 cameraOffset = new(10f, 10f, 0f);
    [SerializeField] private Vector3 cameraRotation = new(45f, -90f, 0f);
    [SerializeField] private float cameraFollowSpeed = 5f;
    [Header("Combat Camera")]
    [SerializeField] private float combatMinDistance = 4f;
    [SerializeField] private float combatMaxDistance = 20f;
    [SerializeField] private float combatMinHeight = 10f;
    [SerializeField] private float combatMaxHeight = 18f;
    [SerializeField] private float combatEdgePadding = 1.5f;

    private bool isCombatViewActive = false;
    private Transform combatPlayerTarget;
    private Transform combatFishTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeCamera();
    }

    public void SetTarget(GameObject targetObject)
    {
        target = targetObject;
        InitializeCamera();
    }

    public void EnterCombatView(Transform player, Transform fish)
    {
        if (player == null || fish == null) return;

        combatPlayerTarget = player;
        combatFishTarget = fish;
        isCombatViewActive = true;
    }

    public void ExitCombatView()
    {
        isCombatViewActive = false;
        combatPlayerTarget = null;
        combatFishTarget = null;
    }

    private void LateUpdate()
    {
        if (isCombatViewActive)
        {
            if (combatPlayerTarget == null || combatFishTarget == null)
            {
                ExitCombatView();
            }
            else
            {
                MoveCombatCamera(combatPlayerTarget, combatFishTarget);
                return;
            }
        }

        if (target != null)
        {
            MoveCamera(target.transform);
        }
    }

    private void InitializeCamera()
    {
        if (target == null) return;

        Vector3 pos = new(
            target.transform.position.x + cameraOffset.x,
            cameraOffset.y,
            target.transform.position.z + cameraOffset.z
        );
        Quaternion rot = Quaternion.Euler(cameraRotation);
        transform.SetPositionAndRotation(pos, rot);
    }

    private void MoveCamera(Transform targetTransform)
    {
        Vector3 newPosition = new(targetTransform.position.x + cameraOffset.x, cameraOffset.y, targetTransform.position.z + cameraOffset.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * cameraFollowSpeed);
    }

    private void MoveCombatCamera(Transform playerTarget, Transform fishTarget)
    {
        Vector3 playerPos = playerTarget.position;
        Vector3 fishPos = fishTarget.position;
        Vector3 midpoint = (playerPos + fishPos) * 0.5f;

        float separation = Vector3.Distance(playerPos, fishPos) + combatEdgePadding;
        float rangeT = Mathf.InverseLerp(combatMinDistance, combatMaxDistance, separation);
        float dynamicHeight = Mathf.Lerp(combatMinHeight, combatMaxHeight, rangeT);

        Vector3 desiredPosition = new(
            midpoint.x + cameraOffset.x,
            dynamicHeight,
            midpoint.z + cameraOffset.z
        );

        transform.rotation = Quaternion.Euler(cameraRotation);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * cameraFollowSpeed);
    }
}
