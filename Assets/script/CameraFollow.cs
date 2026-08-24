using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float height = 6f;

    [SerializeField]
    private float back = 14f;

    [SerializeField]
    private float lookAhead = 14f;

    [SerializeField]
    private float lookHeight = 1f;

    [SerializeField]
    private float sideFollow = 0.35f;

    [SerializeField]
    private float followSpeed = 3f;

    void LateUpdate()
    {
        if (target == null)
            return;

        float x = target.position.x * sideFollow;
        Vector3 want = new Vector3(x, height, target.position.z - back);
        Vector3 lookAt = new Vector3(x, lookHeight, target.position.z + lookAhead);

        transform.position = Vector3.Lerp(transform.position, want, followSpeed * Time.deltaTime);

        Vector3 look = lookAt - transform.position;

        if (look.sqrMagnitude < 0.01f)
            return;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), followSpeed * Time.deltaTime);
    }
}
