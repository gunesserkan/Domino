using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class OrtalamaTaşlar
{
    private List<GameObject> taşObjeleri; // Taşların GameObject'lerini tutan liste

    // Constructor oluşturarak script'e liste parametresi ekliyoruz
    public OrtalamaTaşlar(List<GameObject> taşListesi)
    {
        taşObjeleri = taşListesi;
        OrtalaTaşlar();
    }

    // Ortalama taşlar fonksiyonu
    void OrtalaTaşlar()
    {
        float toplamMesafe = 0f;
        float toplamTaşSayısı = taşObjeleri.Count;

        // Toplam mesafeyi hesapla
        foreach (GameObject taşObjesi in taşObjeleri)
        {
            toplamMesafe += taşObjesi.transform.position.x;
        }

        // Ortalama mesafeyi hesapla
        float ortalamaMesafe = toplamMesafe / toplamTaşSayısı;

        // Taşların pozisyonlarını yeniden hesapla
        for (int i = 0; i < taşObjeleri.Count; i++)
        {
            Vector3 yeniPozisyon = taşObjeleri[i].transform.position;
            yeniPozisyon.x = ortalamaMesafe + 15; // Örneğin, taşlar arasında 2 birim mesafe bırakıyoruz
            taşObjeleri[i].transform.position = yeniPozisyon;
            taşObjeleri[i].transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            taşObjeleri[i].transform.localScale = new Vector3(7f, 7f, 1f);
        }
    }
}

