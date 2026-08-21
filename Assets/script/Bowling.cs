using UnityEngine;
using UnityEngine.InputSystem;

public class Bowling : MonoBehaviour
{
    [SerializeField]
    private float forcePower = 20f;

    [SerializeField]
    private float aimSpeed = 25f;

    [SerializeField]
    private float aimLimit = 20f;

    [SerializeField]
    private float stopSpeed = 0.1f;

    [SerializeField]
    private Transform aimGuide;

    [SerializeField]
    private float guideDistance = 4f;

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

        Aim();
        ShowGuide(true);

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            ShootBall();
    }

    private void Aim()
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

        aimAngle = Mathf.Clamp(aimAngle + move * aimSpeed * Time.deltaTime, -aimLimit, aimLimit);
        ShowAim();
    }

    private void ShowAim()
    {
        if (UIManager.instance != null)
            UIManager.instance.ShowAim(aimAngle);
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

        rb.AddForce(AimDir * forcePower, ForceMode.Impulse);
        ShowGuide(false);

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
