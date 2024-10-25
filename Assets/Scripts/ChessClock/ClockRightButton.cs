using UnityEngine;

public class ClockRightButton : MonoBehaviour
{

    [SerializeField] private ChessClock chessClock;

    private void OnMouseUpAsButton()
    {
        chessClock.WhiteSwap();
    }

}
