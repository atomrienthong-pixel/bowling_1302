using UnityEngine;

public class Pin : MonoBehaviour
{
    [SerializeField]
    private float downAngle = 45f;

    [SerializeField]
    private float minHeight = 1.2f;

    [SerializeField]
    private float stopSpeed = 0.1f;

    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;

    public bool IsDown
    {
        get
        {
            if (!gameObject.activeSelf)
                return true;

            if (transform.position.y < minHeight)
                return true;

            return Vector3.Angle(transform.up, Vector3.up) > downAngle;
        }
    }

    public bool IsMoving
    {
        get
        {
            if (!gameObject.activeSelf)
                return false;

            return rb.linearVelocity.magnitude > stopSpeed || rb.angularVelocity.magnitude > stopSpeed;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void Stop()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void ResetPin()
    {
        gameObject.SetActive(true);
        Stop();
        transform.SetPositionAndRotation(startPosition, startRotation);
    }

    public void Remove()
    {
        Stop();
        gameObject.SetActive(false);
    }
}
