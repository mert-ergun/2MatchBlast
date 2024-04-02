using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public float lifetime = 1.0f;

    public Sprite red;
    public Sprite green;
    public Sprite blue;
    public Sprite yellow;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

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
        }
    }

}
