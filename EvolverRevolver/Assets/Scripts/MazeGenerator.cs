using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell mazeCellPrefab;
    [SerializeField] private int mazeWidth;
    [SerializeField] private int mazeDepth;

    [SerializeField] private GameObject playerPrefab;

    private MazeCell[,] mazeGrid;

    private void Start()
    {
        mazeGrid = new MazeCell[mazeWidth,mazeDepth];
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeDepth; z++)
            {
                mazeGrid[x,z] = Instantiate(
                    mazeCellPrefab,
                    new Vector3(
                        x * mazeCellPrefab.transform.localScale.x,
                        0,
                        z * mazeCellPrefab.transform.localScale.z
                        )
                    ,Quaternion.identity);
            }
        }
        GenerateMaze(null, mazeGrid[0,0]);
        SpawnPlayerAtCenter();
    }

    private void SpawnPlayerAtCenter()
    {
        // Calculate the center indices of the maze grid
        int centerX = mazeWidth / 2;
        int centerZ = mazeDepth / 2;

        // Get the corresponding cell in the center
        MazeCell centerCell = mazeGrid[centerX, centerZ];

        // Get the center position of the cell (use its position)
        Vector3 centerPosition = new Vector3(
            centerCell.transform.position.x,
            centerCell.transform.position.y,  // Adjust based on level design
            centerCell.transform.position.z
        );

        // Instantiate the player at the center position
        Instantiate(playerPrefab, centerPosition, Quaternion.identity);
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell,currentCell);


        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);
            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);

    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        return unvisitedCells.OrderBy(random => Random.Range(1,10)).FirstOrDefault();
    }


    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        // Get the current cell's grid position (these should be integers)
        int x = (int)currentCell.transform.position.x / (int)mazeCellPrefab.transform.localScale.x;
        int z = (int)currentCell.transform.position.z / (int)mazeCellPrefab.transform.localScale.z;

        // Check the right neighbor (x+1)
        if (x + 1 < mazeWidth)
        {
            var cellToRight = mazeGrid[x + 1, z];
            if (!cellToRight.IsVisited)
            {
                yield return cellToRight;
            }
        }

        // Check the left neighbor (x-1)
        if (x - 1 >= 0)
        {
            var cellToLeft = mazeGrid[x - 1, z];
            if (!cellToLeft.IsVisited)
            {
                yield return cellToLeft;
            }
        }

        // Check the front neighbor (z+1)
        if (z + 1 < mazeDepth)
        {
            var cellToFront = mazeGrid[x, z + 1];
            if (!cellToFront.IsVisited)
            {
                yield return cellToFront;
            }
        }

        // Check the back neighbor (z-1)
        if (z - 1 >= 0)
        {
            var cellToBack = mazeGrid[x, z - 1];
            if (!cellToBack.IsVisited)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }
        if(previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }
        if(previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }
        if(previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
        if(previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }
    }

}
