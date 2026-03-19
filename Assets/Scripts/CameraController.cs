using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    [SerializeField] private Vector3 cameraOffset = new(10f, 10f, 0f);
    [SerializeField] private Vector3 cameraRotation = new(45f, -90f, 0f);
    [SerializeField] private float cameraFollowSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeCamera();
    }

    private void LateUpdate()
    {
        MoveCamera(player.transform);
    }

    private void InitializeCamera()
    {
        Vector3 pos = new(
            player.transform.position.x + cameraOffset.x,
            cameraOffset.y,
            player.transform.position.z + cameraOffset.z
        );
        Quaternion rot = Quaternion.Euler(cameraRotation);
        transform.SetLocalPositionAndRotation(pos, rot);
    }

    private void MoveCamera(Transform objectToFollow)
    {
        Vector3 newPosition = new(objectToFollow.position.x + cameraOffset.x, cameraOffset.y, objectToFollow.position.z + cameraOffset.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * cameraFollowSpeed);
    }
}
