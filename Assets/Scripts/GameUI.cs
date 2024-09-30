using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { set; get; }

    [SerializeField] private Animator menuAnimator;

    [SerializeField] private GameObject screenInGameUI;
    [SerializeField] private GameObject screenCanvasClock;

    [SerializeField] private Chessboard chessBoard;

    public TMP_InputField textForsythEdwardsNotation;

    private void Awake()
    {
        Instance = this;
    }

    /* Buttons GameUI */
    public void OnLocalGameButton()
    {
        menuAnimator.SetTrigger("LocalMenu");
    }
    public void OnOnlineGameButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");
    }

    /* Buttons LocalMenu */
    public void OnOnePlayerButton()
    {
        chessBoard.SetGameMode(GameMode.OnePlayer);
        menuAnimator.SetTrigger("TeamMenu");
    }
    public void OnPlayerVSRandomAIButton()
    {
        chessBoard.SetGameMode(GameMode.PlayerVSRandomBot);
        menuAnimator.SetTrigger("TeamMenu");
    }
    public void OnPlayerVSAggressiveRandomAIButton()
    {
        chessBoard.SetGameMode(GameMode.PlayerVSAggressiveRandomBot);
        menuAnimator.SetTrigger("TeamMenu");
    }
    public void OnLocalBackButton()
    {
        menuAnimator.SetTrigger("GameUI");
    }

    /* Buttons TeamMenu */
    public void OnWhiteTeamButton()
    {
        chessBoard.SetPlayerTeam(Team.White);
        menuAnimator.SetTrigger("InGameUI");
        chessBoard.OnStartButton();
    }
    public void OnBlackTeamButton()
    {
        chessBoard.SetPlayerTeam(Team.Black);
        menuAnimator.SetTrigger("InGameUI");
        chessBoard.OnStartButton();
    }
    public void OnTeamBackButton()
    {
        menuAnimator.SetTrigger("LocalMenu");
    }

    /* Buttons InGameUI */

    public void OnOptionsButton()
    {
        if (screenInGameUI.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            screenInGameUI.transform.GetChild(0).gameObject.SetActive(false); // Set Forsyth Edwards Notation
            screenInGameUI.transform.GetChild(1).gameObject.SetActive(false); // Get Forsyth Edwards Notation
            screenInGameUI.transform.GetChild(2).gameObject.SetActive(false); // Spacer
            screenInGameUI.transform.GetChild(3).gameObject.SetActive(false); // UI Clock
            screenInGameUI.transform.GetChild(4).gameObject.SetActive(false); // Change Table
            screenInGameUI.transform.GetChild(5).gameObject.SetActive(false); // Spacer 01
            screenInGameUI.transform.GetChild(6).gameObject.SetActive(false); // ReStart
            screenInGameUI.transform.GetChild(7).gameObject.SetActive(false); // Menu
            screenInGameUI.transform.GetChild(8).gameObject.SetActive(false); // Spacer 02
        }
        else
        {
            screenInGameUI.transform.GetChild(0).gameObject.SetActive(true); // Set Forsyth Edwards Notation
            screenInGameUI.transform.GetChild(1).gameObject.SetActive(true); // Get Forsyth Edwards Notation
            screenInGameUI.transform.GetChild(2).gameObject.SetActive(true); // Spacer
            screenInGameUI.transform.GetChild(3).gameObject.SetActive(true); // UI Clock
            screenInGameUI.transform.GetChild(4).gameObject.SetActive(true); // Change Table
            screenInGameUI.transform.GetChild(5).gameObject.SetActive(true); // Spacer 01
            screenInGameUI.transform.GetChild(6).gameObject.SetActive(true); // ReStart
            screenInGameUI.transform.GetChild(7).gameObject.SetActive(true); // Menu
            screenInGameUI.transform.GetChild(8).gameObject.SetActive(true); // Spacer 02
        }
    }

    public void OnGetForsythEdwardsNotation()
    {
        textForsythEdwardsNotation.text = chessBoard.OnGetForsythEdwardsNotation();
    }
    public void OnSetForsythEdwardsNotation(String chessS)
    {
        chessBoard.OnSetForsythEdwardsNotation(chessS);
    }

    public void OnMenuButton()
    {
        menuAnimator.SetTrigger("GameUI");
    }

    public void OnCanvasClock()
    {
        if (screenCanvasClock.activeInHierarchy)
            screenCanvasClock.SetActive(false);
        else
            screenCanvasClock.SetActive(true);
    }

    public void OnResetButton()
    {
        chessBoard.OnResetButton();
    }
    public void OnExitButton()
    {
        chessBoard.OnExitButton();
    }

    /* Buttons OnlineMenu */
    public void OnOnlineHostButton()
    {
        menuAnimator.SetTrigger("HostMenu");
    }
    public void OnOnlineConnectButton()
    {
        Debug.Log("OnOnlineConnectButton"); // Other logic
    }
    public void OnOnlineBackButton()
    {
        menuAnimator.SetTrigger("GameUI");
    }

    /* Buttons HostMenu */
    public void OnHostBackButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");
    }

}
