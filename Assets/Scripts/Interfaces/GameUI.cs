using System;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { set; get; }

    [Header("Animation")]
    [SerializeField] private Animator menuAnimator;

    [Header("Screens")]
    [SerializeField] private GameObject screenInGameUI;
    [SerializeField] private GameObject screenCanvasClock;

    [Header("Scripts Logic")]
    [SerializeField] private Chessboard chessBoard; // Tablero de Ajedrez
    [SerializeField] private ChessClock chessClock; // Reloj Digital
    [SerializeField] private CameraMovement cameraMovement; // La camara

    [Header("Inputs")]
    [SerializeField] private TMP_InputField textForsythEdwardsNotation;

    [Header("Sounds")]
    [SerializeField] private AudioClip declineSoundClip;
    [SerializeField] private AudioClip clickSoundClip;

    private void Awake()
    {
        Instance = this;
    }

    /* Buttons GameUI */
    public void OnLocalGameButton()
    {
        menuAnimator.SetTrigger("LocalMenu");

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
    }
    public void OnOnlineGameButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
    }

    /* Buttons LocalMenu */
    public void OnOnePlayerButton()
    {
        chessBoard.SetGameMode(GameMode.OnePlayer);
        menuAnimator.SetTrigger("TeamMenu");

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
    }
    public void OnPlayerVSRandomAIButton()
    {
        chessBoard.SetGameMode(GameMode.PlayerVSRandomBot);
        menuAnimator.SetTrigger("TeamMenu");

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
    }
    public void OnPlayerVSAggressiveRandomAIButton()
    {
        chessBoard.SetGameMode(GameMode.PlayerVSAggressiveRandomBot);
        menuAnimator.SetTrigger("TeamMenu");

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
    }
    public void OnLocalBackButton()
    {
        menuAnimator.SetTrigger("GameUI");

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(declineSoundClip, transform, 1f);
    }

    /* Buttons TeamMenu */
    public void OnWhiteTeamButton()
    {
        chessBoard.SetPlayerTeam(Team.White);
        menuAnimator.SetTrigger("InGameUI");
        chessBoard.OnStartButton();

        cameraMovement.MoveCameraToWhiteSide();

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
    }
    public void OnBlackTeamButton()
    {
        chessBoard.SetPlayerTeam(Team.Black);
        menuAnimator.SetTrigger("InGameUI");
        chessBoard.OnStartButton();

        cameraMovement.MoveCameraToBlackSide();

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
    }
    public void OnTeamBackButton()
    {
        menuAnimator.SetTrigger("LocalMenu");

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(declineSoundClip, transform, 1f);
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
            screenInGameUI.transform.GetChild(5).gameObject.SetActive(false); // Change Camera View
            screenInGameUI.transform.GetChild(6).gameObject.SetActive(false); // Spacer 01
            screenInGameUI.transform.GetChild(7).gameObject.SetActive(false); // ReStart
            screenInGameUI.transform.GetChild(8).gameObject.SetActive(false); // Menu
            screenInGameUI.transform.GetChild(9).gameObject.SetActive(false); // Spacer 02
        }
        else
        {
            screenInGameUI.transform.GetChild(0).gameObject.SetActive(true); // Set Forsyth Edwards Notation
            screenInGameUI.transform.GetChild(1).gameObject.SetActive(true); // Get Forsyth Edwards Notation
            screenInGameUI.transform.GetChild(2).gameObject.SetActive(true); // Spacer
            screenInGameUI.transform.GetChild(3).gameObject.SetActive(true); // UI Clock
            screenInGameUI.transform.GetChild(4).gameObject.SetActive(true); // Change Table
            screenInGameUI.transform.GetChild(5).gameObject.SetActive(true); // Change Camera View
            screenInGameUI.transform.GetChild(6).gameObject.SetActive(true); // Spacer 01
            screenInGameUI.transform.GetChild(7).gameObject.SetActive(true); // ReStart
            screenInGameUI.transform.GetChild(8).gameObject.SetActive(true); // Menu
            screenInGameUI.transform.GetChild(9).gameObject.SetActive(true); // Spacer 02
        }
        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
    }

    public void OnGetForsythEdwardsNotation()
    {
        textForsythEdwardsNotation.text = chessBoard.OnGetForsythEdwardsNotation();
    }
    public void OnSetForsythEdwardsNotation(String chessS)
    {
        chessBoard.OnSetForsythEdwardsNotation(chessS);
    }

    public void OnCanvasClock()
    {
        if (screenCanvasClock.activeInHierarchy)
            screenCanvasClock.SetActive(false);
        else
            screenCanvasClock.SetActive(true);
        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
    }

    public void OnMenuButton()
    {
        menuAnimator.SetTrigger("GameUI");

        cameraMovement.MoveCameraToMenuSide();

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(declineSoundClip, transform, 1f);
    }
    public void OnResetButton()
    {
        chessBoard.OnResetButton();
        chessClock.ResetTimer();
    }
    public void OnExitButton()
    {
        chessBoard.OnExitButton();
    }

    /* Buttons InGameUI - Promotion Buttons */
    public void OnRookPromotion()
    {
        chessBoard.OnSetPromotionPiece(PromotionPieceType.Rook);
    }
    public void OnKnightPromotion()
    {
        chessBoard.OnSetPromotionPiece(PromotionPieceType.Knight);
    }
    public void OnBishopPromotion()
    {
        chessBoard.OnSetPromotionPiece(PromotionPieceType.Bishop);
    }
    public void OnQueenPromotion()
    {
        chessBoard.OnSetPromotionPiece(PromotionPieceType.Queen);
    }

    /* Buttons OnlineMenu */
    public void OnOnlineHostButton()
    {
        menuAnimator.SetTrigger("HostMenu");

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
    }
    public void OnOnlineConnectButton()
    {
        Debug.Log("OnOnlineConnectButton"); // Other logic
        
        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
    }
    public void OnOnlineBackButton()
    {
        menuAnimator.SetTrigger("GameUI");

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(declineSoundClip, transform, 1f);
    }

    /* Buttons HostMenu */
    public void OnHostBackButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(declineSoundClip, transform, 1f);
    }

}
