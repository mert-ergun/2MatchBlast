[System.Serializable]
/// <summary>
/// Represents the data needed to save or load a game's state.
/// </summary>
public class GameSaveData
{
    /// <summary>
    /// The current level number in the game.
    /// </summary>
    public int level_number;

    /// <summary>
    /// The width of the grid for the current level.
    /// </summary>
    public int grid_width;

    /// <summary>
    /// The height of the grid for the current level.
    /// </summary>
    public int grid_height;

    /// <summary>
    /// The current count of moves left or used, depending on the game's mechanics.
    /// </summary>
    public int move_count;

    /// <summary>
    /// An array representing the current state of the grid, typically storing information about each cell/block.
    /// </summary>
    public string[] grid;
}
