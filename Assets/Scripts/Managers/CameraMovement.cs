using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

public class CameraMovement : MonoBehaviour
{
    [Header("Chess Board")]
    [SerializeField] private Chessboard chessBoard;

    [Header("Positions")]
    private Vector3 offsetPositionMenuSide = new Vector3(-0.5f, 0.36f, 0f);
    private Vector3 offsetPositionWhiteSide = new Vector3(0f, 0.36f, -0.5f);
    private Vector3 offsetPositionBlackSide = new Vector3(0f, 0.36f, 0.5f);
    private Vector3 offsetPositionTopWhiteSide = new Vector3(0f, 0.56f, 0f);
    private Vector3 offsetPositionTopBlackSide = new Vector3(0f, 0.56f, 0f);

    [Header("Rotations")]
    private Quaternion offsetRotationMenuSide = Quaternion.Euler(35f, 90f, 0f);
    private Quaternion offsetRotationWhiteSide = Quaternion.Euler(35f, 0f, 0f);
    private Quaternion offsetRotationBlackSide = Quaternion.Euler(35f, 180f, 0f);
    private Quaternion offsetRotationTopWhiteSide = Quaternion.Euler(90f, 0f, 0f);
    private Quaternion offsetRotationTopBlackSide = Quaternion.Euler(90f, 180f, 0f);

    [Header("Target")]
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Quaternion targetRotation;

    [Header("Smoothness|Speed")]
    [SerializeField] private float smoothness = 0.5f;
    [SerializeField] private float speed = 50f;

    [Header("Sounds")]
    [SerializeField] private AudioClip changeCameraViewSoundClip;

    private int cameraView = 0;
    private int maxCameraView = 2;

    private void Start()
    {
        targetPosition = chessBoard.gameObject.transform.position + offsetPositionMenuSide;
        targetRotation = chessBoard.gameObject.transform.rotation * offsetRotationMenuSide;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, smoothness * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed * Time.deltaTime);
    }

    public void ChangeView()
    {
        cameraView++;
        if (cameraView >= maxCameraView)
            cameraView = 0;

        switch (cameraView)
        {
            case 0:
                if (chessBoard.GetPlayerTeam() == Team.White)
                    MoveCameraToWhiteSide();
                else if (chessBoard.GetPlayerTeam() == Team.Black)
                    MoveCameraToBlackSide();
                break;
            case 1:
                if (chessBoard.GetPlayerTeam() == Team.White)
                    MoveCameraToTopWhiteSide();
                else if (chessBoard.GetPlayerTeam() == Team.Black)
                    MoveCameraToTopBlackSide();
                break;
            default:
                break;
        }

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(changeCameraViewSoundClip, transform, 1f);
    }

    public void MoveCameraToMenuSide()
    {
        targetPosition = chessBoard.gameObject.transform.TransformPoint(offsetPositionMenuSide);
        targetRotation = chessBoard.gameObject.transform.rotation * offsetRotationMenuSide;

        cameraView = 0;
    }
    public void MoveCameraToWhiteSide()
    {
        targetPosition = chessBoard.gameObject.transform.TransformPoint(offsetPositionWhiteSide);
        targetRotation = chessBoard.gameObject.transform.rotation * offsetRotationWhiteSide;

        cameraView = 0;
    }
    public void MoveCameraToBlackSide()
    {
        targetPosition = chessBoard.gameObject.transform.TransformPoint(offsetPositionBlackSide);
        targetRotation = chessBoard.gameObject.transform.rotation * offsetRotationBlackSide;

        cameraView = 0;
    }
    private void MoveCameraToTopWhiteSide()
    {
        targetPosition = chessBoard.gameObject.transform.TransformPoint(offsetPositionTopWhiteSide);
        targetRotation = chessBoard.gameObject.transform.rotation * offsetRotationTopWhiteSide;

        cameraView = 1;
    }
    private void MoveCameraToTopBlackSide()
    {
        targetPosition = chessBoard.gameObject.transform.TransformPoint(offsetPositionTopBlackSide);
        targetRotation = chessBoard.gameObject.transform.rotation * offsetRotationTopBlackSide;

        cameraView = 1;
    }

}
