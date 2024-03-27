using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class DominoDizme2D : MonoBehaviour
{

    // Ba�lang��ta �n�m�ze yerle�tirece�imiz ta� say�s�
    private int �n�m�zeYerle�tirilecekTa�Say�s� = 12;
    public Sprite[] dominoSpriteListesi;
    private Player1 player1=new Player1();
    private Player2 player2=new Player2();

    void Start()
    {
        List<GameObject> rastgeleTa�lar = new List<GameObject>();
        // Rastgele ta�lar� se�mek i�in bir liste olu�turuyoruz

        foreach (Sprite sprite in dominoSpriteListesi)
        {
            GameObject yeniDomino = new GameObject(sprite.name);
            yeniDomino.AddComponent<SpriteRenderer>().sprite = sprite;
            yeniDomino.transform.position = new Vector3(-200f, 1f, 1f);
            rastgeleTa�lar.Add(yeniDomino);
        }
        GameObject tmp = rastgeleTa�lar[0];

        //Player1 ta�lar
        int coordinateX = -42;
        for (int i = 0; i < �n�m�zeYerle�tirilecekTa�Say�s�; i++)
        {
            // Rastgele bir indeks se�iyoruz
            int rastgeleIndex = Random.Range(0, rastgeleTa�lar.Count);

            // Se�ilen ta�� �n�m�ze yerle�tiriyoruz
            GameObject �n�m�zeYerle�tirilecekTa� = rastgeleTa�lar[rastgeleIndex];
            // �n�m�ze yerle�tirdikten sonra listeden ��kar�yoruz
            rastgeleTa�lar.RemoveAt(rastgeleIndex);

            // Ta�� sahneye ekliyoruz
            GameObject yeniTa� = Instantiate(�n�m�zeYerle�tirilecekTa�, new Vector3(coordinateX, -70, 0), Quaternion.identity);
            // Daha d�zenli g�r�nmesi i�in ta�lar� birbirinden biraz uzakla�t�r�yoruz (i * 2)
            coordinateX += 15;

            //Ta�� 2D olarak ayarl�yoruz
            yeniTa�.transform.localScale = new Vector3(7f, 7f, 1f); // �l�eklerini ayarlayabilirsiniz
            yeniTa�.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            player1.addBones(�n�m�zeYerle�tirilecekTa�);
        }

        coordinateX = -42;
        //Player2 ta�lar
        for (int i = 0; i < �n�m�zeYerle�tirilecekTa�Say�s�; i++)
        {
            // Rastgele bir indeks se�iyoruz
            int rastgeleIndex = Random.Range(0, rastgeleTa�lar.Count);

            // Se�ilen ta�� �n�m�ze yerle�tiriyoruz
            GameObject �n�m�zeYerle�tirilecekTa� = rastgeleTa�lar[rastgeleIndex];
            // �n�m�ze yerle�tirdikten sonra listeden ��kar�yoruz
            rastgeleTa�lar.RemoveAt(rastgeleIndex);

            // Ta�� sahneye ekliyoruz
            GameObject yeniTa� = Instantiate(tmp, new Vector3(coordinateX, 85, 0), Quaternion.identity);
            // Daha d�zenli g�r�nmesi i�in ta�lar� birbirinden biraz uzakla�t�r�yoruz (i * 2)
            coordinateX += 15;

            // Ta�� 2D olarak ayarl�yoruz
            yeniTa�.transform.localScale = new Vector3(7f, 7f, 1f); // �l�eklerini ayarlayabilirsiniz
            yeniTa�.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            player2.addBones(�n�m�zeYerle�tirilecekTa�);
        }
    }
}

