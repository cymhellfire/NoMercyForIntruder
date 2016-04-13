using UnityEngine;
using System.Collections;

namespace NoMercyForIntruder.Game.Building
{

    public class BuildingBaseBehaviour : MonoBehaviour
    {

        private enum BuildingBaseState
        {
            available,
            forbidden
        }

        [SerializeField] private Material availableMaterial;
        [SerializeField] private Material forbiddenMaterial;

        private int[] m_position = new int[2];
        private BuildingBaseState m_curBuildingBaseState;
        private GameObject m_buiding;

        public bool IsAvailable() { return (m_curBuildingBaseState == BuildingBaseState.available); }
        public int GetPosition(int index) { return m_position[index]; }

        // Use this for initialization
        void Start()
        {
            m_curBuildingBaseState = BuildingBaseState.available;
        }

        public void SetPosition(int x, int y)
        {
            m_position[0] = x;
            m_position[1] = y;
        }

        public void GainBuilding(GameObject building)
        {
            m_buiding = building;
            if (m_curBuildingBaseState == BuildingBaseState.available)
            {
                SwitchState(BuildingBaseState.forbidden);
            }
        }

        public void LoseBuilding()
        {
            m_buiding = null;
            SwitchState(BuildingBaseState.available);
        }

        private void SwitchState(BuildingBaseState state)
        {
            Renderer render = GetComponent<Renderer>();
            switch (state)
            {
                case BuildingBaseState.available:
                    render.material = availableMaterial;
                    m_curBuildingBaseState = BuildingBaseState.available;
                    break;
                case BuildingBaseState.forbidden:
                    render.material = forbiddenMaterial;
                    m_curBuildingBaseState = BuildingBaseState.forbidden;
                    break;
            }
        }

    }

}
