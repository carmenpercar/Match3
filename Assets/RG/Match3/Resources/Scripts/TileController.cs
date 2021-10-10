using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController: MonoBehaviour {

    [SerializeField] private float smoothValue = 5.0f;
    [SerializeField] private AnimationCurve movementCurve;
    

    public GameManager gameManager;
    private bool activeMoveDown;
    private float elapsedSeconds;
    private int row;
    private int col;
    private int type;
    private Vector3 positionToMove;
    private Vector3 currentPosition;
    private bool repeatMovement = false;

    public int Row { get => row; set => row = value; }
    public int Col { get => col; set => col = value; }
    public int Type { get => type; set => type = value; }
    public GameManager GameManager { get => gameManager; set => gameManager = value; }
    public bool ActiveMoveDown { get => activeMoveDown; set => activeMoveDown = value; }

    private void Update() {
        if(activeMoveDown) {
            MoveDown();
        }
    }
    private void Start() {
        currentPosition = transform.position;
        positionToMove = currentPosition;
        UpdatePositionToMove();
        if(smoothValue == 0) {
            smoothValue = 0.1f;
        }
    }

    private void OnMouseDown() {
        if(!activeMoveDown) {
            DestroyItem();
        }        
    }
    public void DestroyItem() {
        GameManager.DestroyItem(row, col, this.gameObject);
    }
    public void MoveDown() {

        if(transform.position != positionToMove) {
            float currentValue = elapsedSeconds / smoothValue;
            elapsedSeconds += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, positionToMove, movementCurve.Evaluate(currentValue)); 
            gameManager.ActiveDestroy = false;

        }
        else{
            elapsedSeconds -= elapsedSeconds;
            transform.position = positionToMove;
            UpdateCurrentPosition();
            UpdatePositionToMove();
            gameManager.ActiveDestroy = true;
            gameManager.ActiveUpdating = true;
            activeMoveDown = false;
        }        
    }
    public void UpdateCurrentPosition() {
        currentPosition = transform.position;
        row -= 1;     
    }
    public void UpdatePositionToMove() {
        int pos = 1;
        positionToMove = new Vector3(positionToMove.x, positionToMove.y - pos, positionToMove.z); // New position to move
    }

    public void SetUpTile(int row, int col, int type, GameManager gameManager) { // init tile controller parameters.
        this.row = row;
        this.col = col;
        this.type = type;
        this.gameManager = gameManager;
    }
}