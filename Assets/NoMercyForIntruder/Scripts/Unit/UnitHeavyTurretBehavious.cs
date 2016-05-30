using UnityEngine;
using System.Collections;

namespace NoMercyForIntruder.Game.Unit
{

    public class UnitHeavyTurretBehavious : MonoBehaviour
    {

        private enum TurretState
        {
            idle,
            aiming,
            attacking,
            reloading
        }

        [SerializeField] private GameObject projectile;
        [SerializeField] private Transform[] projectilePos;
        [SerializeField] private int shotPerBarrel;
        [SerializeField] private Transform aimer;
        [SerializeField] private float muzzleSpeed;
        [SerializeField] private float rotateSpeed;
        [SerializeField] private float range;
        [SerializeField] private float fireInterval;
        [SerializeField] private int magazineSize;
        [SerializeField] private float reloadTime;
        [SerializeField] private int magazineCount;
        [SerializeField] private Vector3 rotationModifier = Vector3.zero;

        private TurretState m_curTurretState;
        private bool m_canFire;
        private float m_timer;
        private Transform m_target;
        private int m_rotateDir;
        private int m_curMagAmmo;
        private int m_curMagCount;
        private int m_curBarrelIndex;
        private int m_curBarrelShot;
        private Quaternion m_prevRotation;

        public float GetRange { get { return range; } }

        // Use this for initialization
        void Start()
        {
            m_curTurretState = TurretState.idle;
            m_timer = 0f;
            m_curMagAmmo = magazineSize;
            m_curMagCount = magazineCount - 1;
            m_curBarrelIndex = 0;
            m_curBarrelShot = shotPerBarrel;
            if (m_curMagCount < 0) m_curMagCount = -1;
            m_canFire = true;
            m_prevRotation = transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {
            FSM();
        }

        void FSM()
        {
            switch(m_curTurretState)
            {
                case TurretState.idle:
                    break;
                case TurretState.aiming:
                    aimer.LookAt(m_target);
                    float targetY = aimer.localEulerAngles.y;
                    if (Mathf.Abs(transform.localEulerAngles.z - targetY) > 5f)
                    {
                        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime * m_rotateDir);
                    }
                    else
                    {
                        transform.localEulerAngles = new Vector3(0, 0, targetY);
                        m_curTurretState = TurretState.attacking;
                    }
                    break;
                case TurretState.attacking:
                    if (!m_target)
                    {
                        m_curTurretState = TurretState.idle;
                        return;
                    }
                    if (m_canFire)
                    {
                        GameObject go = Instantiate(projectile, projectilePos[m_curBarrelIndex].position, projectilePos[m_curBarrelIndex].rotation) as GameObject;
                        Rigidbody body = go.GetComponent<Rigidbody>();
                        body.velocity = go.transform.forward * muzzleSpeed;
                        m_canFire = false;
                        m_curBarrelShot--;
                        if (m_curBarrelShot == 0)
                        {
                            m_curBarrelIndex++;
                            m_curBarrelShot = shotPerBarrel;
                            if (m_curBarrelIndex >= projectilePos.Length) m_curBarrelIndex = 0;
                        }
                        if (m_curMagAmmo > 0)
                        {
                            m_curMagAmmo--;
                            if (m_curMagAmmo == 0)
                            {
                                m_curTurretState = TurretState.reloading;
                                m_timer = 0f;
                            }
                        }
                    }
                    else
                    {
                        if (m_timer < fireInterval)
                        {
                            m_timer += Time.deltaTime;
                            // Adjust shoot angle
                            aimer.LookAt(m_target);
                            float targetPosY = aimer.localEulerAngles.y;
                            if (Mathf.Abs(transform.localEulerAngles.z - targetPosY) > 5f)
                            {
                                transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime * m_rotateDir);
                            }
                            else
                            {
                                transform.localEulerAngles = new Vector3(0, 0, targetPosY);
                                m_curTurretState = TurretState.attacking;
                            }
                        }
                        else
                        {
                            m_timer = 0f;
                            m_canFire = true;
                        }
                    }
                    break;
                case TurretState.reloading:
                    if (m_timer < reloadTime)
                    {
                        m_timer += Time.deltaTime;
                    }
                    else
                    {
                        m_timer = 0f;
                        if (m_curMagCount > 0)
                        {
                            m_curMagCount--;
                            m_curMagAmmo = magazineSize;
                            m_curTurretState = TurretState.aiming;
                            m_canFire = true;
                            m_curBarrelIndex = 0;
                            m_curBarrelShot = shotPerBarrel;
                        }
                        else if (m_curMagCount == -1)
                        {
                            m_curMagAmmo = magazineSize;
                            m_curTurretState = TurretState.aiming;
                            m_canFire = true;
                            m_curBarrelIndex = 0;
                            m_curBarrelShot = shotPerBarrel;
                        }
                        else
                        {
                            m_curTurretState = TurretState.idle;
                        }
                    }
                    break;
            }
        }

        public void SetTarget(Transform target)
        {
            m_target = target;
            aimer.LookAt(target);
            aimer.rotation *= Quaternion.Euler(rotationModifier);
            float _axisYDeg = aimer.localEulerAngles.y;
            float yDegDiff = transform.localEulerAngles.z - _axisYDeg;
            if (yDegDiff > -360 && yDegDiff <= -180) m_rotateDir = -1;
            else if (yDegDiff > -180 && yDegDiff <= 0) m_rotateDir = 1;
            else if (yDegDiff > 0 && yDegDiff <= 180) m_rotateDir = -1;
            else m_rotateDir = 1;
            if (m_curTurretState == TurretState.reloading) return;
            m_curTurretState = TurretState.aiming;
        }

        public void Stop()
        {
            m_target = null;
            if (m_curTurretState == TurretState.reloading) return;
            m_curTurretState = TurretState.idle;
        }
    }
}
