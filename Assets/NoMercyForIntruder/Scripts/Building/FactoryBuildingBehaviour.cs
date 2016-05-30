using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NoMercyForIntruder.Game.Controller;
using NoMercyForIntruder.Game.Unit;

namespace NoMercyForIntruder.Game.Building
{

    public class FactoryBuildingBehaviour : MonoBehaviour
    {

        internal enum TrainState
        {
            Training,
            Idle
        }

        [SerializeField] private Transform[] unitPorts;
        [SerializeField] private Transform[] heavyPorts;
        [SerializeField] private int maxCapacity;
        [Tooltip("占用空间超过该值的单位被判定为重型单位")]
        [SerializeField] private int heavyThreshold;

        private bool m_isMenuShowed;
        private PlayerProfile m_player;
        private int m_curCapacity;
        private int m_curUnitCount;
        private List<GameObject> m_ownUnits;
        private GameObject m_trainMenu;

        // Use this for initialization
        void Start()
        {
            m_player = GameObject.FindObjectOfType<PlayerProfile>();
            m_trainMenu = NoMercyForIntruder.Game.Controller.PlayerOperator.trainMenu;
            m_curCapacity = 0;
            m_curUnitCount = 0;
            m_isMenuShowed = false;
            m_ownUnits = new List<GameObject>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowTrainMenu()
        {
            if (!m_isMenuShowed)
            {
                m_trainMenu.SetActive(true);
                m_isMenuShowed = true;
            }
        }

        public void HideTrainMenu()
        {
            if (m_isMenuShowed)
            {
                m_trainMenu.SetActive(false);
                m_isMenuShowed = false;
            }
        }

        public void TrainUnit(GameObject tarUnit)
        {
            UnitDefaultBehavious unit = tarUnit.GetComponent<UnitDefaultBehavious>();
            if (m_curCapacity + unit.GetCapacityTook > maxCapacity) return;     // Check capacity for training
            if (m_player.PayForBuilding(unit.GetBuildCost))             // Check resource for training
            {
                GameObject go;
                if (unit.GetCapacityTook >= heavyThreshold)
                {
                    go = Instantiate(tarUnit, heavyPorts[m_curUnitCount].position, heavyPorts[m_curUnitCount].rotation) as GameObject;
                }
                else
                {
                    go = Instantiate(tarUnit, unitPorts[m_curUnitCount].position, unitPorts[m_curUnitCount].rotation) as GameObject;
                }
                m_ownUnits.Add(go);
            }
            m_curUnitCount++;
            m_curCapacity += unit.GetCapacityTook;
        }
    }
}
