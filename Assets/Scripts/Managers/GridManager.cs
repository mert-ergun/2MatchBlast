using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    private Block[,] grid;
    private float[,] gridPos;

    private int column;
    private int row;

    public float topY;

    private HashSet<Block> checkedTNTs = new HashSet<Block>();

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
                grid[x, y] = blocks[x][y];
                grid[x, y].SetX(x);
                grid[x, y].SetY(y);
                gridPos[x, y] = grid[x, y].transform.position.x;
            }
        }

        // Set the topY value
        topY = grid[0, 0].transform.position.y;

        CheckForTNT();
    }

    public void PrintGrid()
    {
        for (int x = 0; x < row; x++)
        {
            string rowString = "";
            for (int y = 0; y < column; y++)
            {
                rowString += grid[x, y] == null ? "0" : "1";
            }
            Debug.Log(rowString);
        }
    }

    public IEnumerator HandleBlockTap(Block block)
    {
        if (block.type != Block.BlockType.Cube)
        {
            yield break;
        }
        List<Block> connectedBlocks = FindConnectedBlocks(block);

        if (connectedBlocks.Count == 1)
        {
            // Play jiggle animation
            block.GetComponent<Animator>().SetTrigger("Jiggle");
        }
        else if (connectedBlocks.Count >= 2)
        {
            block.GetComponent<Animator>().SetTrigger("Clicked");
            yield return new WaitForSeconds(0.1f);
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
                    if (obstacle1.obstacleType == Obstacle.ObstacleType.Vase && obstacle1.GetComponent<SpriteRenderer>().sprite == obstacle1.vaseSprite1)
                    {
                        allObstacles.Remove(obstacle);
                    }
                    else if (obstacle1.obstacleType == Obstacle.ObstacleType.Stone)
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
                connectedBlocks.Remove(block);
            }

            // Add connected obstacles to the connected blocks list to create new blocks instead of obstacles
            connectedBlocks.AddRange(allObstacles);

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
            /*
            // Group blocks by their row to handle simultaneous fall per row
            var groupedBlocks = connectedBlocks.GroupBy(b => b.GetX()).OrderBy(g => g.Key);
            foreach (var group in groupedBlocks)
            {
                foreach (Block connectedBlock in group)
                {
                    // Spawn a new block at the top for each block in the same row
                    Block newBlock = ObjectPool.Instance.SpawnFromPool("Cube", new Vector3(connectedBlock.GetComponent<Transform>().position.x, topY + 1.42f * 0.33f), Quaternion.identity).GetComponent<Block>();
                    SetBlock(0, connectedBlock.GetY(), newBlock);
                    newBlock.SetX(0);
                    newBlock.SetY(connectedBlock.GetY());
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
            */
            CheckForTNT();
            yield return new WaitForSeconds(0.01f * connectedBlocks.Count);
            GameManager.Instance.StopFalling();
        }
    }

    public void CheckForCombo(TNT tnt)
    {
        // Check if tnt is near another tnt
        for (int x = tnt.GetX() - 1; x <= tnt.GetX() + 1; x++)
        {
            for (int y = tnt.GetY() - 1; y <= tnt.GetY() + 1; y++)
            {
                if (x >= 0 && x < row && y >= 0 && y < column)
                {
                    if (grid[x, y] != null && grid[x, y].type == Block.BlockType.TNT)
                    {
                        StartCoroutine(ExplodeTNT((TNT)grid[x, y], 3));
                        return;
                    }
                }
            }
        }

        StartCoroutine(ExplodeTNT(tnt, 2));
    }

    public IEnumerator ExplodeTNT(TNT tnt, int area)
    {
        // Trigger the explosion of the initial TNT block.
        yield return StartCoroutine(TriggerExplosion(tnt, area));
        GameManager.Instance.StopFalling();
    }


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

        HashSet<Block> newBlocks = new HashSet<Block>();
        foreach (Block block in blocksToExplode)
        {
            newBlocks.Add(block);
        }
        newBlocks.Add(tnt);

        GameManager.Instance.StartFalling();

        foreach (Block block in blocksToExplode)
        {
            if (block == null)
            {
                newBlocks.Remove(block);
                continue;
            }

            if (block.type == Block.BlockType.Obstacle)
            {
                Obstacle obstacle = (Obstacle)block;
                if (obstacle.obstacleType == Obstacle.ObstacleType.Vase && obstacle.GetComponent<SpriteRenderer>().sprite == obstacle.vaseSprite1)
                {
                    newBlocks.Remove(block);
                }
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
        yield return new WaitForSeconds(0.01f * blocksToExplode.Count);
        GameManager.Instance.StopFalling();
    }



    private HashSet<Block> FindTNTNeighbors(TNT tnt, int area)
    {
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

    public Block GetBlock(int x, int y)
    {
        return grid[x, y];
    }

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
