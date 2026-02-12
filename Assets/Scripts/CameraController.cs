using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] public GameObject player;

    [SerializeField] private Vector3 cameraOffset = new Vector3(10f, 10f, 0f);
    [SerializeField] private Vector3 cameraRotation = new Vector3(45f, -90f, 0f);
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
        transform.localPosition = new Vector3(player.transform.position.x + cameraOffset.x, cameraOffset.y, player.transform.position.z + cameraOffset.z);
        transform.localRotation = Quaternion.Euler(cameraRotation);
    }

    private void MoveCamera(Transform objectToFollow)
    {
        Vector3 newPosition = new Vector3(objectToFollow.position.x + cameraOffset.x, cameraOffset.y, objectToFollow.position.z + cameraOffset.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * cameraFollowSpeed);
    }
}
