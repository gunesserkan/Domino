using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        public GameManager manager;
        public Player player1;
        public Player player2;
        public List<GameObject> board = new List<GameObject>(); // Taşların listesi
        //public Dictionary<GameObject, Vector3> tilePositions = new Dictionary<GameObject, Vector3>(); // Taşların konumlarını tutan dictionary
        //taşları yerleştirirken diğer taş ile olan mesafe
        public float xValueForPlace;
        public float yValueForPlace;
        public float scaleValue;
        public Player playingPlayer;
        public bool canPlay;
        //ekranın genişlik ve yükseklikleri
        float screenWidth;
        float screenHeight;
        //merkeze taş geldikten sonra başa ve sona eklenen taşları tutan değişkenler
        public float countFirst, countLast;
        //taş dizilim yönü değiştiğinde konumlandırmaların düzeltilmesi için değişkenler getxValue ve getYValue metodlarında kullanılırlar
        //float xForStart=0f,yForStart=0f;
        public string changeDirectionForFirst, changeDirectionForLast;
        //ilk taş ve son taş için GameObjectler
        public GameObject firstTile, lastTile;


        public void Start()
        {
            countFirst = 0; countLast = 0;
            scaleValue = 3f;
            firstStart();
            //xDirection = "out";
            //yDirection = "null";
        }
        private void Update()
        {
            xValueForPlace = 0f;
            yValueForPlace = 0f;
        }
        //başlangıçta çalıştırılacak metod, oyunun yeniden başlatılmasında da kullanılır
        public void firstStart()
        {
            playingPlayer = findFirst();
            if (playingPlayer == player2)
            {
                manager.playerAI.move();
            }
            else
            {
                canPlay = false;
            }
        }
        //başlangıçta en büyük çifte sahip oyuncunun bulunması
        public Player findFirst()
        {
            Player p = null;
            int value = -1 ;
            GameObject obj = null;

            foreach(GameObject go in player1.GetHand()) 
            {
                if (go.GetComponent<TileCatcher>().dominoTile.IsDouble() && go.GetComponent<TileCatcher>().dominoTile.leftValue > value)
                {
                    p=go.GetComponent<PlayerHandler>().player;
                    value = go.GetComponent<TileCatcher>().dominoTile.leftValue;
                    obj = go;
                }
            }

            foreach(GameObject go in player2.GetHand())
            {
                if (go.GetComponent<TileCatcher>().dominoTile.IsDouble() && go.GetComponent<TileCatcher>().dominoTile.leftValue > value)
                {
                    p = go.GetComponent<PlayerHandler>().player;
                    value = go.GetComponent<TileCatcher>().dominoTile.leftValue;
                }
            }
            if (p == player1)
            {
                obj.GetComponent<SpriteRenderer>().color = Color.green;
            }
            return (p!=null) ? p : player1;
        }

        //oyun sırası kontrolü
        public void changePlayer(Player player)
        {
            if (player==player1)
            {
                changeClickable(false);
                playingPlayer = player2;
                replaceTiles(player1);
                if (player.GetHand().Count != 0)
                {
                    checkPlayableTiles(playingPlayer);
                }
                manager.playerAI.move();
            }
            else if(player==player2)
            {
                playingPlayer = player1;
                replaceTiles(player2);
                if (player.GetHand().Count != 0)
                {
                    checkPlayableTiles(playingPlayer);
                }
                changeClickable(true);
            }
            Color color = manager.txt_information.color;
            color.a = 0f;
            manager.txt_information.color = color;
        }
        //oynanabilir taşları kontrol etme
        public void checkPlayableTiles(Player p)
        {
            DominoTile firstTile = board[0].GetComponent<TileCatcher>().dominoTile;
            DominoTile lastTile = board[board.Count - 1].GetComponent<TileCatcher>().dominoTile;
            bool playableTileFound = false;

            foreach (GameObject go in p.GetHand())
            {
                DominoTile playersTile = go.GetComponent<TileCatcher>().dominoTile;
                if (firstTile.leftValue == playersTile.leftValue ||
                    firstTile.rightValue == playersTile.rightValue ||
                    firstTile.leftValue == playersTile.rightValue ||
                    firstTile.rightValue == playersTile.leftValue ||
                    lastTile.leftValue == playersTile.leftValue ||
                    lastTile.rightValue == playersTile.rightValue ||
                    lastTile.leftValue == playersTile.rightValue ||
                    lastTile.rightValue == playersTile.leftValue)
                {
                    if (!p.isAi())
                    {
                        go.GetComponent<SpriteRenderer>().color = Color.green;
                    }
                    playableTileFound = true;
                }
                else if(!p.isAi())
                {
                    go.GetComponent<SpriteRenderer>().color = Color.grey;
                }
            }
            if (manager.yard.tiles.Count != 0)
            {
                if (!playableTileFound && !p.isAi())
                {
                    manager.isYardOpen(true);
                }
                else if (!playableTileFound && p.isAi())
                {
                    manager.playerAI.takeTileFromBoneYard();
                }
                else
                {
                    manager.isYardOpen(false);
                }
            }
            else
            {
                manager.finishTheGame();
            }
        }


        // Herhangi bir taş tıklamasını yakalama
        public void PlaceTile(GameObject obj,GameObject target)
        {
            Player playedPlayer = obj.GetComponent<PlayerHandler>().player;
           
            //Player playingPlayer = obj.GetComponent<PlayerHandler>().player;
            //DominoTile playingDomino = obj.GetComponent<TileCatcher>().dominoTile;
            
            // Eğer tahta boşsa
            if (target==null)
            {
                //taşın tek ve çift olmasına göre ölçeklenip masaya yerleştirilmesi
                if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                {
                    obj.transform.localScale = new Vector3(scaleValue, scaleValue, 0f);
                    obj.GetComponent<TileMoveController>().MoveToScreenCenter();
                }
                else
                {
                    obj.transform.localScale = new Vector3(scaleValue, scaleValue, 0f);
                    obj.GetComponent<TileMoveController>().MoveToScreenCenterAndRotateDefault();
                }
                //derinlik ayarlama
                obj.GetComponent<SpriteRenderer>().sortingOrder = 0;
                //Boş masaya tek taşların yatay gitmesini sağlayan kod
                board.Add(obj);
                //taşı elden çıkarma
                playingPlayer.handTiles.Remove(obj);
                if (playedPlayer.GetHandSize() <= 0)
                {
                    playingPlayer = null;
                    manager.finishTheGame();
                    manager.isYardOpen(false);
                    return;
                }
                //change player
                changePlayer(obj.GetComponent<PlayerHandler>().player);
                //tıklanma sesi
                manager.source.Play();
                obj.GetComponent<SpriteRenderer>().color = Color.white;
            }
            else
            {
                firstTile = board[0];
                lastTile = board[board.Count - 1];
                if (target == firstTile)
                {
                    checkForFirstTile(obj);
                }
                else if (target==lastTile)
                {
                    checkForLastTile(obj);
                }
            }

        }
        //ilk taş ile kontrol etme
        public void checkForFirstTile(GameObject obj)
        {
             if (firstTile.GetComponent<TileCatcher>().dominoTile.leftValue == obj.GetComponent<TileCatcher>().dominoTile.leftValue ||
                firstTile.GetComponent<TileCatcher>().dominoTile.rightValue == obj.GetComponent<TileCatcher>().dominoTile.rightValue ||
                firstTile.GetComponent<TileCatcher>().dominoTile.leftValue == obj.GetComponent<TileCatcher>().dominoTile.rightValue ||
                firstTile.GetComponent<TileCatcher>().dominoTile.rightValue == obj.GetComponent<TileCatcher>().dominoTile.leftValue)
                {

                //oynanan taşın yanına yerleşeceği taşa göre boşluk değerinin belirlenmesi
                directionForFirst(firstTile, obj);
                
                // Taşın tıklanabilirliğini kapat
                if (obj.GetComponent<BoxCollider2D>()!=null)
                {
                    obj.GetComponent<BoxCollider2D>().enabled = false;
                }
                //derinlik ayarlama
                obj.GetComponent<SpriteRenderer>().sortingOrder = 0;
                //oynanan taşın ölçeklenmesi
                obj.transform.localScale = new Vector3(scaleValue, scaleValue, 0f);
                // Taşı board listesinin başına ekle
                board.Insert(0, obj);
                //taşın desteden silinmesi
                playingPlayer.handTiles.Remove(obj);
                if (obj.GetComponent<PlayerHandler>().player.GetHandSize() <= 0)
                {
                    playingPlayer = null;
                    manager.finishTheGame();
                    manager.isYardOpen(false);
                    return;
                }
                //change player
                changePlayer(obj.GetComponent<PlayerHandler>().player);
                //tıklanma sesi
                manager.source.Play();
                obj.GetComponent<SpriteRenderer>().color = Color.white;
                countFirst++;
            }
            else
            {
                obj.transform.position = obj.GetComponent<TileMoveController>().tileFirstLocation;
            }
        }
        //son taş ile kontrol etme
        public void checkForLastTile(GameObject obj)
        {
            // Eğer obje lastTile ile eşleşiyorsa
            if (lastTile.GetComponent<TileCatcher>().dominoTile.leftValue == obj.GetComponent<TileCatcher>().dominoTile.leftValue ||
                    lastTile.GetComponent<TileCatcher>().dominoTile.rightValue == obj.GetComponent<TileCatcher>().dominoTile.rightValue ||
                    lastTile.GetComponent<TileCatcher>().dominoTile.leftValue == obj.GetComponent<TileCatcher>().dominoTile.rightValue ||
                    lastTile.GetComponent<TileCatcher>().dominoTile.rightValue == obj.GetComponent<TileCatcher>().dominoTile.leftValue)
            {
                //oynanan taşın yanına yerleşeceği taşa göre boşluk değerinin belirlenmesi
                directionForLast(lastTile, obj);
                // Taşın tıklanabilirliğini kapat
                if (!playingPlayer.isAi())
                {
                    obj.GetComponent<BoxCollider2D>().enabled = false;
                }
                //derinlik ayarlama
                obj.GetComponent<SpriteRenderer>().sortingOrder = 0;
                //oynanan taşın ölçeklenmesi
                obj.transform.localScale = new Vector3(scaleValue, scaleValue, 0f);
                // Taşı board listesinin sonuna ekle
                board.Add(obj);
                //taşın desteden silinmesi
                playingPlayer.handTiles.Remove(obj);
                if (obj.GetComponent<PlayerHandler>().player.GetHandSize() <= 0)
                {
                    playingPlayer = null;
                    manager.finishTheGame();
                    manager.isYardOpen(false);
                    return;
                }
                //change player
                changePlayer(obj.GetComponent<PlayerHandler>().player);
                //tıklanma sesi
                manager.source.Play();
                obj.GetComponent<SpriteRenderer>().color = Color.white;
                countLast++;
            }
            else
            {
                obj.transform.position = obj.GetComponent<TileMoveController>().tileFirstLocation;
            }
        }
        public void directionForFirst(GameObject firstTile,GameObject obj)
        {
            if (countFirst < 2)
            {
                xValueForPlace = getXvalue(firstTile, obj, "out");
                yValueForPlace = getYvalue(firstTile, obj, "null");
            }
            //yönelim değiştiğinde taşı ayarlama
            else if (countFirst < 8)
            {
                if (countFirst == 2)
                {
                    if (firstTile.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        firstTile.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        firstTile.transform.position = firstTile.transform.position + new Vector3(-0.15f,0.10f, 0f);
                    }
                    else
                    {
                        firstTile.transform.position = firstTile.transform.position +new Vector3(0.15f, 0.1f, 0f);
                        if (obj.GetComponent<TileCatcher>().dominoTile.leftValue == firstTile.GetComponent<TileCatcher>().dominoTile.leftValue)
                        {
                            firstTile.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.leftValue == firstTile.GetComponent<TileCatcher>().dominoTile.rightValue)
                        {
                            firstTile.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.rightValue == firstTile.GetComponent<TileCatcher>().dominoTile.leftValue)
                        {
                            firstTile.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.rightValue == firstTile.GetComponent<TileCatcher>().dominoTile.rightValue)
                        {
                            firstTile.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                        }
                    }

                }
                xValueForPlace = getXvalue(firstTile, obj, "null");
                yValueForPlace = getYvalue(firstTile, obj, "reverse");
                obj.transform.rotation= Quaternion.Euler(0f, 0f, 0f);
            }
            else if(countFirst<12)
            {
                if (countFirst == 8)
                {
                    if (firstTile.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        firstTile.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                        firstTile.transform.position = firstTile.transform.position + new Vector3(0f,0.15f, 0f);
                    }
                    else
                    {
                        firstTile.transform.rotation= Quaternion.Euler(0f, 0f, 90f);
                        firstTile.transform.position = firstTile.transform.position + new Vector3(+0.15f,-0.15f, 0f);
                        if (obj.GetComponent<TileCatcher>().dominoTile.leftValue == firstTile.GetComponent<TileCatcher>().dominoTile.leftValue)
                        {
                            firstTile.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.leftValue == firstTile.GetComponent<TileCatcher>().dominoTile.rightValue)
                        {
                            firstTile.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.rightValue == firstTile.GetComponent<TileCatcher>().dominoTile.leftValue)
                        {
                            firstTile.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.rightValue == firstTile.GetComponent<TileCatcher>().dominoTile.rightValue)
                        {
                            firstTile.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        }
                    }

                }
                xValueForPlace = getXvalue(firstTile, obj, "in");
                yValueForPlace = getYvalue(firstTile, obj, "null");
                obj.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
            }
            else
            {
                xValueForPlace = getXvalue(firstTile, obj, "null");
                yValueForPlace = getYvalue(firstTile, obj, "reverse")*-1;
                obj.transform.rotation = Quaternion.Euler(0f, 0f, -180f);
                if (countFirst == 12)
                {
                    if (firstTile.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        firstTile.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        firstTile.transform.position = firstTile.transform.position + new Vector3(0.13f, -0.08f, 0f);
                        if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                        {
                            yValueForPlace = yValueForPlace + 0.30f;
                        }
                        else
                        {
                            yValueForPlace = yValueForPlace - 0.02f;
                        }
                    }
                    else
                    {
                        firstTile.transform.rotation= Quaternion.Euler(0f, 0f, 0f);
                        firstTile.transform.position = firstTile.transform.position + new Vector3(-0.15f, -0.2f, 0f);
                        if (obj.GetComponent<TileCatcher>().dominoTile.leftValue == firstTile.GetComponent<TileCatcher>().dominoTile.leftValue)
                        {
                            firstTile.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.rightValue == firstTile.GetComponent<TileCatcher>().dominoTile.leftValue)
                        {
                            firstTile.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.leftValue == firstTile.GetComponent<TileCatcher>().dominoTile.rightValue)
                        {
                            firstTile.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.rightValue == firstTile.GetComponent<TileCatcher>().dominoTile.rightValue)
                        {
                            firstTile.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                        }

                    }

                }
            }


            if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
            {
                obj.GetComponent<TileMoveController>().MoveToLocation(new Vector3(firstTile.transform.position.x - xValueForPlace, firstTile.transform.position.y + yValueForPlace, firstTile.transform.position.z));

            }
            //Taşlar dikey şekilde konumlandırıldıklarında taşın alt tarafı sol üst tarafı sağ değeri olur!!
            else if (firstTile.GetComponent<TileCatcher>().dominoTile.leftValue == obj.GetComponent<TileCatcher>().dominoTile.leftValue)
            {
                obj.GetComponent<TileMoveController>().MoveToLocationAndRotateLeft(new Vector3(firstTile.transform.position.x - xValueForPlace, firstTile.transform.position.y + yValueForPlace, firstTile.transform.position.z));
                obj.GetComponent<TileCatcher>().dominoTile.leftValue = -1;
            }
            else if (firstTile.GetComponent<TileCatcher>().dominoTile.leftValue == obj.GetComponent<TileCatcher>().dominoTile.rightValue)
            {
                obj.GetComponent<TileMoveController>().MoveToLocationAndRotateRight(new Vector3(firstTile.transform.position.x - xValueForPlace, firstTile.transform.position.y + yValueForPlace, firstTile.transform.position.z));
                obj.GetComponent<TileCatcher>().dominoTile.rightValue = -1;
            }
            else if (firstTile.GetComponent<TileCatcher>().dominoTile.rightValue == obj.GetComponent<TileCatcher>().dominoTile.leftValue)
            {
                obj.GetComponent<TileMoveController>().MoveToLocationAndRotateLeft(new Vector3(firstTile.transform.position.x - xValueForPlace, firstTile.transform.position.y + yValueForPlace, firstTile.transform.position.z));
                obj.GetComponent<TileCatcher>().dominoTile.leftValue = -1;
            }
            else if (firstTile.GetComponent<TileCatcher>().dominoTile.rightValue == obj.GetComponent<TileCatcher>().dominoTile.rightValue)
            {
                obj.GetComponent<TileMoveController>().MoveToLocationAndRotateRight(new Vector3(firstTile.transform.position.x - xValueForPlace, firstTile.transform.position.y + yValueForPlace, firstTile.transform.position.z));
                obj.GetComponent<TileCatcher>().dominoTile.rightValue = -1;
            }
        }

        public void directionForLast(GameObject lastTile,GameObject obj) 
        {
            if (countLast < 2)
            {
                xValueForPlace = getXvalue(lastTile, obj, "out");
                yValueForPlace = getYvalue(lastTile, obj, "null");
            }
            else if (countLast < 6)
            {
                xValueForPlace = getXvalue(lastTile, obj, "null");
                yValueForPlace = getYvalue(lastTile, obj, "reverse");
                obj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                if (countLast == 2)
                {
                    if (lastTile.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        lastTile.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        lastTile.transform.position = lastTile.transform.position + new Vector3(0.13f, -0.08f, 0f);
                        if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                        {
                            yValueForPlace = yValueForPlace - 0.30f;
                        }
                        else
                        {
                            yValueForPlace = yValueForPlace + 0.02f;
                        }
                    }
                    else
                    {
                        lastTile.transform.position = lastTile.transform.position + new Vector3(-0.15f, -0.2f, 0f);
                        if (obj.GetComponent<TileCatcher>().dominoTile.leftValue == lastTile.GetComponent<TileCatcher>().dominoTile.leftValue)
                        {
                            lastTile.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                        }
                        else if(obj.GetComponent<TileCatcher>().dominoTile.leftValue == lastTile.GetComponent<TileCatcher>().dominoTile.rightValue)
                        {
                            lastTile.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.rightValue == lastTile.GetComponent<TileCatcher>().dominoTile.leftValue)
                        {
                            lastTile.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                        }
                        else if(obj.GetComponent<TileCatcher>().dominoTile.rightValue == lastTile.GetComponent<TileCatcher>().dominoTile.rightValue)
                        {
                            lastTile.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                        }
                        
                    }

                }
            }
            else if(countLast<11)
            {
                xValueForPlace = getXvalue(lastTile, obj, "in");
                yValueForPlace = getYvalue(lastTile, obj, "null");
                obj.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                if (countLast == 6)
                {
                    if (lastTile.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        lastTile.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                        lastTile.transform.position = lastTile.transform.position + new Vector3(-0.15f, -0.2f, 0f);
                        if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                        {
                            yValueForPlace = yValueForPlace - 0.30f;
                        }
                        else
                        {
                            yValueForPlace = yValueForPlace + 0.02f;
                        }
                    }
                    else
                    {
                        
                        lastTile.transform.position = lastTile.transform.position + new Vector3(-0.13f, 0.13f, 0f);
                        if (obj.GetComponent<TileCatcher>().dominoTile.leftValue == lastTile.GetComponent<TileCatcher>().dominoTile.leftValue)
                        {
                            lastTile.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.leftValue == lastTile.GetComponent<TileCatcher>().dominoTile.rightValue)
                        {
                            lastTile.transform.rotation = Quaternion.Euler(0f, 0f, -180f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.rightValue == lastTile.GetComponent<TileCatcher>().dominoTile.leftValue)
                        {
                            lastTile.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.rightValue == lastTile.GetComponent<TileCatcher>().dominoTile.rightValue)
                        {
                            lastTile.transform.rotation = Quaternion.Euler(0f, 0f, -180f);
                        }

                    }

                }
            }
            else
            {
                obj.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                if (countLast == 11)
                {
                    if (lastTile.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        lastTile.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        lastTile.transform.position = lastTile.transform.position + new Vector3(-0.15f, 0.10f, 0f);
                    }
                    else
                    {
                        obj.transform.rotation= Quaternion.Euler(0f, 0f, 180f);
                        lastTile.transform.rotation= Quaternion.Euler(0f, 0f, 0f);
                        lastTile.transform.position = lastTile.transform.position + new Vector3(0.15f, 0.1f, 0f);
                        if (obj.GetComponent<TileCatcher>().dominoTile.leftValue == lastTile.GetComponent<TileCatcher>().dominoTile.leftValue)
                        {
                            lastTile.transform.rotation = Quaternion.Euler(0f, 0f, -90);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.leftValue == lastTile.GetComponent<TileCatcher>().dominoTile.rightValue)
                        {
                            lastTile.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.rightValue == lastTile.GetComponent<TileCatcher>().dominoTile.leftValue)
                        {
                            lastTile.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                        }
                        else if (obj.GetComponent<TileCatcher>().dominoTile.rightValue == lastTile.GetComponent<TileCatcher>().dominoTile.rightValue)
                        {
                            lastTile.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                        }
                    }

                }
                xValueForPlace = getXvalue(lastTile, obj, "null");
                yValueForPlace = getYvalue(lastTile, obj, "reverse")*-1;
            }

            if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
            {
                // Taşı lastTile'ın sağına yerleştir
                obj.GetComponent<TileMoveController>().MoveToLocation(new Vector3(lastTile.transform.position.x + xValueForPlace, lastTile.transform.position.y - yValueForPlace, lastTile.transform.position.z));
            }
            else if (lastTile.GetComponent<TileCatcher>().dominoTile.leftValue == obj.GetComponent<TileCatcher>().dominoTile.leftValue)
            {
                // Sağ değer eşleşmesi
                obj.GetComponent<TileMoveController>().MoveToLocationAndRotateRight(new Vector3(lastTile.transform.position.x + xValueForPlace, lastTile.transform.position.y - yValueForPlace, lastTile.transform.position.z));
                obj.GetComponent<TileCatcher>().dominoTile.leftValue = -1;
            }
            else if (lastTile.GetComponent<TileCatcher>().dominoTile.leftValue == obj.GetComponent<TileCatcher>().dominoTile.rightValue)
            {
                // Sol değer eşleşmesi
                obj.GetComponent<TileMoveController>().MoveToLocationAndRotateLeft(new Vector3(lastTile.transform.position.x + xValueForPlace, lastTile.transform.position.y - yValueForPlace, lastTile.transform.position.z));
                obj.GetComponent<TileCatcher>().dominoTile.rightValue = -1;

            }
            else if (lastTile.GetComponent<TileCatcher>().dominoTile.rightValue == obj.GetComponent<TileCatcher>().dominoTile.leftValue)
            {
                obj.GetComponent<TileMoveController>().MoveToLocationAndRotateRight(new Vector3(lastTile.transform.position.x + xValueForPlace, lastTile.transform.position.y - yValueForPlace, lastTile.transform.position.z));
                obj.GetComponent<TileCatcher>().dominoTile.leftValue = -1;
            }
            else if (lastTile.GetComponent<TileCatcher>().dominoTile.rightValue == obj.GetComponent<TileCatcher>().dominoTile.rightValue)
            {
                obj.GetComponent<TileMoveController>().MoveToLocationAndRotateLeft(new Vector3(lastTile.transform.position.x + xValueForPlace, lastTile.transform.position.y - yValueForPlace, lastTile.transform.position.z));
                obj.GetComponent<TileCatcher>().dominoTile.rightValue = -1;
            }
        }

        //taşları yeniden yerleştirme
        public void replaceTiles(Player p)
        {
            float coordinateX;
            float coordinateY;
            if (p.isAi())
            {
                coordinateX = -1.82f;
                coordinateY = 4.7f;
            }
            else
            {
                coordinateX = -1.82f;
                coordinateY = -3.87f;
            }
            foreach (GameObject tile in p.GetHand())
            {
                tile.transform.position = new Vector3(coordinateX, coordinateY, 0f);
                tile.transform.localScale = new Vector3(5f, 5f, 1f);
                tile.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
                //tile.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                coordinateX += 0.60f;
            }

        }

        //Taşlar arasındaki boşluğun ölçek değerine göre x değeri belirlenmesi
        public float getXvalue(GameObject target, GameObject obj,string direction)
        {
            float x = 0f;
            float initialScale = 5f;
            float scaleFactor = scaleValue / initialScale;
            if (direction.Equals("out"))
            {
                if (target.GetComponent<TileCatcher>().dominoTile.IsDouble())
                {
                    if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        x= 0.36f;
                    }
                    else
                    {
                        x= 0.48f;
                    }
                }
                else
                {
                    if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        x= 0.48f;
                    }
                    else
                    {
                        x= 0.63f;
                    }
                }
            }
            else if (direction.Equals("in"))
            {
                if (target.GetComponent<TileCatcher>().dominoTile.IsDouble())
                {
                    if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        x = -0.36f;
                    }
                    else
                    {
                        x = -0.48f;
                    }
                }
                else
                {
                    if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        x = -0.48f;
                    }
                    else
                    {
                        x = -0.63f;
                    }
                }
            }
            else if(direction.Equals("reverse"))
            {
                if (target.GetComponent<TileCatcher>().dominoTile.IsDouble())
                {
                    if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        x = -0.36f;
                    }
                    else
                    {
                        x = -0.51f;
                    }
                }
                else
                {
                    if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        x = -0.51f;
                    }
                    else
                    {
                        x = -0.36f;
                    }
                }
            }
            return x;
        }
        //Taşlar arasındaki boşluğun ölçek değerine göre y değeri belirlenmesi
        public float getYvalue(GameObject target,GameObject obj,string direction)
        {
            float y=0f;
            float initialScale = 5f;
            float scaleFactor = scaleValue / initialScale;

            if (direction.Equals("out"))
            {
                if (target.GetComponent<TileCatcher>().dominoTile.IsDouble())
                {
                    if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        y = 0.36f;
                    }
                    else
                    {
                        y = 0.51f;
                    }
                }
                else
                {
                    if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        y = 0.51f;
                    }
                    else
                    {
                        y = 0.36f;
                    }
                }
            }
            else if (direction.Equals("in"))
            {
                if (target.GetComponent<TileCatcher>().dominoTile.IsDouble())
                {
                    if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        y = -0.36f;
                    }
                    else
                    {
                        y = -0.51f;
                    }
                }
                else
                {
                    if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        y = -0.51f;
                    }
                    else
                    {
                        y = -0.36f;
                    }
                }
            }

            else if (direction.Equals("reverse"))
            {
                if (target.GetComponent<TileCatcher>().dominoTile.IsDouble())
                {
                    if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        y = 0.36f;
                    }
                    else
                    {
                        y = 0.48f;
                    }
                }
                else
                {
                    if (obj.GetComponent<TileCatcher>().dominoTile.IsDouble())
                    {
                        y = 0.48f;
                    }
                    else
                    {
                        y = 0.63f;
                    }
                }
            }
            //sclae değerinin 1 olduğunda hizalama değerleri
            return y;
        }

        //oyuncunun taşlarının tıklanabilirliğini değiştirme
        public void changeClickable(bool isclickable)
        {
            if (manager.gameBoard.transform.childCount<=0)
            {
                return;
            }
            foreach (var item in player1.GetHand())
            {
                item.GetComponent<BoxCollider2D>().enabled = isclickable;
            }
        }
    }
}
