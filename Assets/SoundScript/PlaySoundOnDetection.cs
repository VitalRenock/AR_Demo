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
        // Log pour v�rifier que le script s'initialise bien
        Debug.Log("Script initialis� et pr�t � d�tecter l'image.");

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
            Debug.Log("?? Image d�tect�e, on joue le son !");
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            Debug.Log("? Image perdue, on arr�te le son.");
            audioSource.Stop();
        }
    }
}
