using UnityEngine;

public class ClockStartButton : MonoBehaviour
{

    [SerializeField] private ChessClock chessClock;

    private void OnMouseUpAsButton()
    {
        chessClock.StartTimer();
    }

}
