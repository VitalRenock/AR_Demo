using TMPro;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    public TextMeshProUGUI ResultTextMesh;

    public string WinText;
    public string LoseText;

    public AudioSource MusicMenu;
    public AudioSource MusicCelebration;

    void Start()
    {
        MusicMenu.Stop();
        MusicCelebration.Play();

        // On récupère la condition de victoire
        bool winCondition = PlayerPrefs.GetInt("IsWin", 0) == 1;

        if (winCondition)
        {
            ResultTextMesh.text = WinText;
            ResultTextMesh.color = Color.green;
        }
        else
        {
            ResultTextMesh.text = LoseText;
            ResultTextMesh.color = Color.red;
        }

    }

}
