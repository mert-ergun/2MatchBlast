using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the behavior and properties of a particle object in the game.
/// </summary>
public class Particle : MonoBehaviour
{
    /// <summary>
    /// The lifetime duration of the particle before it is returned to the object pool.
    /// </summary>
    public float lifetime = 2.0f;

    // Sprites for different cube particles
    public Sprite red;
    public Sprite green;
    public Sprite blue;
    public Sprite yellow;

    // Sprites for different obstacle particles
    public Sprite vase1;
    public Sprite vase2;
    public Sprite vase3;
    public Sprite box1;
    public Sprite box2;
    public Sprite box3;
    public Sprite stone1;
    public Sprite stone2;
    public Sprite stone3;

    /// <summary>
    /// Returns the particle to the object pool after its lifetime expires.
    /// </summary>
    public IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(lifetime);
        ObjectPool.Instance.ReturnToPool("Particle", gameObject);
    }

    /// <summary>
    /// Sets the particle's sprite based on the specified color or material type.
    /// </summary>
    /// <param name="color">The color or material type of the particle.</param>
    public void SetColor(string color)
    {
        switch (color)
        {
            case "Red":
                GetComponent<SpriteRenderer>().sprite = red;
                break;
            case "Green":
                GetComponent<SpriteRenderer>().sprite = green;
                break;
            case "Blue":
                GetComponent<SpriteRenderer>().sprite = blue;
                break;
            case "Yellow":
                GetComponent<SpriteRenderer>().sprite = yellow;
                break;
            case "Box":
                // Select a random box sprite
                int boxSprite = Random.Range(1, 4);
                switch (boxSprite)
                {
                    case 1:
                        GetComponent<SpriteRenderer>().sprite = box1;
                        break;
                    case 2:
                        GetComponent<SpriteRenderer>().sprite = box2;
                        break;
                    case 3:
                        GetComponent<SpriteRenderer>().sprite = box3;
                        break;
                }
                break;
            case "Vase":
                // Select a random vase sprite
                int vaseSprite = Random.Range(1, 4);
                switch (vaseSprite)
                {
                    case 1:
                        GetComponent<SpriteRenderer>().sprite = vase1;
                        break;
                    case 2:
                        GetComponent<SpriteRenderer>().sprite = vase2;
                        break;
                    case 3:
                        GetComponent<SpriteRenderer>().sprite = vase3;
                        break;
                }
                break;
            case "Stone":
                // Select a random stone sprite
                int stoneSprite = Random.Range(1, 4);
                switch (stoneSprite)
                {
                    case 1:
                        GetComponent<SpriteRenderer>().sprite = stone1;
                        break;
                    case 2:
                        GetComponent<SpriteRenderer>().sprite = stone2;
                        break;
                    case 3:
                        GetComponent<SpriteRenderer>().sprite = stone3;
                        break;
                }
                break;
        }
    }

}
