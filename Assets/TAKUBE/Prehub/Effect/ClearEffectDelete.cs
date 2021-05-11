using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearEffectDelete : MonoBehaviour
{
    public GameObject m_objPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_objPlayer.GetComponent<Player>().IsHitGoalBlock)
        {
            this.gameObject.SetActive(false);
        }
    }
}
