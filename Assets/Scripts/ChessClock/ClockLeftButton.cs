using UnityEngine;

public class ClockLeftButton : MonoBehaviour
{

    [SerializeField] private ChessClock chessClock;

    private void OnMouseUpAsButton()
    {
        chessClock.BlackSwap();
    }

}
