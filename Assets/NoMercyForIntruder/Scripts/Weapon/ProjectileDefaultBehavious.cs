using UnityEngine;
using System.Collections;
using NoMercyForIntruder.Game.Building;

namespace NoMercyForIntruder.Game.Weapon
{

    public class ProjectileDefaultBehavious : MonoBehaviour
    {

        [SerializeField] private GameObject effect;
        [SerializeField] private int damage;
        [SerializeField] private float lifeTime;

        private float m_timer;
        private Transform m_owner;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (m_timer < lifeTime)
            {
                m_timer += Time.deltaTime;
            }
            else
            {
                Death();
            }
        }

        void OnCollisionEnter(Collision col)
        {
            if (col.collider.tag == "Building")
            {
                BuildingDefaultBehaviour build = col.collider.transform.GetComponentInParent<BuildingDefaultBehaviour>();
                if (build)
                {
                    build.Damage(damage);
                }
            }
            Death();
        }

        void Death()
        {
            if (effect != null)
                Instantiate(effect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        public void SetOwner(Transform owner)
        {
            m_owner = owner;
        }
    }
}
