using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCreateEffect : MonoBehaviour
{
    public GameObject[] m_objParts;
    public ParticleSystem m_objExplosion;
    public AudioClip m_sound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.tag == "Player")
    //    {
    //        Instantiate(m_objExplosion, collision.transform.position, Quaternion.identity);
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(m_sound, other.transform.position);//SEçƒê∂
            Instantiate(m_objExplosion, other.transform.position, Quaternion.identity);
            for(int i = 0; i < m_objParts.Length; i++)
            {
                Instantiate(m_objParts[i],other.transform.position, Quaternion.identity);
                Debug.Log("ê∂ê¨");
            }
        }
    }
}
