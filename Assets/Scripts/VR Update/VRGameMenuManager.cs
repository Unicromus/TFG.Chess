using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRGameMenuManager : MonoBehaviour
{
    [Header("VR Menu Settings")]

    [Tooltip("Canvas Menu")]
    public GameObject canvasMenu;

    [Tooltip("Input Action (Left Menu) from Left Controller to show/hide the menu.")]
    public InputActionProperty leftMenuButton;
    [Tooltip("Input Action (Right Menu) from Right Controller to move the menu.")]
    public InputActionProperty rightMenuButton;

    private CanvasGroup canvasGroup;
    private bool isVisible = true;

    [Tooltip("Player Camera")]
    public Transform playerCamera;
    [SerializeField] private float spawnDistance = 2;
    private bool isFixed = true;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // Start is called before the first frame update
    private void Start()
    {
        // Obtener o añadir el CanvasGroup al Canvas principal
        canvasGroup = canvasMenu.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = canvasMenu.AddComponent<CanvasGroup>();
        }

        // Asegurarse de que el menu empieza siendo visible
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Save initial position and rotation of the canvas menu
        initialPosition = canvasMenu.transform.position;
        initialRotation = canvasMenu.transform.rotation;
    }

    // Update is called once per frame
    private void Update()
    {
        if (leftMenuButton.action.WasPressedThisFrame())
        {
            isVisible = !isVisible;

            // Mostrar u ocultar todo el menu
            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }
        if (rightMenuButton.action.WasPressedThisFrame())
        {
            isFixed = !isFixed;

            if (isFixed)
            {
                canvasMenu.transform.position = initialPosition;
                canvasMenu.transform.rotation = initialRotation;
            }
            else
            {
                canvasMenu.transform.position = playerCamera.position + new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z).normalized * spawnDistance; // Aparece a la altura del jugador
            }
        }

        // Si el menu esta desanclado, el menu mirara constantemente al jugador
        if (!isFixed)
        {
            canvasMenu.transform.LookAt(new Vector3(playerCamera.position.x, canvasMenu.transform.position.y, playerCamera.position.z)); // Gira en el eje Y del canvas
            canvasMenu.transform.forward *= -1; // Se invierte para que el menu mire al jugador
        }
    }
}
