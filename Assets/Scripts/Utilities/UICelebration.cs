using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the UI celebration effects, including particle spawning and star animation.
/// </summary>
public class UICelebration : MonoBehaviour
{
    public GameObject particlePrefab;
    public int numberOfParticles = 30;
    public Transform starTransform;
    public GameObject overlay;
    public float spawnRadius = 100f;
    public float particleSpeed = 50f;
    public float starGrowDuration = 2.0f;  // Duration for the star to reach its full size
    public float starRotationSpeed = 50f;  // Rotation speed of the star

    /// <summary>
    /// Starts the particle spawning and star animation coroutines.
    /// </summary>
    void Start()
    {
        // Start the particle spawning and star animation coroutines
        StartCoroutine(SpawnParticles());
        StartCoroutine(AnimateStar());
    }

    /// <summary>
    /// Spawns particles around the star with varying transparency and moves them outward.
    /// </summary>
    private IEnumerator SpawnParticles()
    {
        for (int i = 0; i < numberOfParticles; i++)
        {
            GameObject particle = Instantiate(particlePrefab, transform);
            particle.transform.position = starTransform.position + (Vector3)Random.insideUnitCircle * spawnRadius;

            // Move the particle just in front of the Overlay but behind everything else
            particle.transform.SetSiblingIndex(overlay.transform.GetSiblingIndex() + 1);

            Image particleImage = particle.GetComponent<Image>();
            Color newColor = particleImage.color;
            newColor.a = Random.Range(0.5f, 1f); // Adjust transparency range as needed
            particleImage.color = newColor;

            Vector3 direction = (particle.transform.position - starTransform.position).normalized;
            StartCoroutine(MoveAndRotateParticle(particle, direction));

            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// Moves and rotates a single particle in a given direction.
    /// </summary>
    /// <param name="particle">The particle GameObject to animate.</param>
    /// <param name="direction">The direction to move the particle.</param>
    private IEnumerator MoveAndRotateParticle(GameObject particle, Vector3 direction)
    {
        while (particle != null)
        {
            particle.transform.position += direction * particleSpeed * Time.deltaTime;
            particle.transform.Rotate(0, 0, particleSpeed * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// Animates the star, causing it to grow to its full size and rotate.
    /// </summary>
    private IEnumerator AnimateStar()
    {
        float time = 0;
        Vector3 originalScale = starTransform.localScale;
        starTransform.localScale = Vector3.zero; // Start from zero scale

        while (time < starGrowDuration)
        {
            starTransform.localScale = Vector3.Lerp(Vector3.zero, originalScale, time / starGrowDuration);
            starTransform.Rotate(0, 0, starRotationSpeed * Time.deltaTime);  // Rotate the star

            time += Time.deltaTime;
            yield return null;
        }

        // Keep rotating the star after it has grown to full size
        while (true)
        {
            starTransform.Rotate(0, 0, starRotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
