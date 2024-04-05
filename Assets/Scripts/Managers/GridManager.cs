using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The GridManager class is responsible for managing the grid of blocks in the game.
/// </summary>
public class GridManager : Singleton<GridManager>
{
    /// <summary>
    /// The grid is a 2D array of blocks that represents the game board.
    /// </summary>
    private Block[,] grid;

    /// <summary>
    /// The gridPos array is a 2D array of floats that represents the x position of each block in the grid. 
    /// This is used to determine the position of the newly spawned blocks.
    /// </summary>
    public float[,] gridPos;

    private int column;
    private int row;

    /// <summary>
    /// The topY value is the y position of the top row of the grid.
    /// This is used to determine the position of the newly spawned blocks.
    /// </summary>
    public float topY;

    /// <summary>
    /// The checkedTNTs hashset is used to keep track of the TNT blocks that have already been checked for explosion.
    /// This is used to prevent infinite recursion when checking for connected TNT blocks.
    /// </summary>
    private HashSet<Block> checkedTNTs = new HashSet<Block>();

    /// <summary>
    /// The InitializeGrid method initializes the grid with the given blocks.
    /// </summary>
    public void InitializeGrid(Block[][] blocks)
    {
        row = blocks.Length;
        column = blocks[0].Length;
        grid = new Block[row, column];
        gridPos = new float[row, column];

        // Initialize the grid
        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < column; y++)
            {
                if (blocks[x][y] == null)
                {
                    grid[x, y] = null;
                    continue;
                }

                grid[x, y] = blocks[x][y];
                grid[x, y].SetX(x);
                grid[x, y].SetY(y);
                gridPos[x, y] = grid[x, y].transform.position.x;
            }
        }

        // Set the topY value
        topY = grid[0, 0].transform.position.y;

        // If the grid position is saved, set the grid position to the saved position, this will prevent null blocks to not having a position
        if (LevelSaver.Instance.CheckForGridPos() && LevelSaver.Instance.GetGridPosLevel() == LevelInitializer.Instance.levelData.level_number)
        {
            gridPos = LevelSaver.Instance.GetGridPos();
        }

        // Check for any TNT-able blocks in the grid
        CheckForTNT();
    }

    /// <summary>
    /// The HandleBlockTap method is called when a block is tapped.
    /// It finds the connected blocks and obstacles and destroys them.
    /// </summary>
    /// <param name="block">The block that was tapped.</param>
    public IEnumerator HandleBlockTap(Block block)
    {
        if (block.type != Block.BlockType.Cube)
        {
            yield break;
        }
        List<Block> connectedBlocks = FindConnectedBlocks(block);

        // If there is only one block, play the jiggle animation
        if (connectedBlocks.Count == 1)
        {
            // Play jiggle animation
            block.GetComponent<Animator>().SetTrigger("Jiggle");
        }
        // If there are two or more blocks, BLAST!
        else if (connectedBlocks.Count >= 2)
        {
            block.GetComponent<Animator>().SetTrigger("Clicked");
            yield return new WaitForSeconds(0.1f);

            // Start the falling state, prevent the player from making any moves
            GameManager.Instance.StartFalling();
            GameManager.Instance.UseMove();
            // Reverse the order of the blocks to destroy the bottom blocks first
            connectedBlocks.Reverse();
            List<Block> allObstacles = new List<Block>();
            foreach (Block connectedBlock in connectedBlocks)
            {
                // Play the destroy animation
                connectedBlock.Explode();

                List<Block> obstacles = FindConnectedObstacles(connectedBlock);
                allObstacles.AddRange(obstacles);
                foreach (Block obstacle in obstacles)
                {
                    Obstacle obstacle1 = (Obstacle)obstacle;
                    if (obstacle1.obstacleType == Obstacle.ObstacleType.Stone)
                    {
                        allObstacles.Remove(obstacle);
                        continue;
                    }
                    if (obstacle1.isExploded)
                    {
                        allObstacles.Remove(obstacle);
                        continue;
                    }
                    obstacle1.Explode();
                    obstacle1.isExploded = true;
                }

                ObjectPool.Instance.ReturnToPool(connectedBlock.type.ToString(), connectedBlock.gameObject);
                GameManager.Instance.UpdateGoals();
            }

            ClearIsExploded();

            if (connectedBlocks.Count >= 5)
            {
                // Spawn a TNT block at the position of the tapped block
                Block tntBlock = ObjectPool.Instance.SpawnFromPool("TNT", block.transform.position, Quaternion.identity).GetComponent<Block>();
                tntBlock.SetType("TNT");
                SetBlock(block.GetX(), block.GetY(), tntBlock);
                tntBlock.SetX(block.GetX());
                tntBlock.SetY(block.GetY());
                tntBlock.GetComponent<SpriteRenderer>().sortingOrder = (row - tntBlock.GetX() - 1) * column + tntBlock.GetY();
            }

            GameManager.Instance.FallBlock();
            yield return new WaitForSeconds(0.2f);

            List<int[]> blocksToSpawn = FindBlocksToSpawn();
            // Group them by their row to handle simultaneous fall per row
            var groupedBlocks = blocksToSpawn.GroupBy(b => b[0]).OrderBy(g => g.Key);
            foreach (var group in groupedBlocks)
            {
                foreach (int[] blockToSpawn in group)
                {
                    // Spawn a new block at the top position of the grid, make it look like it's coming from the top
                    Block newBlock = ObjectPool.Instance.SpawnFromPool("Cube", new Vector3(gridPos[blockToSpawn[0], blockToSpawn[1]], topY + 1.42f * 0.33f), Quaternion.identity).GetComponent<Block>();
                    SetBlock(0, blockToSpawn[1], newBlock);
                    newBlock.SetX(0);
                    newBlock.SetY(blockToSpawn[1]);
                    newBlock.GetComponent<SpriteRenderer>().sortingOrder = (row - newBlock.GetX() - 1) * column + newBlock.GetY();
                    Cube cube = (Cube)newBlock;
                    cube.SetType("rand");

                    if (checkBlockCanFall(newBlock))
                    {
                        FallBlock(newBlock);
                    }
                    else
                    {
                        newBlock.Fall(1);
                    }
                }
                yield return new WaitForSeconds(0.15f); // Wait for the current row to finish before proceeding to the next
            }

            CheckForTNT();
            yield return new WaitForSeconds(0.01f * blocksToSpawn.Count);
            GameManager.Instance.StopFalling();
            LevelSaver.Instance.SaveCurrentLevel();
        }
    }

    /// <summary>
    /// The CheckForCombo method is called when a TNT block is tapped. It checks if the TNT block is near another TNT block or not.
    /// If it is, it triggers the explosion of the TNT blocks with a larger area.
    /// </summary>
    /// <param name="tnt">The TNT block that was tapped.</param>
    public void CheckForCombo(TNT tnt)
    {
        // Check if tnt is near another tnt
        for (int x = tnt.GetX() - 1; x <= tnt.GetX() + 1; x++)
        {
            for (int y = tnt.GetY() - 1; y <= tnt.GetY() + 1; y++)
            {
                if (x >= 0 && x < row && y >= 0 && y < column)
                {
                    if (grid[x, y] != null && grid[x, y].type == Block.BlockType.TNT && grid[x, y] != tnt)
                    {
                        StartCoroutine(ExplodeTNT((TNT)grid[x, y], 3));
                        return;
                    }
                }
            }
        }

        StartCoroutine(ExplodeTNT(tnt, 2));
    }

    /// <summary>
    /// The ExplodeTNT method is called when a TNT block is tapped. It triggers the explosion of the TNT block and the blocks around it.
    /// It starts the explosion coroutine and stops the falling state.
    /// </summary>
    public IEnumerator ExplodeTNT(TNT tnt, int area)
    {
        // Trigger the explosion of the initial TNT block.
        yield return StartCoroutine(TriggerExplosion(tnt, area));
        GameManager.Instance.StopFalling();
        LevelSaver.Instance.SaveCurrentLevel();
    }

    /// <summary>
    /// This coroutine triggers the explosion of the TNT block and the blocks around it.
    /// Any TNT blocks that are in range of the initial TNT block are also exploded.
    /// </summary>
    /// <param name="tnt">The initial TNT block that was tapped.</param>
    /// <param name="area">The area of the explosion.</param>
    public IEnumerator TriggerExplosion(TNT tnt, int area)
    {
        if (tnt == null || tnt.isExploded)
        {
            yield break;
        }

        checkedTNTs.Clear();
        checkedTNTs.Add(tnt);

        HashSet<Block> blocksToExplode = FindTNTNeighbors(tnt, area);

        blocksToExplode.Remove(tnt);
        tnt.Explode();
        SetBlock(tnt.GetX(), tnt.GetY(), null);

        GameManager.Instance.StartFalling();

        foreach (Block block in blocksToExplode)
        {
            if (block == null)
            {
                continue;
            }

            if (block.type == Block.BlockType.Obstacle)
            {
                Obstacle obstacle = (Obstacle)block;
                obstacle.Explode();
                obstacle.isExploded = true;
            }
            else if (block.type == Block.BlockType.TNT)
            {
                block.Explode();
                SetBlock(block.GetX(), block.GetY(), null);
            }
            else
            {
                block.Explode();
                block.isExploded = true;
                ObjectPool.Instance.ReturnToPool(block.type.ToString(), block.gameObject);
            }
            GameManager.Instance.UpdateGoals();
        }

        GameManager.Instance.FallBlock();

        yield return new WaitForSeconds(0.2f);

        List<int[]> blocksToSpawn = FindBlocksToSpawn();
        // Group them by their row to handle simultaneous fall per row
        var groupedBlocks = blocksToSpawn.GroupBy(b => b[0]).OrderBy(g => g.Key);
        foreach (var group in groupedBlocks)
        {
            foreach (int[] blockToSpawn in group)
            {
                Block newBlock = ObjectPool.Instance.SpawnFromPool("Cube", new Vector3(gridPos[blockToSpawn[0], blockToSpawn[1]], topY + 1.42f * 0.33f), Quaternion.identity).GetComponent<Block>();
                SetBlock(0, blockToSpawn[1], newBlock);
                newBlock.SetX(0);
                newBlock.SetY(blockToSpawn[1]);
                newBlock.GetComponent<SpriteRenderer>().sortingOrder = (row - newBlock.GetX() - 1) * column + newBlock.GetY();
                Cube cube = (Cube)newBlock;
                cube.SetType("rand");

                if (checkBlockCanFall(newBlock))
                {
                    FallBlock(newBlock);
                }
                else
                {
                    newBlock.Fall(1);
                }
            }
            yield return new WaitForSeconds(0.15f); // Wait for the current row to finish before proceeding to the next
        }

        CheckForTNT();
        ClearIsExploded();
        yield return new WaitForSeconds(0.01f * blocksToSpawn.Count);
        GameManager.Instance.StopFalling();
    }

    /// <summary>
    /// The FindTNTNeighbors method finds all the blocks that are in range of the TNT block.
    /// It functions recursively to find all the connected blocks.
    /// If a TNT block is found, it is added to the checkedTNTs hashset and the FindTNTNeighbors method is called again.
    /// </summary>
    /// <param name="tnt">The TNT block that was tapped.</param>
    /// <param name="area">Area of the explosion.</param>
    /// <returns></returns>
    private HashSet<Block> FindTNTNeighbors(TNT tnt, int area)
    {
        // Use a hashset to store the neighbors to prevent duplicates
        HashSet<Block> neighbors = new HashSet<Block>();

        for (int x = tnt.GetX() - area; x <= tnt.GetX() + area; x++)
        {
            for (int y = tnt.GetY() - area; y <= tnt.GetY() + area; y++)
            {
                if (x >= 0 && x < row && y >= 0 && y < column)
                {
                    if (grid[x, y] != null)
                    {
                        neighbors.Add(grid[x, y]);
                        if (grid[x, y].type == Block.BlockType.TNT)
                        {
                            TNT tntNeighbor = (TNT)grid[x, y];
                            if (!checkedTNTs.Contains(tntNeighbor))
                            {
                                checkedTNTs.Add(tntNeighbor);
                                // Recursively find the neighbors of the TNT block and add them to the set
                                HashSet<Block> newNeighbors = FindTNTNeighbors(tntNeighbor, 2);
                                foreach (Block neighbor in newNeighbors)
                                {
                                    neighbors.Add(neighbor);
                                }
                            }
                        }
                    }
                }
            }
        }
        return neighbors;
    }

    /// <summary>
    /// The ClearIsExploded method resets the isExploded value of all the obstacles in the grid.
    /// </summary>
    private void ClearIsExploded()
    {
        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < column; y++)
            {
                if (grid[x, y] != null && grid[x, y].type == Block.BlockType.Obstacle)
                {
                    Obstacle obstacle = (Obstacle)grid[x, y];
                    obstacle.isExploded = false;
                }
            }
        }
    }

    /// <summary>
    /// The FindBlocksToSpawn method finds the blocks that need to be spawned in the grid.
    /// </summary>
    /// <returns>A list of int arrays containing the x and y positions of the blocks that need to be spawned.</returns>
    private List<int[]> FindBlocksToSpawn()
    {
        // Iterate over each element in the grid, if there is a null element, check it's above elements until a non-null element is found, if there is one, don't add those null elements to the list
        List<int[]> blocks = new List<int[]>();

        for (int y = 0; y < column; y++)
        {
            for (int x = 0; x < row; x++)
            {
                if (grid[x, y] == null)
                {
                    int[] block = new int[] { x, y };
                    blocks.Add(block);
                } else
                {
                    break;
                }
            }
        }

        return blocks;
    }

    /// <summary>
    /// The checkBlockCanFall method checks if the block can fall or not.
    /// </summary>
    /// <param name="block">The block to check.</param>
    /// <returns>True if a null block is below the block to check.</returns>
    private bool checkBlockCanFall(Block block)
    {
        if (block.GetX() < row - 1)
        {
            if (grid[block.GetX() + 1, block.GetY()] == null)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Makes the block fall down to the lowest possible position.
    /// </summary>
    /// <param name="block">The block to fall.</param>
    private void FallBlock(Block block)
    {
        int fallDistance = 0;
        for (int i = block.GetX() + 1; i < row; i++)
        {
            if (grid[i, block.GetY()] == null)
            {
                fallDistance++;
            }
            else
            {
                break;
            }
        }

        if (fallDistance > 0)
        {
            block.Fall(fallDistance + 1);

            grid[block.GetX(), block.GetY()] = null;
            grid[block.GetX() + fallDistance, block.GetY()] = block;
            block.SetX(block.GetX() + fallDistance);
            // Set the sorting order
            block.GetComponent<SpriteRenderer>().sortingOrder -= fallDistance * column;
        }
    }

    /// <summary>
    /// Finds the connected obstacles to the given block.
    /// </summary>
    /// <param name="block">The block to find the connected obstacles.</param>
    /// <returns>A list of connected obstacles.</returns>
    private List<Block> FindConnectedObstacles(Block block)
    {
        List<Block> neighbors = GetNeighbors(block);

        List<Block> connectedObstacles = new List<Block>();

        foreach (Block neighbor in neighbors)
        {
            if (neighbor != null)
            {
                if (neighbor.type == Block.BlockType.Obstacle)
                {
                    connectedObstacles.Add(neighbor);
                }
            }
        }

        return connectedObstacles;
    }

    /// <summary>
    /// Finds the connected blocks to the given block.
    /// It uses a breadth-first search algorithm to find the connected blocks.
    /// </summary>
    /// <param name="block">The block to find the connected blocks.</param>
    /// <returns>A list of connected blocks.</returns>
    private List<Block> FindConnectedBlocks(Block block)
    {
        List<Block> connectedBlocks = new List<Block>();
        Queue<Block> blocksToCheck = new Queue<Block>();
        HashSet<Block> visitedBlocks = new HashSet<Block>();

        blocksToCheck.Enqueue(block);
        visitedBlocks.Add(block);

        while (blocksToCheck.Count > 0)
        {
            Block currentBlock = blocksToCheck.Dequeue();
            if (currentBlock == null)
            {
                continue;
            }
            if (currentBlock.type != Block.BlockType.Cube)
            {
                continue;
            }
            Cube cube = (Cube)currentBlock;

            connectedBlocks.Add(currentBlock);

            List<Block> neighbors = GetNeighbors(currentBlock);
            foreach (Block neighbor in neighbors)
            {
                if (neighbor == null)
                {
                    continue;
                }
                if (neighbor.type != Block.BlockType.Cube)
                {
                    continue;
                }
                
                Cube neighborCube = (Cube)neighbor;

                if (!visitedBlocks.Contains(neighbor) && cube.color == neighborCube.color)
                {
                    blocksToCheck.Enqueue(neighbor);
                    visitedBlocks.Add(neighbor);
                }
            }
        }

        return connectedBlocks;
    }

    /// <summary>
    /// Finds the neighbors of the given block. Checks all four directions. Excludes the diagonal neighbors.
    /// </summary>
    /// <param name="block">The block to find the neighbors.</param>
    /// <returns>A list of neighbors.</returns>
    private List<Block> GetNeighbors(Block block)
    {
        List<Block> neighbors = new List<Block>();

        int x = block.GetX();
        int y = block.GetY();

        if (x > 0)
        {
            neighbors.Add(grid[x - 1, y]);
        }
        if (x < row - 1)
        {
            neighbors.Add(grid[x + 1, y]);
        }
        if (y > 0)
        {
            neighbors.Add(grid[x, y - 1]);
        }
        if (y < column - 1)
        {
            neighbors.Add(grid[x, y + 1]);
        }

        return neighbors;
    }

    /// <summary>
    /// Checks if there are any blocks that can fall. Excludes the top row.
    /// </summary>
    /// <returns>True if there are any blocks that can fall.</returns>
    public bool CheckForFallingBlocks()
    {
        for (int y = 0; y < column; y++)
        {
            for (int x = 1; x < row; x++)
            {
                if (grid[x, y] == null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the whole grid for TNT-able blocks.
    /// </summary>
    public void CheckForTNT()
    {
        // Check the whole grid, if there are 5 blocks of the same color in a sequence, call their setTNT method
        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < column; y++)
            {
                if (grid[x, y] == null || grid[x, y].type != Block.BlockType.Cube)
                {
                    continue;
                }
                Block block = grid[x, y];
                List<Block> connectedBlocks = FindConnectedBlocks(block);
                if (connectedBlocks.Count >= 5)
                {
                    for (int i = 0; i < connectedBlocks.Count; i++)
                    {
                        Cube cube = (Cube)connectedBlocks[i];
                        cube.SetTNT();
                    }
                } else
                {
                    for (int i = 0; i < connectedBlocks.Count; i++)
                    {
                        Cube cube = (Cube)connectedBlocks[i];
                        cube.SetNormal();
                    }
                }
                
            }
        }
    }

    /// <summary>
    /// The GetBlock method returns the block at the given x and y position.
    /// </summary>
    /// <param name="x">Row position of the block.</param>
    /// <param name="y">Column position of the block.</param>
    /// <returns>The block at the given position.</returns>
    public Block GetBlock(int x, int y)
    {
        return grid[x, y];
    }

    /// <summary>
    /// Sets the block at the given x and y position.
    /// </summary>
    /// <param name="x">Row position of the block.</param>
    /// <param name="y">Column position of the block.</param>
    /// <param name="block">The block to set.</param>
    public void SetBlock(int x, int y, Block block)
    {
        grid[x, y] = block;
    }

    public int GetColumn()
    {
        return column;
    }

    public int GetRow()
    {
        return row;
    }





}
