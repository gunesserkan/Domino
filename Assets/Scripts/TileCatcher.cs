using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Assets.Scripts
{
    public class TileCatcher : MonoBehaviour
    {
        public DominoTile dominoTile;
        public void Initialization(DominoTile dominoTile) { 
        this.dominoTile = dominoTile;
        }
    }
}