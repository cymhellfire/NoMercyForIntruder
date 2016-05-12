using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NoMercyForIntruder.Game.Unit;

namespace NoMercyForIntruder.Game.Building
{

    public class DefenceBuildingBehaviour : MonoBehaviour
    {

        private enum BuildingState
        {
            idle,
            attacking,
            switching
        }

        [SerializeField] private float attackRange;

        private SphereCollider m_detector;
        private List<GameObject> m_targetList;
        private GameObject m_curTarget;
        private BuildingState m_curState;
        private UnitTurretBehavious m_turret;

        // Use this for initialization
        void Start()
        {
            m_detector = gameObject.AddComponent<SphereCollider>();
            m_detector.isTrigger = true;
            m_detector.radius = attackRange;
            m_targetList = new List<GameObject>();
            m_curTarget = null;
            m_curState = BuildingState.idle;
            m_turret = GetComponentInChildren<UnitTurretBehavious>();
        }

        // Update is called once per frame
        void Update()
        {
            FSM();
        }

        void FSM()
        {
            switch(m_curState)
            {
                case BuildingState.idle:

                    break;
                case BuildingState.attacking:
                    
                    break;
                case BuildingState.switching:
                    if (m_targetList.Count > 0)
                    {
                        m_curTarget = m_targetList[0];
                        m_curState = BuildingState.attacking;
                        m_turret.SetTarget(m_curTarget.transform);
                    }
                    else
                    {
                        m_turret.Stop();
                        m_curState = BuildingState.idle;
                    }
                    break;
            }
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.tag == "Unit")
            {
                if (!m_targetList.Contains(col.gameObject))
                {
                    m_targetList.Add(col.gameObject);
                    if (m_curTarget == null)
                    {
                        m_curTarget = col.gameObject;
                    }
                    m_curState = BuildingState.switching;
                }
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.tag == "Unit")
            {
                if (m_targetList.Contains(col.gameObject))
                {
                    m_targetList.Remove(col.gameObject);
                    m_curState = BuildingState.switching;
                }
                if (m_curTarget == col.gameObject)
                {
                    m_curTarget = null;
                }
            }
        }

    }
}
