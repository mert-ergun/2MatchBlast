using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
/// <summary>
/// Manages the UI display for a specific level goal, showing the obstacle to collect and the count remaining.
/// </summary>
public class GoalDisplay : MonoBehaviour
{
    /// <summary>
    /// Image component displaying the obstacle associated with the goal.
    /// </summary>
    public Image obstacleImage;

    /// <summary>
    /// Text component showing the remaining count for the goal.
    /// </summary>
    public TextMeshProUGUI countText;

    /// <summary>
    /// Image component showing a checkmark when the goal is completed.
    /// </summary>
    public Image goalCheck;

    // Sprites for each obstacle type
    public Sprite Vase;
    public Sprite Box;
    public Sprite Stone;

    /// <summary>
    /// Sets up the goal display based on the provided LevelGoal. LevelGoal defined in LevelInitializer.cs.
    /// </summary>
    /// <param name="goal">The LevelGoal to display.</param>
    public void SetupGoal(LevelGoal goal)
    {
        obstacleImage.sprite = GetSpriteForObstacle(goal.type);
        countText.text = goal.count.ToString();
        goalCheck.enabled = false;
    }

    /// <summary>
    /// Retrieves the sprite associated with a specific obstacle type.
    /// </summary>
    /// <param name="obstacleType">The type of obstacle.</param>
    /// <returns>The sprite corresponding to the obstacle type.</returns>
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

    /// <summary>
    /// Updates the display to reflect the current count of the goal.
    /// </summary>
    /// <param name="newCount">The updated count for the goal.</param>
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
