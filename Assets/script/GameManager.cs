using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private Bowling ball;

    [SerializeField]
    private Transform pinRoot;

    [SerializeField]
    private int totalFrame = 10;

    [SerializeField]
    private int strikeBonus = 10;

    [SerializeField]
    private int spareBonus = 5;

    [SerializeField]
    private float minRollTime = 1.5f;

    [SerializeField]
    private float maxRollTime = 11f;

    private Pin[] pins;
    private int score;
    private int frame = 1;
    private int roll = 1;
    private int knockedThisFrame;
    private bool rolling;
    private bool gameOver;
    private float rollTimer;

    public int Score { get { return score; } }
    public bool CanShoot { get { return !rolling && !gameOver; } }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (pinRoot != null)
            pins = pinRoot.GetComponentsInChildren<Pin>(true);

        if (pins == null)
            pins = new Pin[0];

        ShowState();
    }

    void Update()
    {
        if (!rolling)
            return;

        rollTimer += Time.deltaTime;

        if (rollTimer < minRollTime)
            return;

        if (rollTimer < maxRollTime && SomethingMoving())
            return;

        EndRoll();
    }

    public void StartRoll()
    {
        if (!CanShoot)
            return;

        rolling = true;
        rollTimer = 0f;
    }

    private bool SomethingMoving()
    {
        if (ball != null && ball.IsMoving)
            return true;

        for (int i = 0; i < pins.Length; i++)
        {
            if (pins[i].IsMoving)
                return true;
        }

        return false;
    }

    private int CountKnocked()
    {
        int count = 0;

        for (int i = 0; i < pins.Length; i++)
        {
            if (pins[i].IsDown)
                count++;
        }

        return count;
    }

    private void EndRoll()
    {
        rolling = false;

        int knocked = CountKnocked();
        int gain = knocked - knockedThisFrame;
        knockedThisFrame = knocked;
        score += gain;

        if (roll == 1 && knocked == pins.Length)
        {
            score += strikeBonus;
            ShowMessage("Strike! +" + (gain + strikeBonus));
            NextFrame();
        }
        else if (roll == 2)
        {
            if (knocked == pins.Length)
            {
                score += spareBonus;
                ShowMessage("Spare! +" + (gain + spareBonus));
            }
            else
            {
                ShowMessage("Pin +" + gain);
            }

            NextFrame();
        }
        else
        {
            roll = 2;
            RemoveKnockedPins();
            ball.ResetBall();
            ShowMessage("Pin +" + gain);
        }

        ShowState();
    }

    private void NextFrame()
    {
        frame++;
        roll = 1;
        knockedThisFrame = 0;

        ball.ResetBall();
        ResetAllPins();

        if (frame > totalFrame)
            EndGame();
    }

    private void RemoveKnockedPins()
    {
        for (int i = 0; i < pins.Length; i++)
        {
            if (pins[i].IsDown)
                pins[i].Remove();
            else
                pins[i].Stop();
        }
    }

    private void ResetAllPins()
    {
        for (int i = 0; i < pins.Length; i++)
            pins[i].ResetPin();
    }

    private void ShowState()
    {
        if (UIManager.instance == null)
            return;

        UIManager.instance.ShowScore(score);
        UIManager.instance.ShowFrame(Mathf.Min(frame, totalFrame), totalFrame, roll);
    }

    private void ShowMessage(string message)
    {
        if (UIManager.instance != null)
            UIManager.instance.ShowMessage(message);
    }

    private void EndGame()
    {
        gameOver = true;

        if (UIManager.instance != null)
            UIManager.instance.ShowGameOver("Game Over\nScore: " + score);
    }
}
