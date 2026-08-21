using UnityEngine;
using UnityEngine.InputSystem;

public class Bowling : MonoBehaviour
{
    [SerializeField]
    private float forcePower = 20f;

    [SerializeField]
    private float moveSpeed = 4f;

    [SerializeField]
    private float moveLimit = 2f;

    [SerializeField]
    private float stopSpeed = 0.1f;

    private Rigidbody rb;
    private Vector3 startPosition;

    public bool IsMoving { get { return rb.linearVelocity.magnitude > stopSpeed; } }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (GameManager.instance != null && !GameManager.instance.CanShoot)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            ShootBall();

        if (Keyboard.current.rightArrowKey.isPressed)
            Move(1f);

        if (Keyboard.current.leftArrowKey.isPressed)
            Move(-1f);
    }

    public void ShootBall()
    {
        if (GameManager.instance != null && !GameManager.instance.CanShoot)
            return;

        rb.AddForce(Vector3.forward * forcePower, ForceMode.Impulse);

        if (GameManager.instance != null)
            GameManager.instance.StartRoll();
    }

    private void Move(float direction)
    {
        float x = Mathf.Clamp(transform.position.x + direction * moveSpeed * Time.deltaTime, -moveLimit, moveLimit);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    public void ResetBall()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPosition;
    }
}
