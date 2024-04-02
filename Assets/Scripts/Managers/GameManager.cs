using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private TextMeshProUGUI moveCountText;
    public void HandleBlockTap(Block block)
    {
        if (moveCountText.text == "0")
        {
            return;
        }
        StartCoroutine(GridManager.Instance.HandleBlockTap(block));
       

    }

    public void UseMove()
    {
        // Update move count
        moveCountText.text = (int.Parse(moveCountText.text) - 1).ToString();
    }

    public void FallBlock()
    {
        // Fall all the blocks until they reach above another block
        for (int x = GridManager.Instance.GetRow()-1; x >= 0; x--)
        {
            for (int y = GridManager.Instance.GetColumn()-1; y >= 0; y--)
            {
                Block block = GridManager.Instance.GetBlock(x, y);
                if (block != null)
                {
                    int fallDistance = 0;
                    for (int i = x + 1; i < GridManager.Instance.GetRow(); i++)
                    {
                        if (GridManager.Instance.GetBlock(i, y) == null)
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
                        block.Fall(fallDistance);

                        GridManager.Instance.SetBlock(x, y, null);
                        GridManager.Instance.SetBlock(x + fallDistance, y, block);
                        block.SetX(x + fallDistance);
                        // Set the sorting order
                        block.GetComponent<SpriteRenderer>().sortingOrder -= fallDistance * GridManager.Instance.GetColumn();
                    }
                }
            }
        }
    }
    
}
