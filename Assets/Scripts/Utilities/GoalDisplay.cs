using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GoalDisplay : MonoBehaviour
{
    public Image obstacleImage; 
    public TextMeshProUGUI countText;
    public Image goalCheck;

    public Sprite Vase;
    public Sprite Box;
    public Sprite Stone;

    public void SetupGoal(LevelGoal goal)
    {
        obstacleImage.sprite = GetSpriteForObstacle(goal.type);
        countText.text = goal.count.ToString();
        goalCheck.enabled = false;
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

    public void UpdateGoalDisplay(int newCount)
    {
        countText.text = newCount.ToString();

        // If the goal is completed, change the count text to goal_check sprite
        if (newCount == 0)
        {
            countText.enabled = false;
            goalCheck.enabled = true;
            
        }
    }

}
