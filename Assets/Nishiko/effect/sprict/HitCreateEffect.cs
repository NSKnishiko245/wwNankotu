using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCreateEffect : MonoBehaviour
{
    public GameObject[] m_objParts;
    public ParticleSystem m_objExplosion;
    public AudioClip m_sound;

    public bool isFinished { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        isFinished = false;
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
            Bakuhatu(other.gameObject);
        }
    }
    
    public void Bakuhatu(GameObject player)
    {
        if (!isFinished)
        {
            AudioSource.PlayClipAtPoint(m_sound, player.transform.position);//SEçƒê∂
            Instantiate(m_objExplosion, player.transform.position, Quaternion.identity);
            for (int i = 0; i < m_objParts.Length; i++)
            {
                Instantiate(m_objParts[i], player.transform.position, Quaternion.identity);
            }
            isFinished = true;
        }
    }
}
