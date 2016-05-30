using UnityEngine;
using System.Collections;

namespace NoMercyForIntruder.Game.Building
{

    public class GameFieldInitialization : MonoBehaviour
    {
        [Tooltip("建造地基")]
        [SerializeField] private GameObject buildingBasePrefab;
        [Tooltip("两个自然数来表示建造区域的长和宽")]
        [SerializeField] private int[] fieldSize = new int[2];
        [Tooltip("建造地基的间隔距离")]
        [SerializeField] private float distance;

        private Vector3 m_curPos;
        private bool m_fieldShowed;
        private BuildingBaseBehaviour[,] m_BuidingBases;

        public BuildingBaseBehaviour[,] GetBuildingBases() { return m_BuidingBases; }
        public int[] GetFieldSize() { return fieldSize; }

        // Use this for initialization
        void Start()
        {
            m_curPos = transform.position;
            m_BuidingBases = new BuildingBaseBehaviour[fieldSize[0], fieldSize[1]];
            m_fieldShowed = true;
            Initialization();
        }

        private void Initialization()
        {
            for (int i = 0; i < fieldSize[0]; i++)
            {
                for (int j = 0; j < fieldSize[1]; j++)
                {
                    GameObject go = Instantiate(buildingBasePrefab, m_curPos, buildingBasePrefab.transform.rotation) as GameObject;
                    m_BuidingBases[i, j] = go.GetComponent<BuildingBaseBehaviour>();
                    m_BuidingBases[i, j].SetPosition(i, j);
                    m_curPos.x += distance;
                }
                m_curPos.x = transform.position.x;
                m_curPos.z += distance;
            }
            HideBuildField();
        }

        public void ShowBuildField()
        {
            if (m_fieldShowed) return;
            for (int i = 0; i < fieldSize[0]; i++)
            {
                for (int j = 0; j < fieldSize[1]; j++)
                {
                    m_BuidingBases[i, j].gameObject.SetActive(true);
                }
            }
            m_fieldShowed = true;
        }

        public void HideBuildField()
        {
            if (!m_fieldShowed) return;
            for(int i = 0; i < fieldSize[0]; i++)
            {
                for (int j = 0; j < fieldSize[1]; j++)
                {
                    m_BuidingBases[i, j].gameObject.SetActive(false);
                }
            }
            m_fieldShowed = false;
        }
        
    }
}
