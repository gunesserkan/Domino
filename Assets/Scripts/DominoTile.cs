using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using System.Collections;
using UnityEngine.EventSystems;

public class DominoTile 
{
    public int leftValue;             // Sol tarafýn deðeri
    public int rightValue;            // Sað tarafýn deðeri
    public Sprite tileSprite;         // Taþýn görseli
    public int possitionLeft=0,possitionRight=0,possitionTop=0,possitionBottom=0;

    // Taþýn deðerlerini ve görselini ayarlamak için bir yöntem
    public  DominoTile(int left, int right, Sprite sprite)
    {
        leftValue = left;
        rightValue = right;
        tileSprite = sprite;
    }
    // Taþýn çift olup olmadýðýný kontrol eden bir metot
    public bool IsDouble()
    {
        return leftValue == rightValue;
    }

    // Taþýn toplam deðerini döndüren bir metot
    public int GetTotalValue()
    {
        return leftValue + rightValue;
    }

    // Taþý bir metin olarak temsil eden bir metot
    public override string ToString()
    {
        return leftValue + "-" + rightValue;
    }

    public Sprite GetSprite() { 
        return tileSprite;
    }
}
