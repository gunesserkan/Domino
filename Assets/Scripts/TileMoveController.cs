using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Assets.Scripts;

public class TileMoveController : MonoBehaviour
{
    public Vector3 tileFirstLocation;
    private Vector3 offset;
    private bool isDragging = false;
    //private Vector3 initialPosition;
    private Camera mainCamera;
    private GameManager gameManager;
    private float distanceToFirst,distanceToLast;
    private GameObject firstTile, lastTile;

    private void Awake()
    {
        distanceToFirst = 0;
        distanceToLast = 0;
        isDragging = false;
        mainCamera = Camera.main;
        gameManager = FindObjectOfType<GameManager>(); // GameManager nesnesini bul
    }

    private void OnMouseDown()
    {
        if (GetComponent<BoxCollider2D>().enabled == true)
        {
            tileFirstLocation = transform.position;
            offset = transform.position - GetMouseWorldPosition();
            isDragging = true;
        }
    }
    private void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
            GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        
    }
    private Vector3 GetMouseWorldPosition()
    {
        // Fare pozisyonunu ekrandan dünya koordinatlarına çevir
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z); // Z mesafesi

        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
    private void OnMouseUp()
    {
        isDragging = false;
        if (transform.position.y <= -3.15f)
        {
            transform.position = tileFirstLocation;
            GetComponent<SpriteRenderer>().sortingOrder = 0;
            return;
        }
        if (gameManager.controller.board.Count != 0)
        {
             firstTile = gameManager.controller.board[0];
             lastTile = gameManager.controller.board[gameManager.controller.board.Count - 1];
        }
        else
        {
            gameManager.clickedTile(this.gameObject, null);
            return;
        }

        checkTile();
    }
    public void checkTile() {

        bool isFromBoneYard = gameManager.yard.tiles.Contains(this.gameObject);
        if (isFromBoneYard)
        {
            gameManager.controller.playingPlayer.GetHand().Insert(0, gameManager.yard.getFromYard());
            gameManager.controller.playingPlayer.GetHand()[0].GetComponent<PlayerHandler>().player = gameManager.controller.playingPlayer;
            gameManager.controller.playingPlayer.GetHand()[0].transform.SetParent(gameManager.gameBoard.transform, false);
            gameManager.controller.replaceTiles(gameManager.controller.playingPlayer);
            gameManager.controller.checkPlayableTiles(gameManager.controller.playingPlayer);
            if(GetComponent<BoxCollider2D>() != null)
            {
                GetComponent<BoxCollider2D>().enabled = true;
            }
        }
        else
        {
            distanceToFirst = Vector3.Distance(transform.position, firstTile.transform.position);
            distanceToLast = Vector3.Distance(transform.position, lastTile.transform.position);
            if (firstTile == lastTile)
            {
                gameManager.clickedTile(this.gameObject, firstTile);
            }
            else if (distanceToLast < distanceToFirst)
            {
                gameManager.clickedTile(this.gameObject, lastTile);
            }
            else if (distanceToFirst < distanceToLast)
            {

                gameManager.clickedTile(this.gameObject, firstTile);
            }
            else
            {
                this.transform.position = tileFirstLocation;
            }
        }
    }

    public void MoveToScreenCenter()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
    }

    //Merkeze git ve 90 derece dön
    public void MoveToScreenCenterAndRotateDefault()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        transform.rotation = Quaternion.Euler(0f,0f,180f);
    }

    //çift taş yerleştirme
    public void MoveToLocation(Vector3 targetPosition)
    {
        // Konumu doğrudan ayarlama
        transform.position = targetPosition;
    }

    //tek taş yerleştirme saat yönünde 90 döndürerek
    public void MoveToLocationAndRotateRight(Vector3 targetPosition)
    {
        // Konumu doğrudan ayarlama
        transform.position = targetPosition;

        // Saat yönünde 90 derece dönme
        transform.rotation = Quaternion.Euler(0f, 0f, -90f) * transform.rotation;
    }
    //tek taş yerleştirme saat yönü tersine 90 döndürerek
    public void MoveToLocationAndRotateLeft(Vector3 targetPosition)
    {
        // Konumu doğrudan ayarlama
        transform.position = targetPosition;

        // Saat yönünde 90 derece dönme
        transform.rotation = Quaternion.Euler(0f, 0f,90f) * transform.rotation;
    }

}
