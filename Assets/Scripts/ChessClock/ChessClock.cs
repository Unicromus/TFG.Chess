using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChessClock : MonoBehaviour
{

    [SerializeField] private GameObject victoryScreen;

    [SerializeField] private GameObject clockTopButtons;

    [SerializeField] private Chessboard chessBoard; // Tablero de ajedrez.

    public TMP_Text textTimerWhite; // The text we gonna change for the white timer
    public TMP_Text textTimerBlack; // The text we gonna change for the black timer

    private const float GAME_OVER_TIME = 0.0f;
    private const float SECONDS_IN_MINUTE = 60.0f;
    private const float TEN_MINUTES = 10 * SECONDS_IN_MINUTE;

    private float timerWhite; // The white timer
    private float timerBlack; // The black timer

    private bool isTimerWhite; // The white timer is running
    private bool isTimerBlack; // The black timer is running

    private bool isWhiteTurn; // Which turn? TRUE --> WHITE | FALSE --> BLACK

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
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerWhite)
        {
            if (timerWhite > GAME_OVER_TIME)
            {
                timerWhite -= Time.deltaTime;
                DisplayWhiteTime();
            }
            else // White runs out of time. Black wins.
            {
                StopTimer();
                timerWhite = GAME_OVER_TIME;
                DisplayWhiteTime();
                DisplayVictory((int)Team.Black);
            }
        }
        else if (isTimerBlack)
        {
            if (timerBlack > GAME_OVER_TIME)
            {
                timerBlack -= Time.deltaTime;
                DisplayBlackTime();
            }
            else // Black runs out of time. White wins.
            {
                StopTimer();
                timerBlack = GAME_OVER_TIME;
                DisplayBlackTime();
                DisplayVictory(((int)Team.White));
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
        if (isTimerWhite && !isTimerBlack && isWhiteTurn && !chessBoard.GetIsWhiteTurn())
        {
            isTimerWhite = !isTimerWhite;   // = false
            isTimerBlack = !isTimerBlack;   // = true
            isWhiteTurn = !isWhiteTurn;     // = false

            clockTopButtons.transform.Rotate(new Vector3(0, 0, -5.5f));
        }
    }
    public void BlackSwap()
    {
        if (!isTimerWhite && isTimerBlack && !isWhiteTurn && chessBoard.GetIsWhiteTurn())
        {
            isTimerWhite = !isTimerWhite;   // true
            isTimerBlack = !isTimerBlack;   // false
            isWhiteTurn = !isWhiteTurn;     // true

            clockTopButtons.transform.Rotate(new Vector3(0, 0, +5.5f));
        }
    }

    public void StartTimer()
    {
        if (isWhiteTurn)
            isTimerWhite = true;
        else if (!isWhiteTurn)
            isTimerBlack = true;
    }
    public void StopTimer()
    {
        isTimerWhite = false;
        isTimerBlack = false;
    }
    public void ResetTimer()
    {
        StopTimer();
        timerWhite = TEN_MINUTES;
        timerBlack = TEN_MINUTES;
        isWhiteTurn = true;
        DisplayWhiteTime();
        DisplayBlackTime();

        if (!isWhiteTurn)
            clockTopButtons.transform.Rotate(new Vector3(0, 0, +5.5f));
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
