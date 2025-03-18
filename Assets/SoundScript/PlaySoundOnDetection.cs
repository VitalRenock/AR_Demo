using UnityEngine;
using Vuforia;

public class PlaySoundOnDetection : MonoBehaviour
{
    private ObserverBehaviour observerBehaviour;
    private AudioSource audioSource;

    void Start()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();
        audioSource = GetComponent<AudioSource>();
        // Log pour vérifier que le script s'initialise bien
        Debug.Log("Script initialisé et prêt à détecter l'image.");

        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnDestroy()
    {
        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        if (targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED)
        {
            Debug.Log("?? Image détectée, on joue le son !");
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            Debug.Log("? Image perdue, on arrête le son.");
            audioSource.Stop();
        }
    }
}
