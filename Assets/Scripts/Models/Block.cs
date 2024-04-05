using UnityEngine;

/// <summary>
/// Base class for blocks in the game, defining common properties and behaviors.
/// </summary>
public class Block : MonoBehaviour
{
    /// <summary>
    /// Enum representing the types of blocks.
    /// </summary>
    public enum BlockType
    {
        Cube,
        Obstacle,
        TNT
    }

    public BlockType type;
    private int x;
    private int y;
    public bool isExploded = false;

    /// <summary>
    /// Prefab for the particle effect when the block is activated or interacts in the game.
    /// </summary>
    [SerializeField]
    public GameObject particlePrefab;

    /// <summary>
    /// Activates the block when it is clicked.
    /// </summary>
    protected virtual void OnMouseDown()
    {
        ActivateBlock();
    }

    /// <summary>
    /// Contains common activation logic for a block, which can be overridden in derived classes.
    /// </summary>
    public virtual void ActivateBlock()
    { 
        
    }

    /// <summary>
    /// Deactivates the block, making it inactive in the scene.
    /// </summary>
    public virtual void DeactivateBlock()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the block type. This method can be overridden in derived classes to provide custom behavior.
    /// </summary>
    /// <param name="blockType">The type of block to set.</param>
    public virtual void SetType(string blockType)
    {

    }

    /// <summary>
    /// Initiates the block's falling behavior. Starts a coroutine to animate the block falling.
    /// </summary>
    /// <param name="fallDistance">The distance the block should fall.</param>
    public void Fall(int fallDistance)
    {
        StartCoroutine(FallCoroutine(fallDistance));
    }

    /// <summary>
    /// Coroutine to handle the block falling with animation.
    /// </summary>
    /// <param name="fallDistance">The distance the block should fall.</param>
    public System.Collections.IEnumerator FallCoroutine(int fallDistance)
    {
        float fallSpeed = 4.0f; // Units per second
        float distance = Mathf.Abs((1.42f * 0.33f) * fallDistance); // Calculate the absolute fall distance
        float duration = distance / fallSpeed; // Total duration based on speed and distance

        float bounceHeight = 0.05f; // Height of the bounce above the end position

        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, transform.position.y - distance, transform.position.z);
        Vector3 bouncePos = new Vector3(endPos.x, endPos.y + bounceHeight, endPos.z); // Position slightly above the end position for bounce

        // Move to end position
        float time = 0;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos; // Ensure the block is exactly at the end position

        float bounceDuration = 0.2f; // Fixed duration for the bounce effect

        // Bounce up to bounce position
        time = 0;
        while (time < bounceDuration)
        {
            transform.position = Vector3.Lerp(endPos, bouncePos, time / bounceDuration);
            time += Time.deltaTime;
            yield return null;
        }

        // Settle back to end position
        time = 0;
        while (time < bounceDuration)
        {
            transform.position = Vector3.Lerp(bouncePos, endPos, time / bounceDuration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos; // Ensure the block ends up exactly at the end position
    }

    /// <summary>
    /// Triggers the explosion behavior of the block, with visual and physical effects. TNT blocks override this method.
    /// </summary>
    public virtual void Explode()
    {
        // Instantiate some particles
        int numParticles = 3; // Number of particles to instantiate
        for (int i = 0; i < numParticles; i++)
        {
            GameObject particle = ObjectPool.Instance.SpawnFromPool("Particle", transform.position, Quaternion.identity);
            // Rotate the particle to a random angle
            particle.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));

            // Scale the particle to a random size
            float scale = Random.Range(0.05f, 0.15f);
            particle.transform.localScale = new Vector3(scale, scale, 0);

            particle.GetComponent<SpriteRenderer>().sortingOrder = 101; // Ensure particles are rendered above everything else
            if (type == BlockType.Cube) // Set the particle color based on the block color
            {
                Cube cube = (Cube)this;
                particle.GetComponent<Particle>().SetColor(cube.color.ToString());
            }
            else if (type == BlockType.Obstacle)
            {
                Obstacle obstacle = (Obstacle)this;
                particle.GetComponent<Particle>().SetColor(obstacle.obstacleType.ToString());
            }
            else if (type == BlockType.TNT)
            {
                particle.GetComponent<Particle>().SetColor("TNT");
            }

            // Apply a random force to each particle
            Rigidbody2D rb = particle.GetComponent<Rigidbody2D>();
            Vector2 force = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(1f, 5f);
            rb.AddForce(force, ForceMode2D.Impulse);
            particle.GetComponent<Particle>().StartCoroutine(particle.GetComponent<Particle>().ReturnToPool());
        }
        GridManager.Instance.SetBlock(x, y, null);
    }

    public void SetX(int x)
    {
        this.x = x;
    }

    public void SetY(int y)
    {
        this.y = y;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }
}
