using UnityEngine;
using System.Collections;

namespace NoMercyForIntruder.Game.Building
{

    public class ResourceBuildingBehaviour : MonoBehaviour
    {

        internal enum ProduceState
        {
            Producing,
            Full
        }

        [SerializeField] private float produceTime;
        [SerializeField] private int produceAmount;
        [SerializeField] private ResourceType resourceType;
        [SerializeField] private GameObject fullEffect;

        private float m_timer;
        private ProduceState m_curState;

        // Use this for initialization
        void Start()
        {
            m_curState = ProduceState.Producing;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_curState != ProduceState.Full)
            {
                m_timer += Time.deltaTime;
                if (m_timer >= produceTime)
                {
                    m_curState = ProduceState.Full;
                    m_timer = 0f;
                    fullEffect.SetActive(true);
                }
            }
        }

        public void GetResource()
        {
            if (m_curState == ProduceState.Full)
            {
                fullEffect.SetActive(false);
                m_curState = ProduceState.Producing;
                Messenger.Broadcast<int, ResourceType>("Gain Resource", produceAmount, resourceType);
            }
        }
    }
}
