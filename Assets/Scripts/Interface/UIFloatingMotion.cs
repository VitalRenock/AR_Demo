using UnityEngine;

public class UIFloatingMotion : MonoBehaviour
{
    [Header("Paramètres de mouvement horizontal")]
    /// <summary>
    /// Activer/Désactiver le mouvement horizontal
    /// </summary>
    public bool moveHorizontally = true;
    /// <summary>
    /// Distance du mouvement horizontal
    /// </summary>
    public float horizontalAmplitude = 10f;
    /// <summary>
    /// Vitesse du mouvement horizontal
    /// </summary>
    public float horizontalSpeed = 2f;

    [Header("Paramètres de mouvement vertical")]
    /// <summary>
    /// Activer/Désactiver le mouvement vertical
    /// </summary>
    public bool moveVertically = true;
    /// <summary>
    /// Distance du mouvement vertical
    /// </summary>
    public float verticalAmplitude = 10f;
    /// <summary>
    /// Vitesse du mouvement vertical
    /// </summary>
    public float verticalSpeed = 2f;

    private Vector3 startPosition;
    private float timeOffset;

    void Start()
    {
        // On stocke la position initiale
        startPosition = transform.localPosition;
        // Décalage aléatoire pour éviter que tous les objets bougent en même temps
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        float newX = startPosition.x;
        float newY = startPosition.y;

        if (moveHorizontally)
            newX += Mathf.Sin(Time.time * horizontalSpeed + timeOffset) * horizontalAmplitude;

        if (moveVertically)
            newY += Mathf.Cos(Time.time * verticalSpeed + timeOffset) * verticalAmplitude;

        transform.localPosition = new Vector3(newX, newY, startPosition.z);
    }
}
