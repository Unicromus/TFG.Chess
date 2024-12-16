using UnityEngine;

public class Tables : MonoBehaviour
{

    [SerializeField] private GameObject[] tables;

    private int table = 0;

    [Header("Sounds")]
    [SerializeField] private AudioClip changeTableSoundClip;

    public void Awake()
    {
        for (int i = 0; i < tables.Length; i++)
        {
            if (tables[i].activeSelf)
            {
                table = i;
                return;
            }
        }
    }

    public void ChangeTable()
    {
        tables[table].SetActive(false);
        table++;

        if (table >= tables.Length)
            table = 0;
        tables[table].SetActive(true);

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(changeTableSoundClip, transform, 1f);
    }

}
