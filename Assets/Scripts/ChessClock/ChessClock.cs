using TMPro;
using UnityEngine;

public class ChessClock : MonoBehaviour
{
    [Header("Victory Screen")]
    [SerializeField] private GameObject victoryScreen;

    [Header("Clock Top Buttons")]
    [SerializeField] private GameObject clockTopButtons;

    [Header("Chess Board")]
    [SerializeField] private Chessboard chessBoard; // Tablero de ajedrez.

    [Header("Sounds")]
    [SerializeField] private AudioClip plusRightSoundClip;
    [SerializeField] private AudioClip minusLeftSoundClip;
    [SerializeField] private AudioClip startSoundClip;
    [SerializeField] private AudioClip stopResetSoundClip;
    [SerializeField] private AudioClip incorrectSoundClip;
    [SerializeField] private AudioClip thirtySecondsSoundClip;
    [SerializeField] private AudioClip gameWinSoundClip;
    [SerializeField] private AudioClip gameLoseSoundClip;

    public TMP_Text textTimerWhite; // The text we gonna change for the white timer
    public TMP_Text textTimerBlack; // The text we gonna change for the black timer

    private const float GAME_OVER_TIME = 0.0f;
    private const float LAST_THIRTY_SECONDS = 30.0f;
    private const float SECONDS_IN_MINUTE = 60.0f;
    private const float TEN_MINUTES = 10 * SECONDS_IN_MINUTE;
    private const float SIXTY_MINUTES = 60 * SECONDS_IN_MINUTE;

    private float timerWhite; // The white timer
    private float timerBlack; // The black timer

    private bool isTimerWhite; // The white timer is running
    private bool isTimerBlack; // The black timer is running

    private bool isWhiteTurn; // Which turn? TRUE --> WHITE | FALSE --> BLACK

    [SerializeField] private float ADD_TIME = 3.0f;
    private bool whiteLastThirtySeconds;
    private bool blackLastThirtySeconds;

