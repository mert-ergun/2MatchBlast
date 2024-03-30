using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GoalDisplay : MonoBehaviour
{
    public Image obstacleImage; 
    public TextMeshProUGUI countText;

    public Sprite Vase;
    public Sprite Box;
    public Sprite Stone;

    public void SetupGoal(LevelGoal goal)
    {
        obstacleImage.sprite = GetSpriteForObstacle(goal.type);
        countText.text = goal.count.ToString();
    }

    private Sprite GetSpriteForObstacle(string obstacleType)
    {
        switch (obstacleType)
        {
            case "v":
                return Vase;
            case "bo":
                return Box;
            case "s":
                return Stone;
            default:
                return null;
        }
    }
}
