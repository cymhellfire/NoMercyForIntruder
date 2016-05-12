using UnityEngine;
using System.Collections;

namespace NoMercyForIntruder.Game.Weapon
{

    public class ProjectileDefaultBehavious : MonoBehaviour
    {

        [SerializeField] private GameObject effect;
        [SerializeField] private float damage;
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