    // Start is called before the first frame update
    void Awake()
    {
        
        timerWhite = TEN_MINUTES; // Set the white timer to 10 minutes; 10 * 60 = 600s
        timerBlack = TEN_MINUTES; // Set the black timer to 10 minutes; 10 * 60 = 600s
        isTimerWhite = false;
        isTimerBlack = false;
        isWhiteTurn = true;
        DisplayWhiteTime();
        DisplayBlackTime();
        whiteLastThirtySeconds = false;
        blackLastThirtySeconds = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerWhite)
        {
            if (timerWhite <= LAST_THIRTY_SECONDS & !whiteLastThirtySeconds)
            {
                whiteLastThirtySeconds = true;

                // play sound FX
                SoundFXManager.Instance.PlaySoundFXClip(thirtySecondsSoundClip, transform, 1f);
            }

            if (timerWhite > GAME_OVER_TIME)
            {
                timerWhite -= Time.deltaTime;
                DisplayWhiteTime();
            }
            else // White runs out of time. BLACK WINS.
            {
                isTimerWhite = false;
                isTimerBlack = false;
                timerWhite = GAME_OVER_TIME;
                DisplayWhiteTime();
                DisplayVictory((int)Team.Black);

                // play sound FX
                if (chessBoard.GetPlayerTeam() == Team.Black)
                    SoundFXManager.Instance.PlaySoundFXClip(gameWinSoundClip, transform, 1f);
                else if (chessBoard.GetPlayerTeam() == Team.White)
                    SoundFXManager.Instance.PlaySoundFXClip(gameLoseSoundClip, transform, 1f);
            }
        }
        else if (isTimerBlack)
        {
            if (timerBlack <= LAST_THIRTY_SECONDS & !blackLastThirtySeconds)
            {
                blackLastThirtySeconds = true;

                // play sound FX
                SoundFXManager.Instance.PlaySoundFXClip(thirtySecondsSoundClip, transform, 1f);
            }

            if (timerBlack > GAME_OVER_TIME)
            {
                timerBlack -= Time.deltaTime;
                DisplayBlackTime();
            }
            else // Black runs out of time. WHITE WINS.
            {
                isTimerWhite = false;
                isTimerBlack = false;
                timerBlack = GAME_OVER_TIME;
                DisplayBlackTime();
                DisplayVictory(((int)Team.White));

                // play sound FX
                if (chessBoard.GetPlayerTeam() == Team.White)
                    SoundFXManager.Instance.PlaySoundFXClip(gameWinSoundClip, transform, 1f);
                else if (chessBoard.GetPlayerTeam() == Team.Black)
                    SoundFXManager.Instance.PlaySoundFXClip(gameLoseSoundClip, transform, 1f);
            }
        }
    }

    /* UI - Display : Victory, Timers */
    private void DisplayVictory(int winningTeam)
    {
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }
    private void DisplayWhiteTime()
    {
        int minutes = Mathf.FloorToInt(timerWhite / SECONDS_IN_MINUTE); // Get the minutes of the white timer: 5.4 --> 5 minutes | 5.6 --> 5 minutes
        int seconds = Mathf.FloorToInt(timerWhite - minutes * SECONDS_IN_MINUTE); // Get the seconds of the white timer: 5.4 - 5 = 0.4 --> 0.4 * 60 = 24 seconds; 24.4 | 24.6 --> 24 seconds
        textTimerWhite.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    private void DisplayBlackTime()
    {
        int minutes = Mathf.FloorToInt(timerBlack / SECONDS_IN_MINUTE); // Get the minutes of the black timer: 5.4 --> 5 minutes | 5.6 --> 5 minutes
        int seconds = Mathf.FloorToInt(timerBlack - minutes * SECONDS_IN_MINUTE); // Get the seconds of the black timer: 5.4 - 5 = 0.4 --> 0.4 * 60 = 24 seconds; 24.4 | 24.6 --> 24 seconds
        textTimerBlack.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /* Buttons */
    public void WhiteSwap()
    {
        if (isTimerWhite && !isTimerBlack && isWhiteTurn && !chessBoard.GetIsWhiteTurn() && !chessBoard.GetIsPromoting())
        {
            isTimerWhite = !isTimerWhite;   // = false
            isTimerBlack = !isTimerBlack;   // = true
            isWhiteTurn = !isWhiteTurn;     // = false

            clockTopButtons.transform.Rotate(new Vector3(0, 0, -5.5f));

            if (whiteLastThirtySeconds)
            {
                timerWhite += ADD_TIME;
                DisplayWhiteTime();
                DisplayBlackTime();
            }

            // play sound FX
            SoundFXManager.Instance.PlaySoundFXClip(plusRightSoundClip, transform, 1f);
        }
        else
        {
            // play sound FX
            SoundFXManager.Instance.PlaySoundFXClip(incorrectSoundClip, transform, 1f);
        }
    }
    public void BlackSwap()
    {
        if (!isTimerWhite && isTimerBlack && !isWhiteTurn && chessBoard.GetIsWhiteTurn() && !chessBoard.GetIsPromoting())
        {
            isTimerWhite = !isTimerWhite;   // true
            isTimerBlack = !isTimerBlack;   // false
            isWhiteTurn = !isWhiteTurn;     // true

            clockTopButtons.transform.Rotate(new Vector3(0, 0, +5.5f));

            if (blackLastThirtySeconds)
            {
                timerBlack += ADD_TIME;
                DisplayWhiteTime();
                DisplayBlackTime();
            }

            // play sound FX
            SoundFXManager.Instance.PlaySoundFXClip(minusLeftSoundClip, transform, 1f);
        }
        else
        {
            // play sound FX
            SoundFXManager.Instance.PlaySoundFXClip(incorrectSoundClip, transform, 1f);
        }
    }

    public void PlusTimer()
    {
        if ((!isTimerWhite & !isTimerBlack) & (timerWhite < SIXTY_MINUTES & timerBlack < SIXTY_MINUTES))
        {
            timerWhite += SECONDS_IN_MINUTE;
            timerBlack += SECONDS_IN_MINUTE;
            DisplayWhiteTime();
            DisplayBlackTime();

            // play sound FX
            SoundFXManager.Instance.PlaySoundFXClip(plusRightSoundClip, transform, 1f);
        }
        else
        {
            SoundFXManager.Instance.PlaySoundFXClip(incorrectSoundClip, transform, 1f);
        }
    }
    public void MinusTimer()
    {
        if ((!isTimerWhite & !isTimerBlack) & (timerWhite > SECONDS_IN_MINUTE & timerBlack > SECONDS_IN_MINUTE))
        {
            timerWhite -= SECONDS_IN_MINUTE;
            timerBlack -= SECONDS_IN_MINUTE;
            DisplayWhiteTime();
            DisplayBlackTime();

            // play sound FX
            SoundFXManager.Instance.PlaySoundFXClip(minusLeftSoundClip, transform, 1f);
        }
        else
        {
            // play sound FX
            SoundFXManager.Instance.PlaySoundFXClip(incorrectSoundClip, transform, 1f);
        }
    }

    public void StartTimer()
    {
        if (!isTimerWhite & !isTimerBlack)
        {
            if (isWhiteTurn)
                isTimerWhite = true;
            else if (!isWhiteTurn)
                isTimerBlack = true;

            // play sound FX
            SoundFXManager.Instance.PlaySoundFXClip(startSoundClip, transform, 1f);
        }
        else
        {
            // play sound FX
            SoundFXManager.Instance.PlaySoundFXClip(incorrectSoundClip, transform, 1f);
        }
    }
    public void StopTimer()
    {
        if (isTimerWhite | isTimerBlack)
        {
            isTimerWhite = false;
            isTimerBlack = false;

            // play sound FX
            SoundFXManager.Instance.PlaySoundFXClip(stopResetSoundClip, transform, 1f);
        }
        else
        {
            // play sound FX
            SoundFXManager.Instance.PlaySoundFXClip(incorrectSoundClip, transform, 1f);
        }
    }
    public void ResetTimer()
    {
        isTimerWhite = false;
        isTimerBlack = false;
        timerWhite = TEN_MINUTES;
        timerBlack = TEN_MINUTES;
        if (!isWhiteTurn)
            clockTopButtons.transform.Rotate(new Vector3(0, 0, +5.5f));
        isWhiteTurn = true;
        DisplayWhiteTime();
        DisplayBlackTime();
        whiteLastThirtySeconds = false;
        blackLastThirtySeconds = false;

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(stopResetSoundClip, transform, 1f);
    }
    public void ResetTimerAndBoard()
    {
        ResetTimer();
        chessBoard.OnResetButton();
    }

    /* Getters & Setters */
    public bool GetIsTimerWhite()
    {
        return isTimerWhite;
    }
    public bool GetIsTimerBlack()
    {
        return isTimerBlack;
    }
    public bool GetIsWhiteTurn()
    {
        return isWhiteTurn;
    }

}
