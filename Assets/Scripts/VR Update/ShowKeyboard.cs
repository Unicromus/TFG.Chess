using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.UI; // To access the kayboard parameters

public class ShowKeyboard : MonoBehaviour
{
    private TMP_InputField inputField;

    [Tooltip("Z")]
    public float distance = 0.5f;
    [Tooltip("Y")]
    public float verticalOffset = -0.5f;

    [Tooltip("Initial position, can be the camera (close position) or other position like the Input Field (distant position)")]
    public Transform positionSource;

    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onSelect.AddListener(x => OpenKeyboard());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenKeyboard()
    {
        NonNativeKeyboard.Instance.InputField = inputField; // Assign our input field to the keyboard. We connect them to be the same and work together
        NonNativeKeyboard.Instance.PresentKeyboard(inputField.text); // Show the scene's singleton Keyboard

        Vector3 direction = positionSource.forward;
        direction.y = 0; // We want the direction to be always horizontal
        direction.Normalize();

        Vector3 targetPosition = positionSource.position + (direction * distance) + (Vector3.up * verticalOffset); // initial position + Z + Y

        NonNativeKeyboard.Instance.RepositionKeyboard(targetPosition); // Reposition the scene's singleton Keyboard

        SetCaretColorAlpha(1); // Show the caret

        NonNativeKeyboard.Instance.OnClosed += Instance_OnClosed; // Create a fuction that will be called when the keyboard is closing
    }

    private void Instance_OnClosed(object sender, System.EventArgs e)
    {
        SetCaretColorAlpha(0); // Hide the caret
        NonNativeKeyboard.Instance.OnClosed -= Instance_OnClosed; // Remove the instance OnClosed function we called
    }

    public void SetCaretColorAlpha(float value)
    {
        inputField.customCaretColor = true;
        Color caretColor = inputField.caretColor;
        caretColor.a = value;
        inputField.caretColor = caretColor;
    }
}
