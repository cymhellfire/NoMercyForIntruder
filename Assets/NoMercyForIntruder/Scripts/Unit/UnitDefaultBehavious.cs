using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding;

namespace NoMercyForIntruder.Game.Unit
{
    internal enum UnitState
    {
        idle,
        moving,
        turning,
        attacking
    }

    [RequireComponent(typeof(Seeker))]
    [RequireComponent(typeof(CharacterController))]
    public class UnitDefaultBehavious : MonoBehaviour
    {

        [SerializeField] private int maxHP;
        [SerializeField] private float speed;
        [SerializeField] private float rotateSpeed;
        [SerializeField] private float nextWaypointDistance;
        [SerializeField] private Text stateShow;
        [SerializeField] private GameObject aimer;

        private int m_curHP;
        private UnitState m_curState;
        private Vector3 m_targetPos;
        private Path m_path;
        private int m_curWaypointIndex;
        private Seeker m_seeker;
        private CharacterController m_charCtrl;

        // Use this for initialization
        void Start()
        {
            m_curState = UnitState.idle;
            m_seeker = GetComponent<Seeker>();
            m_charCtrl = GetComponent<CharacterController>();
            stateShow.text = transform.rotation.ToString();
        }

        // Update is called once per frame
        void Update()
        {
            FSM();
            stateShow.text = m_curState.ToString();
        }

        void OnPathComplete(Path path)
        {
            if (!path.error)
            {
                m_path = path;
                m_curWaypointIndex = 0;
                m_curState = UnitState.moving;
            }
        }

        void FSM()
        {
            switch(m_curState)
            {
                case UnitState.idle:
                    if (Input.GetMouseButton(0))
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 200f))
                        {
                            Debug.Log("Hit " + hit.collider.name);
                            if (hit.collider.name == "Terrain")
                            {
                                m_targetPos = hit.point;
                                m_seeker.StartPath(transform.position, m_targetPos, OnPathComplete);
                            }
                        }
                    }
                    break;
                case UnitState.moving:
                    if (m_path == null)
                    {
                        m_curState = UnitState.idle;
                        return;
                    }
                    if (m_curWaypointIndex >= m_path.vectorPath.Count)
                    {
                        Debug.Log("Final point reach");
                        m_curState = UnitState.idle;
                        return;
                    }

                    Vector3 dir = (m_path.vectorPath[m_curWaypointIndex] - transform.position).normalized;
                    dir *= speed;
                    m_charCtrl.SimpleMove(dir);

                    if (Vector3.Distance(m_path.vectorPath[m_curWaypointIndex], transform.position) < nextWaypointDistance)
                    {
                        m_curWaypointIndex++;
                        if (m_curWaypointIndex < m_path.vectorPath.Count)
                            m_curState = UnitState.turning;
                    }
                    break;
                case UnitState.turning:
                    if (Rotate(m_path.vectorPath[m_curWaypointIndex]))
                    {
                        m_curState = UnitState.moving;
                    }
                    break;
                case UnitState.attacking:
                    break;
            }
        }

        /// <summary>
        /// 朝向指定地点，通过返回值判断是否旋转完成
        /// </summary>
        /// <param name="targetPos">目标位置</param>
        /// <returns>是否已经面向目标</returns>
        bool Rotate(Vector3 targetPos)
        {
            aimer.transform.LookAt(m_path.vectorPath[m_curWaypointIndex]);
            Quaternion targetRot = aimer.transform.rotation;
            if (Quaternion.Angle(transform.rotation, targetRot) > 5f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
                return false;
            }
            else
            {
                transform.rotation = targetRot;
                return true;
            }
        }
    }
}
