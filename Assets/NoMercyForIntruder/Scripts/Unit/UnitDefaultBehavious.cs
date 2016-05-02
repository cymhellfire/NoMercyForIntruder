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
        private int m_rotateDir;
        private Seeker m_seeker;
        private UnitTurretBehavious m_turret;
        private Transform m_target;

        // Use this for initialization
        void Start()
        {
            m_curState = UnitState.idle;
            m_seeker = GetComponent<Seeker>();
            stateShow.text = transform.rotation.ToString();
            m_turret = GetComponentInChildren<UnitTurretBehavious>();
            m_target = null;
        }

        // Update is called once per frame
        void Update()
        {
            CheckInput();
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

        void CheckInput()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 200f))
                {
                    Debug.Log("Hit " + hit.collider.name);
                    if (hit.collider.name == "Terrain")
                    {
                        m_target = null;
                        m_targetPos = hit.point;
                        m_seeker.StartPath(transform.position, m_targetPos, OnPathComplete);
                        m_turret.Stop();
                    }
                    else if (hit.collider.tag == "Building")
                    {
                        m_target = hit.collider.transform;
                        if (Vector3.Distance(transform.position, hit.collider.transform.position) > m_turret.GetRange)
                        {
                            m_targetPos = hit.collider.transform.position;
                            m_seeker.StartPath(transform.position, m_targetPos, OnPathComplete);
                            m_turret.Stop();
                        }
                        else
                        {
                            m_turret.SetTarget(hit.collider.transform);
                            m_curState = UnitState.attacking;
                        }
                    }
                }
            }
        }

        void FSM()
        {
            switch(m_curState)
            {
                case UnitState.idle:
                    // No action
                    break;
                case UnitState.moving:
                    if (m_target != null)
                    {
                        if (Vector3.Distance(transform.position, m_target.position) <= m_turret.GetRange)
                        {
                            m_turret.SetTarget(m_target);
                            m_curState = UnitState.attacking;
                            return;
                        }
                    }
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
                    Move(dir);

                    if (Vector3.Distance(m_path.vectorPath[m_curWaypointIndex], transform.position) < nextWaypointDistance)
                    {
                        m_curWaypointIndex++;
                        if (m_curWaypointIndex < m_path.vectorPath.Count)
                        {
                            aimer.transform.LookAt(m_path.vectorPath[m_curWaypointIndex]);
                            float _axisYDeg = aimer.transform.eulerAngles.y;
                            float yDegDiff = transform.eulerAngles.y - _axisYDeg;
                            if (yDegDiff > -360 && yDegDiff <= -180) m_rotateDir = -1;
                            else if (yDegDiff > -180 && yDegDiff <= 0) m_rotateDir = 1;
                            else if (yDegDiff > 0 && yDegDiff <= 180) m_rotateDir = -1;
                            else m_rotateDir = 1;
                            m_curState = UnitState.turning;
                        }
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
                transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime * m_rotateDir);
                return false;
            }
            else
            {
                transform.rotation = targetRot;
                return true;
            }
        }

        /// <summary>
        /// 以指定速度移动
        /// </summary>
        /// <param name="speed">移动速度</param>
        void Move(Vector3 speed)
        {
            transform.position += speed * Time.deltaTime;
        }
    }
}
