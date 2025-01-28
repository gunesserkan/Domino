using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using TMPro;
public class GameManager : MonoBehaviour
{

    //Karşı hamle için AI
    public AI playerAI;
    //ses dosyası ekleme
    public AudioSource source;

    public GameObject gameBoard;
    public Player player1; // Oyuncu
    public Player opponent; // Rakip oyuncu
    //puanları tutan değişkenler ve nesneler
    public int player1Points;
    public int player2Points;
    public int round;
    public TextMeshProUGUI txt_player_points;
    public TextMeshProUGUI txt_round;
    public TextMeshProUGUI txt_computer_points;
    public float tileSpacing = 0.1f; // Taşlar arasındaki mesafe
    public float tileScaleFactor = 0.5f; // Taşların boyutunu küçültme ölçeği

    public GameObject pnl_top_buttons;
    public GameObject pnl_quit_screen;
    public GameObject pnl_finish;
    public TextMeshProUGUI txt_points;
    public TextMeshProUGUI txt_winner;


    public GameController controller;
    public Sprite[] darkSprites;
    public Sprite[] lightSprites;
    public Sprite[] tileSprites; // Taşların sprite'larını içeren dizi
    private List<DominoTile> remainingTiles = new List<DominoTile>(); // Kullanılmamış taşları tutacak liste
    //dağıtılmayan taşların tutulacağı nesne
    public BoneYard yard;
    //bahçede kalan taşların bilgisini tutan text nesnesi
    public TextMeshProUGUI txt_information;
    //rectTransform kalan taşları gösteren text nesnesi için
    private RectTransform rectTransform;
    //hedeflenen genişlik
    public float targetWidth = 70f;
    //hedeflenen yükselik
    public float targetHeight = 40f;
    //hareketin gerçekleşme süresi
    public float duration = 1f;
    //taş çekmek için kullanılacak panel
    public GameObject pnl_take_tile;
    public void Awake()
    {
        //başlangıçta seçilen temayı uygulama
        string selectedTheme = GameStarter.themeKeeper; // Default değeri "dark" olarak ayarlandı
        Debug.Log(selectedTheme);
        if (selectedTheme.Equals("dark"))
        {
            tileSprites = darkSprites;
            Debug.Log("dark");
        }
        else if (selectedTheme.Equals("light"))
        {
            tileSprites = lightSprites;
            Debug.Log("light");
        }
        //controller = new GameController(player, opponent);
        // Taşların listesini oluşturma
        CreateTileList();
        Shuffle(remainingTiles);
        // Oyuncuları oluşturma ve taşları dağıtma
        player1 = new Player("Player1");
        opponent = new Player("Player2");

        opponent.setIsAi(true);

        controller.player1 = player1;
        controller.player2 = opponent;

        //DistributeTilesToPlayers(player);
        //DistributeTilesToPlayers(opponent);
        DistributeTiles(player1, -33f, -70f);
        DistributeTiles(opponent, -33f, 85f);
        playerAI.player = opponent;
        addTileToYard();
        player1Points = 0;
        player2Points = 0;
        round = 1;
        Color color = txt_information.color;
        color.a = 0f;
        txt_information.color = color;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MoveQuitPanel(-3);
        }
    }

    // Kullanılmamış taşları içeren listeyi oluşturma
    private void CreateTileList()
    {
        int i = 0;
        for (int ii = 0; ii <= 6; ii++)
            for (int iii = ii; iii <= 6; iii++)
            { 
                if (i < 28) {
                    DominoTile tile = new DominoTile(ii, iii, tileSprites[i]);
                    remainingTiles.Add(tile);
                i++;
                }
            }   
    }
    //Herhangi bir taş tıklamasını yakalama
    public void clickedTile(GameObject obj,GameObject target)
    {
        controller.PlaceTile(obj,target);
    }
    public static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n+1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    private void DistributeTiles(Player player, float coordinateX, float coordinateY)
    {
        Shuffle(remainingTiles);
        for (int i=0;i<7;i++)
        {
            int randomIndex = Random.Range(0, remainingTiles.Count);
            DominoTile dominoTile = remainingTiles[randomIndex];
            GameObject newTile;
            SpriteRenderer spriteRenderer;
            newTile = new GameObject(dominoTile.GetSprite().name);
            spriteRenderer = newTile.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = dominoTile.GetSprite();
            //objeye DominoTile özelliği ekleme
            newTile.AddComponent<TileCatcher>().Initialization(dominoTile);
            // TileDragHandler bileşeni ekleme (sürükleme yeteneği için)
            newTile.AddComponent<TileMoveController>();
            player.AddTileToHand(newTile);
            newTile.AddComponent<PlayerHandler>().player = player;
            // BoxCollider2D bileşeni ekleme (dokunulabilirlik için)
            if (!player.isAi())
            {
                BoxCollider2D boxCollider = newTile.AddComponent<BoxCollider2D>();
                boxCollider.size = spriteRenderer.bounds.size;
            }
            // Taşın pozisyonunu ayarlama
            newTile.transform.position = new Vector3(coordinateX, coordinateY, 0f);

            // Taşın ölçek ve dönüş ayarlarını yapma
            newTile.transform.localScale = new Vector3(5f, 5f, 0f);
            newTile.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            newTile.transform.SetParent(gameBoard.transform, false);

            // Yeni taş için koordinatları güncelleme
            coordinateX += 11f;
            remainingTiles.RemoveAt(randomIndex);
        }
    }

    //dağıtılmamış taşların bahçeye eklenmesi
    public void addTileToYard()
    {
        float positionX = 12f;
        float positionY = 0f;
        int count = remainingTiles.Count;
        for (int i=0;i<count;i++)
        {
            int randomIndex=Random.Range(0,remainingTiles.Count-1);
            GameObject obj=new GameObject(remainingTiles[randomIndex].GetSprite().name);
            SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = remainingTiles[randomIndex].GetSprite();
            obj.AddComponent<TileCatcher>().dominoTile = remainingTiles[randomIndex];
            obj.AddComponent<TileMoveController>();
            obj.AddComponent<PlayerHandler>();
            BoxCollider2D boxCollider = obj.AddComponent<BoxCollider2D>();
            boxCollider.size = spriteRenderer.bounds.size;
            obj.transform.SetParent(yard.transform, false);
            obj.transform.localScale = new Vector3(5f, 5f, 0f);
            obj.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            obj.transform.position = new Vector3(positionX, positionY, 0f);
            obj.GetComponent<SpriteRenderer>().sortingLayerName = "Front";
            yard.addToYard(obj, tileSprites[28]);
            remainingTiles.RemoveAt(randomIndex);
        }
    }

    //kullanılmayan taşların ekranda görünür olması
    public void isYardOpen(bool isIt)
    {
        if (isIt)
        {
            if (yard.tiles.Count == 0)
            {
                finishTheGame();
                return;
            }
            pnl_take_tile.transform.position = new Vector3(0.1f, 0f, 0f);
        }
        else
        {
            pnl_take_tile.transform.position = new Vector3(15f, 0f, 0f);
        }
    }

    //oyun bittiğinde çalışacak metod
    public void finishTheGame()
    {
        isYardOpen(false);
        int p1Points = 0,p2Points=0;
        foreach(var tile in controller.player1.GetHand()) {
            p1Points+=tile.GetComponent<TileCatcher>().dominoTile.GetTotalValue();
        }
        foreach(var tile in controller.player2.GetHand())
        {
            p2Points += tile.GetOrAddComponent<TileCatcher>().dominoTile.GetTotalValue();
        }
        if (p1Points < p2Points)
        {
            player1Points += p2Points;
            txt_points.text = " Puanınız: " + p2Points + "\n Bilgisayarın \n puanı: " + 0 + " ";
            txt_winner.text = "Kazandınız.";
        }
        else
        {
            player2Points += p1Points;
            txt_points.text = " Puanınız: " + 0 + "\n Bilgisayarın \n puanı: " + p1Points + " ";
            txt_winner.text = "Kaybettiniz.";
        }
        int childCount = gameBoard.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(gameBoard.transform.GetChild(i).gameObject);
        }
        MoveFinishPanel(0.1f);
        controller.countFirst = 0f;
        controller.countLast = 0f;
    }

    public void MoveFinishPanel(float x)
    {
        pnl_finish.transform.position = new Vector3(x, 0f, 0f);
        Color color = txt_information.color;
        color.a = 0f;
        txt_information.color = color;
    }
    public void replay(GameObject gameBoard)
    {
        controller.canPlay = false;
        controller.playingPlayer = null;
        remainingTiles.Clear();
        player1.GetHand().Clear();
        opponent.GetHand().Clear();
        playerAI.playableTilesToFist.Clear();
        playerAI.playableTilesToLast.Clear();
        controller.board.Clear();
        controller.countFirst = 0;
        controller.countLast = 0;
        //controller.tilePositions.Clear();
        yard.tiles.Clear();
        isYardOpen(false);
        CreateTileList();
        Shuffle(remainingTiles);
        int childCount = gameBoard.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Destroy(gameBoard.transform.GetChild(i).gameObject);
        }
        childCount = yard.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(yard.transform.GetChild(i).gameObject);
        }
        //DistributeTilesToPlayers(player);
        DistributeTiles(player1, -33f, -70f);
        //DistributeTilesToPlayers(opponent);
        DistributeTiles(opponent, -33f, 85f);
        addTileToYard();
        controller.firstStart();
        pnl_finish.transform.position = new Vector3(-10f, 0f, 0f);
        round++;
        txt_player_points.text = "Puanınız: " + player1Points;
        txt_computer_points.text = "Rakip: " + player2Points;
        txt_round.text = "Tur: " + round;
    }
    //taşların temizlenmesi ve yeniden dağıtılması
    public void restartGame(GameObject gameBoard)
    {
        controller.playingPlayer = null;
        controller.canPlay = false;
        remainingTiles.Clear();
        player1.GetHand().Clear();
        opponent.GetHand().Clear();
        controller.board.Clear();
        //controller.tilePositions.Clear();
        yard.tiles.Clear();
        isYardOpen(false);
        CreateTileList();
        Shuffle(remainingTiles);
        int childCount = gameBoard.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Destroy(gameBoard.transform.GetChild(i).gameObject);
        }
        childCount = yard.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(yard.transform.GetChild(i).gameObject);
        }
        //DistributeTilesToPlayers(player);
        DistributeTiles(player1, -33f, -70f);
        //DistributeTilesToPlayers(opponent);
        DistributeTiles(opponent, -33f, 85f);
        addTileToYard();
        controller.firstStart();
        pnl_finish.transform.position= new Vector3(-10f, 0f, 0f);
        player1Points = 0;
        player2Points = 0;
        round = 1;
        txt_player_points.text = "Puanınız: " + player1Points;
        txt_computer_points.text = "Rakip: " + player2Points;
        txt_round.text = "Tur: " + round;
        
    }

    //Çıkış butonuna tıklandığında giriş sahnesine dönme
    public void exitToStart() {
        SceneManager.LoadScene("StartScene");
    }

    //üst taraftaki buton ve çıkış paneli ayarları
    public float moveDuration = 0.5f;
    public float targetYPosition = 0.0f;

    public void MoveQuitPanel(float targetYPosition)
    {
        this.targetYPosition = targetYPosition;
        StartCoroutine(MovePanelCoroutine());
    }

    private IEnumerator MovePanelCoroutine()
    {
        Vector3 startPosition = pnl_quit_screen.transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, targetYPosition, startPosition.z);

        float startTime = Time.time;
        while (Time.time < startTime + moveDuration)
        {
            float t = (Time.time - startTime) / moveDuration;
            pnl_quit_screen.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        pnl_quit_screen.transform.position = targetPosition;
    }
    public void showYardInformation()
    {
        rectTransform = txt_information.GetComponent<RectTransform>();
        txt_information.text = "Bakçede kalan taş: " + yard.tiles.Count;
        StartCoroutine(FadeInTextMeshPro());
    }
    private IEnumerator FadeInTextMeshPro()
    {
        float elapsedTime = 0f;
        Color color = txt_information.color;

        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            txt_information.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        txt_information.color = color; // Görünür olma işlemi tamamlandığında hedef şeffaflığı ayarla
    }
    public void takeTileFromYard() {
        yard.tiles[yard.tiles.Count - 1].GetComponent<TileMoveController>().checkTile();
    }
}
