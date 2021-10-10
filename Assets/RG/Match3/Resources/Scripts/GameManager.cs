using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int rows = 10;
    [SerializeField] private int colls = 10;
    [SerializeField] private int maxSize = 10;
    [SerializeField] private int minSize = 3;
    [SerializeField] private List<GameObject> tilesList;

    private List<GameObject> tilesInMatrix = new List<GameObject>();
    private int match = 3;
    private int previousTile;
    private int counter;
    private float offset = 0.5f;
    private Vector3 startPosition = new Vector3();
    private int index;
    private bool activeDestroy;
    private float secondsToUpdateMatrix = 0.2f;
    private float elapsedSecondsToUpdate;
    private bool activeUpdating;

    public bool ActiveDestroy { get => activeDestroy; set => activeDestroy = value; }
    public bool ActiveUpdating { get => activeUpdating; set => activeUpdating = value; }

    void Start() {        
        previousTile = 5;
        activeDestroy = true;
        SetMatrixConditions(); //Set max number and min number of colls and rows
        CreateMatrix();

    }
    private void Update() {
        if(CheckIfUpdateMatrix() && activeUpdating) {
            UpdateMatrix();
            activeUpdating = false;
        }
    }

    private void SetMatrixConditions() {
        int divisor = 2;        
        if(rows > maxSize) {
            rows = maxSize;
        }
        else if(rows < minSize) {
            rows = minSize;
        }
        if(colls > maxSize) {
            colls = maxSize;
        }
        else if(colls < minSize) {
            colls = minSize;
        }
        startPosition = new Vector3(-colls / divisor, -rows / divisor, 0);
    }
    private void CreateMatrix() {
        for(int row = 0; row < rows; row ++) {
            for(int col = 0; col < colls; col ++) {
                SpawnItemInStartPosition(row, col); // Decide the position of the tile.
            }

        }
    }

    private void SpawnItemInStartPosition(int row, int col) {
        
        Vector3 itemStartPosition = new Vector3(startPosition.x + col + offset, startPosition.y + row + offset, 0); // Get the item start position.
        GameObject itemToSpawn;
        GameObject instantiatedItem;
        itemToSpawn = GetRandomItem(row, col); //Get random tile.
        instantiatedItem = Instantiate(itemToSpawn, itemStartPosition, Quaternion.identity);
        instantiatedItem.GetComponent<TileController>().SetUpTile(row, col, index, this);
        tilesInMatrix.Add(instantiatedItem);

    }
    private GameObject GetRandomItem(int row, int col) {
        GameObject tile;
        index = Random.Range(0, tilesList.Count);
        while(index == previousTile && counter==2) { // Choose different tile if the selected has already been choosen twice.
            index = Random.Range(0, tilesList.Count);
        }
        if(counter == 2) {
            counter = 0;
        }
        previousTile = index;
        counter += 1;
        tile = tilesList[index];     
        return tile;
    }
   public void DestroyItem(int row, int col, GameObject tileToDestroy) {
        if(activeDestroy) {
            activeDestroy = false;
            tilesInMatrix.Remove(tileToDestroy);
            TileController tileController;
            foreach(GameObject tile in tilesInMatrix) {
                tileController = tile.GetComponent<TileController>();
                if(tileController.Row >= row && tileController.Col == col) {
                    tileController.ActiveMoveDown = true;
                }
            }
            Destroy(tileToDestroy);
            activeDestroy = true;
        }
    }
    public void UpdateMatrix() {
        List<TileController> tiles = new List<TileController>();

        foreach(GameObject item in tilesInMatrix) {
            if(item != null) {
                tiles.Add(item.GetComponent<TileController>());
            }
        }
        
        for(int i = 0; i < rows; i++) {
            List<TileController> tilesInRow = new List<TileController>();
            List<TileController> tilesToRemove = new List<TileController>();
            int previousCol = 0;
            int count = 0;
            int previousType = 5;
            foreach(TileController tile in tiles) {
                if(tile.Row == i) {
                    tilesInRow.Add(tile);
                }
            }
            tilesInRow = tilesInRow.OrderBy(tilesInRow => tilesInRow.Col).ToList();
            foreach(TileController tile in tilesInRow) {
                if(tile.Col != 0 && tile.Col > previousCol + 1) {
                    previousType = 5;
                }
                previousCol = tile.Col;
                if(tile.Type == previousType) {
                    count += 1;
                    tilesToRemove.Add(tile);

                }
                else {
                    previousType = tile.Type;
                    if(tilesToRemove.Count >= match) {
                        RemoveItems(tilesToRemove);
                        return;
                    }
                    tilesToRemove.Clear();
                    count = 1;
                    tilesToRemove.Add(tile);
                }
            }
            if(tilesToRemove.Count >= match) {
                RemoveItems(tilesToRemove);
                return;
            }
            else if(tilesToRemove.Count != 0) {
                tilesToRemove.Clear();
            }
            tilesInRow.Clear();
            count = 0;
        }   
    }
   
    private void RemoveItems(List<TileController> items) {
        foreach(TileController item in items) {
            item.DestroyItem();
        }
    }
    
    private bool CheckIfUpdateMatrix() {
        elapsedSecondsToUpdate += Time.deltaTime;
        if(elapsedSecondsToUpdate >= secondsToUpdateMatrix) {
            elapsedSecondsToUpdate -= elapsedSecondsToUpdate;
            return true;
        }
        else {
            return false;
        }
    }
}

