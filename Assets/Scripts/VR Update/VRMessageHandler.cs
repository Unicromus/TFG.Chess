using System.Collections;
using TMPro;
using UnityEngine;

public class VRMessageHandler : MonoBehaviour
{
    public TextMeshPro messageText;  // Asigna el TextMeshPro en el Inspector - El mensaje que se mostrara
    private Coroutine hideCoroutine;
    private Transform cameraTransform;

    void Start()
    {
        messageText.gameObject.SetActive(false);
        cameraTransform = Camera.main.transform; // Obtiene la cámara del jugador
    }

    void LateUpdate()
    {
        if (messageText.gameObject.activeSelf)
        {
            // Hace que el texto siempre mire hacia la cámara
            transform.LookAt(cameraTransform);
            transform.Rotate(0, 180, 0); // Voltea el texto para que no salga al revés
        }
    }

    public void ShowMessage(string message, float duration = 4.0f)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        // Si ya hay una corutina en marcha, la cancelamos
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(HideMessageAfterDelay(duration));
    }

    IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.gameObject.SetActive(false);
    }
}
