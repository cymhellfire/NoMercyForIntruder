using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using NoMercyForIntruder.Game.Building;

namespace NoMercyForIntruder.Game.Controller
{

    internal enum OperationState
    {
        None,
        Add,
        Delete
    }

    public class PlayerOperator : MonoBehaviour
    {

        [SerializeField] private GameObject buildingPrefab;
        [SerializeField] private PlayerProfile m_player;

        private OperationState m_curOperationState;
        private BuildingDefaultBehaviour m_curSelectedBuilding;

        // Use this for initialization
        void Start()
        {
            m_curOperationState = OperationState.None;
            m_curSelectedBuilding = null;
        }

        // Update is called once per frame
        void Update()
        {
            FSM();
        }

        void FSM()          // Handle input in all status
        {
            switch(m_curOperationState)
            {
                case OperationState.None:
                    CheckInputForNone();
                    break;
                case OperationState.Add:
                    CheckInputForAdd();
                    break;
                case OperationState.Delete:
                    CheckInputForDelete();
                    break;
            }
        }

        void CheckInputForNone()
        {
            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Building")
                    {
                        BuildingDefaultBehaviour build = hit.collider.GetComponent<BuildingDefaultBehaviour>();
                        if (build)
                        {
                            if (m_curSelectedBuilding != null)
                            {
                                m_curSelectedBuilding.Select(false);
                            }
                            m_curSelectedBuilding = build;
                            build.Select(true);
                        }
                    }
                    else
                    {
                        if (m_curSelectedBuilding != null)
                        {
                            m_curSelectedBuilding.Select(false);
                            m_curSelectedBuilding = null;
                        }
                    }
                }
            }
        }

        void CheckInputForAdd()
        {
            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "BuildingBase")
                    {
                        BuildingBaseBehaviour buildBase = hit.collider.GetComponent<BuildingBaseBehaviour>();
                        if (buildBase.IsAvailable())
                        {
                            GameObject go = Instantiate(buildingPrefab, buildBase.transform.position, buildingPrefab.transform.rotation) as GameObject;
                            BuildingDefaultBehaviour building = go.GetComponent<BuildingDefaultBehaviour>();
                            if (building.CheckForBuild(buildBase.GetPosition(0), buildBase.GetPosition(1)) && m_player.PayForBuilding(building.GetCost))
                                building.TakeBuildingBase(buildBase.GetPosition(0), buildBase.GetPosition(1));
                            else
                                Destroy(go);
                        }
                    }
                }
            }
        }

        void CheckInputForDelete()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Building")
                    {
                        BuildingDefaultBehaviour building = hit.collider.GetComponent<BuildingDefaultBehaviour>();
                        building.Sell();
                    }
                }
            }
        }

        public void SwitchState(int index)
        {
            m_curOperationState = (OperationState)index;
        }
    }
}
