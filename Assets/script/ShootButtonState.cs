using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Greys out the Shoot button while a roll is in progress, so it visibly
/// matches the fact that clicking it does nothing until the ball and pins
/// settle (Bowling.ShootBall already guards on GameManager.CanShoot).
/// </summary>
public class ShootButtonState : MonoBehaviour
{
    [SerializeField]
    private Button shootButton;

    void Awake()
    {
        if (shootButton == null)
            shootButton = GetComponent<Button>();
    }

    void Update()
    {
        if (shootButton == null || GameManager.instance == null)
            return;

        shootButton.interactable = GameManager.instance.CanShoot;
    }
}
