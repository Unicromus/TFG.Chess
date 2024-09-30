using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChessClock : MonoBehaviour
{

    [SerializeField] private GameObject victoryScreen;

    [SerializeField] private GameObject clockTopButtons;

    public TMP_Text textTimerWhite; // The text we gonna change for the white timer
    public TMP_Text textTimerBlack; // The text we gonna change for the black timer

    private const float gameOverTime = 0.0f;
    private const float secondsInMinute = 60.0f;
    private const float tenMinutes = 10 * secondsInMinute;

    [SerializeField] private float timerWhite = tenMinutes; // The white timer
    [SerializeField] private float timerBlack = tenMinutes; // The black timer

    private bool isTimerWhite = false; // The white timer is running
    private bool isTimerBlack = false; // The black timer is running

    public bool isWhiteTurn; // Which turn?

    private int whiteTeam = 0;
    private int blackTeam = 1;

    // Start is called before the first frame update
    void Awake()
    {
        isWhiteTurn = true;
        timerWhite = tenMinutes; // Set the white timer to 10 minutes; 10 * 60 = 600s
        timerBlack = tenMinutes; // Set the black timer to 10 minutes; 10 * 60 = 600s
        DisplayWhiteTime();
        DisplayBlackTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerWhite)
        {
            if (timerWhite > gameOverTime)
            {
                timerWhite -= Time.deltaTime;
                DisplayWhiteTime();
            }
            else // White runs out of time. Black wins.
            {
                StopTimer();
                timerWhite = gameOverTime;
                DisplayWhiteTime();
                DisplayVictory(blackTeam);
            }
        }
        else if (isTimerBlack)
        {
            if (timerBlack > gameOverTime)
            {
                timerBlack -= Time.deltaTime;
                DisplayBlackTime();
            }
            else // Black runs out of time. White wins.
            {
                StopTimer();
                timerBlack = gameOverTime;
                DisplayBlackTime();
                DisplayVictory(whiteTeam);
            }
        }
    }

    private void DisplayVictory(int winningTeam)
    {
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }

    private void DisplayWhiteTime()
    {
        int minutes = Mathf.FloorToInt(timerWhite / secondsInMinute); // Get the minutes of the white timer: 5.4 --> 5 minutes | 5.6 --> 5 minutes
        int seconds = Mathf.FloorToInt(timerWhite - minutes * secondsInMinute); // Get the seconds of the white timer: 5.4 - 5 = 0.4 --> 0.4 * 60 = 24 seconds; 24.4 | 24.6 --> 24 seconds
        textTimerWhite.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void DisplayBlackTime()
    {
        int minutes = Mathf.FloorToInt(timerBlack / secondsInMinute); // Get the minutes of the black timer: 5.4 --> 5 minutes | 5.6 --> 5 minutes
        int seconds = Mathf.FloorToInt(timerBlack - minutes * secondsInMinute); // Get the seconds of the black timer: 5.4 - 5 = 0.4 --> 0.4 * 60 = 24 seconds; 24.4 | 24.6 --> 24 seconds
        textTimerBlack.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void WhiteSwap()
    {
        if (isTimerWhite && !isTimerBlack && isWhiteTurn)
        {
            isTimerWhite = !isTimerWhite;   // = false
            isTimerBlack = !isTimerBlack;   // = true
            isWhiteTurn = !isWhiteTurn;     // = false

            clockTopButtons.transform.Rotate(new Vector3(0, 0, -5.5f));
        }
    }

    public void BlackSwap()
    {
        if (!isTimerWhite && isTimerBlack && !isWhiteTurn)
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
        if (!isWhiteTurn)
            clockTopButtons.transform.Rotate(new Vector3(0, 0, +5.5f));

        StopTimer();
        isWhiteTurn = true;
        timerWhite = tenMinutes;
        timerBlack = tenMinutes;
        DisplayWhiteTime();
        DisplayBlackTime();
    }

}
