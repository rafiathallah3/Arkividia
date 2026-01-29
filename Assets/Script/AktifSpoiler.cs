using UnityEngine;

public class AktifSpoiler : MonoBehaviour
{
    public string textSpoiler;
    public bool apakahSudahAktif = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !apakahSudahAktif)
        {
            apakahSudahAktif = true;
            DialogueManager.instance.ShowDialogue(textSpoiler, true);
        }
    }
}
