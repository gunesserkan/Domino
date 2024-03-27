using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class DominoDizme2D : MonoBehaviour
{

    // Baþlangýçta önümüze yerleþtireceðimiz taþ sayýsý
    private int önümüzeYerleþtirilecekTaþSayýsý = 12;
    public Sprite[] dominoSpriteListesi;
    private Player1 player1=new Player1();
    private Player2 player2=new Player2();

    void Start()
    {
        List<GameObject> rastgeleTaþlar = new List<GameObject>();
        // Rastgele taþlarý seçmek için bir liste oluþturuyoruz

        foreach (Sprite sprite in dominoSpriteListesi)
        {
            GameObject yeniDomino = new GameObject(sprite.name);
            yeniDomino.AddComponent<SpriteRenderer>().sprite = sprite;
            yeniDomino.transform.position = new Vector3(-200f, 1f, 1f);
            rastgeleTaþlar.Add(yeniDomino);
        }
        GameObject tmp = rastgeleTaþlar[0];

        //Player1 taþlar
        int coordinateX = -42;
        for (int i = 0; i < önümüzeYerleþtirilecekTaþSayýsý; i++)
        {
            // Rastgele bir indeks seçiyoruz
            int rastgeleIndex = Random.Range(0, rastgeleTaþlar.Count);

            // Seçilen taþý önümüze yerleþtiriyoruz
            GameObject önümüzeYerleþtirilecekTaþ = rastgeleTaþlar[rastgeleIndex];
            // Önümüze yerleþtirdikten sonra listeden çýkarýyoruz
            rastgeleTaþlar.RemoveAt(rastgeleIndex);

            // Taþý sahneye ekliyoruz
            GameObject yeniTaþ = Instantiate(önümüzeYerleþtirilecekTaþ, new Vector3(coordinateX, -70, 0), Quaternion.identity);
            // Daha düzenli görünmesi için taþlarý birbirinden biraz uzaklaþtýrýyoruz (i * 2)
            coordinateX += 15;

            //Taþý 2D olarak ayarlýyoruz
            yeniTaþ.transform.localScale = new Vector3(7f, 7f, 1f); // Ölçeklerini ayarlayabilirsiniz
            yeniTaþ.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            player1.addBones(önümüzeYerleþtirilecekTaþ);
        }

        coordinateX = -42;
        //Player2 taþlar
        for (int i = 0; i < önümüzeYerleþtirilecekTaþSayýsý; i++)
        {
            // Rastgele bir indeks seçiyoruz
            int rastgeleIndex = Random.Range(0, rastgeleTaþlar.Count);

            // Seçilen taþý önümüze yerleþtiriyoruz
            GameObject önümüzeYerleþtirilecekTaþ = rastgeleTaþlar[rastgeleIndex];
            // Önümüze yerleþtirdikten sonra listeden çýkarýyoruz
            rastgeleTaþlar.RemoveAt(rastgeleIndex);

            // Taþý sahneye ekliyoruz
            GameObject yeniTaþ = Instantiate(tmp, new Vector3(coordinateX, 85, 0), Quaternion.identity);
            // Daha düzenli görünmesi için taþlarý birbirinden biraz uzaklaþtýrýyoruz (i * 2)
            coordinateX += 15;

            // Taþý 2D olarak ayarlýyoruz
            yeniTaþ.transform.localScale = new Vector3(7f, 7f, 1f); // Ölçeklerini ayarlayabilirsiniz
            yeniTaþ.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            player2.addBones(önümüzeYerleþtirilecekTaþ);
        }
    }
}

