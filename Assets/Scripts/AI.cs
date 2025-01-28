using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AI : MonoBehaviour
{
    public GameController controller;
    public GameManager manager;
    public Player player;
    private bool isPlaying = false;
    private GameObject tileToPlace;
    private GameObject tileTarget;
    public List<GameObject> playableTilesToFist;
    public List<GameObject> playableTilesToLast;
    // Update is called once per frame
    public void Start()
    {

    }


    public void move()
    {
            StartCoroutine(PlayAfterDelay(2f));
    }

    IEnumerator PlayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        tileToPlace = null;
        tileTarget = null;
        if (player.GetHand().Count != 0)
        {
            tileToPlace = play();

        }
        if (tileToPlace != null)
        {
            controller.PlaceTile(tileToPlace, tileTarget);
        }
    }

    public GameObject play()
    {


        if (controller.board.Count == 0)
        {
            return findBigDouble();
        }
        GameObject playingTile = null;
        GameObject firstTile = controller.board[0];
        GameObject lastTile = controller.board[controller.board.Count - 1];
        int[] tilescores=new int[7];
        int point;
        playableTilesToFist = new List<GameObject>();
        playableTilesToLast = new List<GameObject>();
        foreach (GameObject tile in player.GetHand())
        {
            point = 0;
            if (firstTile.GetComponent<TileCatcher>().dominoTile.leftValue == tile.GetComponent<TileCatcher>().dominoTile.leftValue ||
               firstTile.GetComponent<TileCatcher>().dominoTile.rightValue == tile.GetComponent<TileCatcher>().dominoTile.rightValue ||
               firstTile.GetComponent<TileCatcher>().dominoTile.leftValue == tile.GetComponent<TileCatcher>().dominoTile.rightValue ||
               firstTile.GetComponent<TileCatcher>().dominoTile.rightValue == tile.GetComponent<TileCatcher>().dominoTile.leftValue)
            {
                playableTilesToFist.Add(tile);
                point = 1;
            }
            if (lastTile.GetComponent<TileCatcher>().dominoTile.leftValue == tile.GetComponent<TileCatcher>().dominoTile.leftValue ||
                    lastTile.GetComponent<TileCatcher>().dominoTile.rightValue == tile.GetComponent<TileCatcher>().dominoTile.rightValue ||
                    lastTile.GetComponent<TileCatcher>().dominoTile.leftValue == tile.GetComponent<TileCatcher>().dominoTile.rightValue ||
                    lastTile.GetComponent<TileCatcher>().dominoTile.rightValue == tile.GetComponent<TileCatcher>().dominoTile.leftValue)
            {
                playableTilesToLast.Add(tile);
                point = 1;
                
            }
            if(point==1)
            {
                tilescores[(int)tile.GetComponent<TileCatcher>().dominoTile.leftValue]++;
            } 
        }
        int bigScore = -1, bigValue = -1;
        for(int i = 0; i < tilescores.Length; i++)
        {
            if (tilescores[i] >= bigScore)
            {
                bigScore = tilescores[i];
                bigValue = i;
            }
        }
        
        float firstOrLast;
        if (playableTilesToFist.Count != 0 && playableTilesToLast.Count != 0)
        {
            do
            {
                firstOrLast = Random.Range(0.0f, 1.0f);
            } while (firstOrLast == 0f);

        }
        else if (playableTilesToFist.Count != 0)
        {
            firstOrLast = 0.5f;
        }
        else
        {
            firstOrLast = 0f;
        }
        int randomIndex = 0;
        if (firstOrLast >= 0.5)
        {
            foreach(GameObject gobj in playableTilesToFist)
            {
                if (gobj.GetComponent<TileCatcher>().dominoTile.leftValue == bigValue || gobj.GetComponent<TileCatcher>().dominoTile.rightValue == bigValue)
                {
                    removeTile(playableTilesToLast, gobj);
                    playableTilesToFist.Remove(gobj);
                    tileTarget = firstTile;
                    return gobj;
                }
            }
            randomIndex = Random.Range(0, playableTilesToFist.Count - 1);
            playingTile = playableTilesToFist[randomIndex];
            playableTilesToFist.Remove(playingTile);
            tileTarget = firstTile;
            removeTile(playableTilesToLast, playingTile);
        }
        else
        {
            foreach (GameObject gobj in playableTilesToLast)
            {
                if (gobj.GetComponent<TileCatcher>().dominoTile.leftValue == bigValue || gobj.GetComponent<TileCatcher>().dominoTile.rightValue == bigValue)
                {
                    removeTile(playableTilesToFist, gobj);
                    playableTilesToLast.Remove(gobj);
                    tileTarget = lastTile;
                    return gobj;
                }
            }
            randomIndex = Random.Range(0, playableTilesToLast.Count - 1);
            playingTile = playableTilesToLast[randomIndex];
            playableTilesToLast.Remove(playingTile);
            tileTarget = lastTile;
            removeTile(playableTilesToFist, playingTile);
        }

        return playingTile;
    }
    private void removeTile(List<GameObject> list,GameObject gobje)
    {
        foreach(GameObject tile in list)
        {
            if (tile == gameObject)
            {
                list.Remove(tile);
                break;
            }
        }
    }
    public GameObject findBigDouble()
        {
            GameObject big = null;
            int value = -1;
            foreach (GameObject obj in player.GetHand())
            {
                if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                {
                    if (obj.GetComponent<TileCatcher>().dominoTile.leftValue > value)
                    {
                        big = obj;
                    }
                }
            }
            return big;
        }
        public void takeTileFromBoneYard()
        {
            if (manager.yard.tiles.Count !=0)
            {
                manager.yard.tiles[0].GetComponent<TileMoveController>().checkTile();
                manager.showYardInformation();
                controller.checkPlayableTiles(player);
            }
            else
            {
                manager.finishTheGame();
            }

        }
    }
