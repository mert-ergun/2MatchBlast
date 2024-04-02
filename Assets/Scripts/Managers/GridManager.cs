using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    private Block[,] grid;

    private int column;
    private int row;

    public float topY;

    public void InitializeGrid(Block[][] blocks)
    {
        row = blocks.Length;
        column = blocks[0].Length;
        grid = new Block[row, column];

        // Initialize the grid
        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < column; y++)
            {
                grid[x, y] = blocks[x][y];
                grid[x, y].SetX(x);
                grid[x, y].SetY(y);
            }
        }

        // Set the topY value
        topY = grid[0, 0].transform.position.y;
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
        List<Block> connectedBlocks = FindConnectedBlocks(block);

        if (connectedBlocks.Count == 1)
        {
            // Play jiggle animation
            block.GetComponent<Animator>().SetTrigger("Jiggle");
        }

        else if (connectedBlocks.Count >= 2)
        {
            GameManager.Instance.UseMove();
            // Reverse the order of the blocks to destroy the bottom blocks first
            connectedBlocks.Reverse();
            foreach (Block connectedBlock in connectedBlocks)
            {
                // Play the destroy animation
                connectedBlock.Explode();
                ObjectPool.Instance.ReturnToPool(connectedBlock.type.ToString(), connectedBlock.gameObject);
                // Remove the block from the grid
                grid[connectedBlock.GetX(), connectedBlock.GetY()] = null;   
            }

            
            GameManager.Instance.FallBlock();

            yield return new WaitForSeconds(0.05f);

            foreach (Block connectedBlock in connectedBlocks)
            {
                // Spawn a new block at the top
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
                yield return new WaitForSeconds(0.05f);
            }
        }
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
            if (currentBlock.type != Block.BlockType.Cube)
            {
                continue;
            }
            Cube cube = (Cube)currentBlock;

            connectedBlocks.Add(currentBlock);

            List<Block> neighbors = GetNeighbors(currentBlock);
            foreach (Block neighbor in neighbors)
            {
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
