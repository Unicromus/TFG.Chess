using UnityEngine;

public class Tables : MonoBehaviour
{

    [SerializeField] private GameObject[] tables;

    private int table = 0;

    [Header("Sounds")]
    [SerializeField] private AudioClip changeTableSoundClip;

    public void ChangeTable()
    {
        table++;
        if (table >= tables.Length)
            table = 0;

        switch(table) {
            case 0:
                tables[0].gameObject.SetActive(true);
                tables[1].gameObject.SetActive(false);
                tables[2].gameObject.SetActive(false);
                break;
            case 1:
                tables[0].gameObject.SetActive(false);
                tables[1].gameObject.SetActive(true);
                tables[2].gameObject.SetActive(false);
                break;
            case 2:
                tables[0].gameObject.SetActive(false);
                tables[1].gameObject.SetActive(false);
                tables[2].gameObject.SetActive(true);
                break;
            default:
                break;
        }

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(changeTableSoundClip, transform, 1f);
    }

}
