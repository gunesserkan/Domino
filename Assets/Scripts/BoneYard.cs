using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneYard : MonoBehaviour
{
    public List<GameObject> tiles;
    public Dictionary<GameObject,Sprite> sprites;

    public void Awake()
    {
        tiles = new List<GameObject>();
        sprites = new Dictionary<GameObject,Sprite>();
    }

    public void addToYard(GameObject go,Sprite sprite)
    {
        tiles.Add(go);
        sprites.Add(go, go.GetComponent<SpriteRenderer>().sprite);
        go.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public GameObject getFromYard()
    {
        GameObject go = tiles[tiles.Count - 1];
        go.GetComponent<SpriteRenderer>().sprite = sprites[go];
        GameObject obj = go;
        tiles.Remove(go);
        sprites.Remove(go);
        obj.GetComponent<BoxCollider2D>().enabled = false;
        return obj;
    }
}
