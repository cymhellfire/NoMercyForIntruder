using UnityEngine;
using System.Collections;

namespace NoMercyForIntruder.Game.Building
{

    public class BuildingDefaultBehaviour : MonoBehaviour
    {

        internal enum BuildingType
        {
            Command,
            Resource,
            Factory,
            Defence,
            None
        }

        private enum BuildingState
        {
            idle,
            setup
        }

        [SerializeField] private GameObject selectedEffect;
        [SerializeField] private int[] buildingSize = new int[2];
        [Tooltip("四项资源分别是：木头、食物、石油和钢铁")]
        [SerializeField] private int[] buildCost = new int[4];
        [SerializeField] private BuildingType buildingType = BuildingType.None;
        [SerializeField] private int maxHealth;

        private bool m_selected;
        private BuildingState m_curBuildingState;
        private int m_useBaseCount = 0;
        private BuildingBaseBehaviour[] m_takenBuildBases;
        private GameFieldInitialization m_gameField;
        private int m_curHealth;

        public int[] GetCost { get { return buildCost; } }

        // Use this for initialization
        void Start()
        {
            m_selected = false;
            m_curHealth = maxHealth;
            m_curBuildingState = BuildingState.setup;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Select(bool set)
        {
            if (set)
            {
                if (!m_selected)
                {
                    m_selected = true;
                    if (selectedEffect) selectedEffect.SetActive(true);
                    switch(buildingType)
                    {
                        case BuildingType.Resource:
                            ResourceBuildingBehaviour res = GetComponent<ResourceBuildingBehaviour>();
                            res.GetResource();
                            break;
                        case BuildingType.Factory:
                            FactoryBuildingBehaviour fac = GetComponent<FactoryBuildingBehaviour>();
                            fac.ShowTrainMenu();
                            break;
                    }
                }
            }
            else
            {
                if (m_selected)
                {
                    m_selected = false;
                    if (selectedEffect) selectedEffect.SetActive(false);
                    switch(buildingType)
                    {
                        case BuildingType.Factory:
                            FactoryBuildingBehaviour fac = GetComponent<FactoryBuildingBehaviour>();
                            fac.HideTrainMenu();
                            break;
                    }
                }
            }
        }

        public bool CheckForBuild(int originX, int originY)
        {
            if (m_gameField == null) m_gameField = FindObjectOfType<GameFieldInitialization>();
            int[] fieldSize = m_gameField.GetFieldSize();
            BuildingBaseBehaviour[,] bases = m_gameField.GetBuildingBases();
            for (int i = 0; i < buildingSize[0]; i++)
            {
                for (int j = 0; j < buildingSize[1]; j++)
                {
                    if (originX + i >= fieldSize[0] || originY + j >= fieldSize[1]) return false;
                    if (!bases[originX + i, originY + j].IsAvailable()) return false;
                }
            }
            return true;
        }

        public void TakeBuildingBase(int originX, int originY)
        {
            if (m_gameField == null) m_gameField = FindObjectOfType<GameFieldInitialization>();
            m_takenBuildBases = new BuildingBaseBehaviour[buildingSize[0] * buildingSize[1]];
            BuildingBaseBehaviour[,] bases = m_gameField.GetBuildingBases();
            for (int i = 0; i < buildingSize[0]; i++)
            {
                for (int j = 0; j < buildingSize[1]; j++)
                {
                    bases[originX + i, originY + j].GainBuilding(gameObject);
                    m_takenBuildBases[m_useBaseCount] = bases[originX + i, originY + j];
                    m_useBaseCount++;
                }
            }
            float offsetX = (float)(buildingSize[0] / 2.0 - 0.5);
            float offsetY = (float)(buildingSize[1] / 2.0 - 0.5);
            gameObject.transform.position += new Vector3(offsetX, 0, offsetY);
        }

        void ReleaseTakenBase()
        {
            for (int i = 0; i < m_useBaseCount; i++)
            {
                m_takenBuildBases[i].LoseBuilding();
            }
            m_useBaseCount = 0;
        }

        public void Sell()
        {
            ReleaseTakenBase();
            Destroy(gameObject);
        }

        public void Damage(int damage)
        {
            m_curHealth -= damage;
            if (m_curHealth <= 0)
            {
                BuildingDestroy();
            }
        }

        void BuildingDestroy()
        {
            ReleaseTakenBase();
            Destroy(gameObject);
        }
    }

}
