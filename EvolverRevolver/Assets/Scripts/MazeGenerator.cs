using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell mazeCellPrefab;
    [SerializeField] private int mazeWidth;
    [SerializeField] private int mazeDepth;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private EnemySpawner enemySpawnerPrefab;

    private MazeCell[,] mazeGrid;

    private void Start()
    {
        mazeGrid = new MazeCell[mazeWidth,mazeDepth];
        //for (int x = 0; x < mazeWidth; x++)
        //{
        //    for (int z = 0; z < mazeDepth; z++)
        //    {
        //        mazeGrid[x,z] = Instantiate(
        //            mazeCellPrefab,
        //            new Vector3(
        //                x * mazeCellPrefab.transform.localScale.x,
        //                0,
        //                z * mazeCellPrefab.transform.localScale.z
        //                )
        //            ,Quaternion.identity);
        //        mazeGrid[x, z].transform.SetParent(transform);
        //    }
        //}
        //GenerateMaze(null, mazeGrid[0,0]);
        LoadMaze();
        SpawnPlayerAtCenter();
        CreateEnemySpawners();
    }

    private void LoadMaze()
    {
        // Initialize the mazeGrid array with the correct size
        mazeGrid = new MazeCell[mazeWidth, mazeDepth];

        // Loop through all the children of the mazeParent (the saved maze)
        foreach (Transform child in transform)
        {
            // Get the scale of the maze cell
            float xScale = child.localScale.x;
            float zScale = child.localScale.z;

            // Get the position of the child
            // Adjust the x and z indices by dividing by the scale factor
            int x = Mathf.RoundToInt(child.position.x / xScale); // Adjust x based on scale
            int z = Mathf.RoundToInt(child.position.z / zScale); // Adjust z based on scale

            // Cast the child to a MazeCell (assuming it has a MazeCell component)
            MazeCell mazeCell = child.GetComponent<MazeCell>();

            if (mazeCell != null && x >= 0 && x < mazeWidth && z >= 0 && z < mazeDepth)
            {
                // Place the maze cell in the correct spot in the mazeGrid
                mazeGrid[x, z] = mazeCell;
            }
        }
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
        playerPrefab.transform.position = centerPosition;
    }

    private void CreateEnemySpawners()
    {
        Debug.Log("Creating EnemySpawners");
        for (int x = 2; x < mazeWidth - 2; x += 4) // Start at index 2 for the border
        {
            for (int z = 2; z < mazeDepth - 2; z += 4) // Place every 4 blocks, with a 2-block buffer
            {
                // Calculate the spawner's position at the center of the cell
                Vector3 spawnerPosition = new Vector3(
                    mazeGrid[x, z].transform.position.x,
                    mazeGrid[x, z].transform.position.y,
                    mazeGrid[x, z].transform.position.z
                );

                // Instantiate spawner
                EnemySpawner enemySpawner = Instantiate(enemySpawnerPrefab, spawnerPosition, Quaternion.identity);
                Debug.Log("!PLayer Transfom"+playerPrefab.transform);
                enemySpawner.SetPlayerControllerTransform(playerPrefab.transform);

                SetEnemySpawnerPatrolPoints(enemySpawner, x, z);
            }
        }
    }

    private void SetEnemySpawnerPatrolPoints(EnemySpawner enemySpawner, int startX, int startZ)
    {
        Transform point1 = mazeGrid[startX, startZ].transform;

        int patrolGap = 4;

        int endX = startX + patrolGap < mazeWidth ? startX + patrolGap : startX - patrolGap;
        int endZ = startZ + patrolGap < mazeDepth ? startZ + patrolGap : startZ - patrolGap;

        Transform point2 = mazeGrid[endX, endZ].transform;
        Debug.Log("Setting Enemy Patrol Points");
        enemySpawner.SetEnemyPatrolPoints(new Transform[] { point1, point2 });

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
