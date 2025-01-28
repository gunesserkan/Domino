using UnityEngine;
using System.Collections.Generic;
using System;
using Assets.Scripts;

public class Player
{
    public string playerName;         
    public List<GameObject> handTiles;  // Oyuncunun elindeki taşlar
    public int score;
    public bool ai;

    public Player(string name)
    {
        playerName = name;
        handTiles = new List<GameObject>();
        score = 0;
    }

    // Oyuncunun eline yeni bir domino taşı ekler
    public void AddTileToHand(GameObject tile)
    {
        handTiles.Add(tile);
    }

    // Oyuncunun elinden bir domino taşı çıkarır
    public void RemoveTileFromHand(GameObject tile)
    {
        handTiles.Remove(tile);
    }

    // Oyuncunun elindeki domino taşlarını gösterir
    public void DisplayHandTiles()
    {
        Debug.Log(playerName + "'s Hand Tiles:");
        foreach (var tile in handTiles)
        {
            Debug.Log(tile.GetComponent<TileCatcher>().dominoTile.ToString());
        }
    }

    // Oyuncunun elindeki toplam domino taşı sayısını döndürür
    public int GetHandSize()
    {
        return handTiles.Count;
    }

    // Oyuncunun elindeki domino taşlarını döndürür
    public List<GameObject> GetHand()
    {
        return handTiles;
    }

    // Oyuncunun skorunu günceller
    private void UpdateScore()
    {
        score += 1; // Örnek bir skor güncelleme işlemi
    }
    public bool isAi() { 
    return ai; }
    public void setIsAi(bool ai) { 
        this.ai= ai;
    }
}
