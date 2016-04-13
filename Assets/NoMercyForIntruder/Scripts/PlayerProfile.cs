using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NoMercyForIntruder.Game
{
    internal enum ResourceType
    {
        Wood,
        Food,
        Oil,
        Steel
    }
}

namespace NoMercyForIntruder.Game.Controller {

    public class PlayerProfile : MonoBehaviour
    {

        [Tooltip("四项资源分别是：木头、食物、石油和钢铁")]
        [SerializeField] int[] initResource = new int[4];
        [SerializeField] Text resourceShow;

        private int[] m_curResource = new int[4];

        // Use this for initialization
        void Start()
        {
            Messenger.AddListener<int, ResourceType>("Gain Resource", GainResource);
            for (int i = 0; i < 4; i++)
            {
                m_curResource[i] = initResource[i];
            }
            resourceShow.text = string.Format("木材：{0:D2}\n食物：{1:D2}\n石油：{2:D2}\n钢铁：{3:D2}", m_curResource[0], m_curResource[1], m_curResource[2], m_curResource[3]);
        }

        public bool PayForBuilding(int[] requires)              // Pay the cost after checking
        {
            for (int i = 0; i < 8; i++)
            {
                if (i < 4)
                {
                    if (m_curResource[i] < requires[i]) return false;
                }
                else
                {
                    m_curResource[i - 4] -= requires[i - 4];
                }
            }
            resourceShow.text = string.Format("木材：{0:D2}\n食物：{1:D2}\n石油：{2:D2}\n钢铁：{3:D2}", m_curResource[0], m_curResource[1], m_curResource[2], m_curResource[3]);
            return true;
        }

        private void GainResource(int amount, ResourceType type)
        {
            m_curResource[(int)type] += amount;
            resourceShow.text = string.Format("木材：{0:D2}\n食物：{1:D2}\n石油：{2:D2}\n钢铁：{3:D2}", m_curResource[0], m_curResource[1], m_curResource[2], m_curResource[3]);
        }
    }
}
