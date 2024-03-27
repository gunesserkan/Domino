using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1
{

    private OrtalamaTaþlar ortalama;
    private List<GameObject> bones = new List<GameObject>();
    public void addBones(GameObject obj) {
        bones.Add(obj);
        ortalama = new OrtalamaTaþlar(bones);
        Debug.Log("Player1: "+obj.name);
    }
    public void removeBones(GameObject obj) {  bones.Remove(obj); }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
