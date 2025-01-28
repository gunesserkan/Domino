using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using System.Collections;
using UnityEngine.EventSystems;

public class DominoTile 
{
    public int leftValue;             // Sol taraf�n de�eri
    public int rightValue;            // Sa� taraf�n de�eri
    public Sprite tileSprite;         // Ta��n g�rseli
    public int possitionLeft=0,possitionRight=0,possitionTop=0,possitionBottom=0;

    // Ta��n de�erlerini ve g�rselini ayarlamak i�in bir y�ntem
    public  DominoTile(int left, int right, Sprite sprite)
    {
        leftValue = left;
        rightValue = right;
        tileSprite = sprite;
    }
    // Ta��n �ift olup olmad���n� kontrol eden bir metot
    public bool IsDouble()
    {
        return leftValue == rightValue;
    }

    // Ta��n toplam de�erini d�nd�ren bir metot
    public int GetTotalValue()
    {
        return leftValue + rightValue;
    }

    // Ta�� bir metin olarak temsil eden bir metot
    public override string ToString()
    {
        return leftValue + "-" + rightValue;
    }

    public Sprite GetSprite() { 
        return tileSprite;
    }
}
