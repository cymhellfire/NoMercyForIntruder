using UnityEngine;
using System.Collections;

namespace NoMercyForIntruder.Game.Controller
{

    internal enum GameState
    {
        Defence,
        Intrude,
        Observe
    }

    public class GameController : MonoBehaviour
    {



        private GameState m_curGameState;

        // Use this for initialization
        void Start()
        {
            m_curGameState = GameState.Observe;
        }

        // Update is called once per frame
        void Update()
        {

        }


    }
}
