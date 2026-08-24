using UnityEngine;
using UnityEngine.InputSystem;

public class Bowling : MonoBehaviour
{
    [SerializeField]
    private float forcePower = 34f;

    [SerializeField]
    private float aimSpeed = 6f;

    [SerializeField]
    private float aimLimit = 4f;

    [SerializeField]
    private float moveSpeed = 6f;

    [SerializeField]
    private float moveLimit = 2.2f;

    [SerializeField]
    private float stopSpeed = 0.1f;

    [SerializeField]
    private Transform aimGuide;

    [SerializeField]
    private float guideDistance = 8f;

    private Rigidbody rb;
    private Vector3 startPosition;
    private float aimAngle;

    public float AimAngle { get { return aimAngle; } }
    public Vector3 AimDir { get { return Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward; } }
    public bool IsMoving { get { return rb.linearVelocity.magnitude > stopSpeed; } }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        ShowAim();
    }

    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.CanShoot)
        {
            ShowGuide(false);
            return;
        }

        HoldStill();
        Aim();
        Move();
        ShowGuide(true);

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            ShootBall();
    }

    private void HoldStill()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void Aim()
    {
        if (Keyboard.current == null)
            return;

        float move = 0f;

        if (Keyboard.current.qKey.isPressed)
            move -= 1f;

        if (Keyboard.current.eKey.isPressed)
            move += 1f;

        if (move == 0f)
            return;

        aimAngle = Mathf.Clamp(aimAngle + move * aimSpeed * Time.deltaTime, -aimLimit, aimLimit);
        ShowAim();
    }

    private void Move()
    {
        if (Keyboard.current == null)
            return;

        float move = 0f;

        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            move -= 1f;

        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            move += 1f;

        if (move == 0f)
            return;

        float x = Mathf.Clamp(transform.position.x + move * moveSpeed * Time.deltaTime, -moveLimit, moveLimit);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
        ShowAim();
    }

    private void ShowAim()
    {
        if (UIManager.instance != null)
            UIManager.instance.ShowAim(aimAngle, transform.position.x);
    }

    private void ShowGuide(bool show)
    {
        if (aimGuide == null)
            return;

        if (aimGuide.gameObject.activeSelf != show)
            aimGuide.gameObject.SetActive(show);

        if (!show)
            return;

        Vector3 dir = AimDir;
        aimGuide.position = transform.position + dir * guideDistance;
        aimGuide.rotation = Quaternion.LookRotation(dir);
    }

    public void ShootBall()
    {
        if (GameManager.instance != null && !GameManager.instance.CanShoot)
            return;

        rb.AddForce(AimDir * forcePower, ForceMode.VelocityChange);
        ShowGuide(false);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayRoll();

        if (GameManager.instance != null)
            GameManager.instance.StartRoll();
    }

    public void ResetBall()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPosition;
        ShowAim();
    }
}
