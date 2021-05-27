using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GoalScript : MonoBehaviour
{

    public int openFrame = 0;
    public enum E_ParticleColor
    {
        GOLD,
        SILVER,
        BRONZE
    }

    private bool startFlg = false;
    private bool isPlayEffect = true;

    enum TURN{
        GEAR_TIME,
        DOOR_TIME,
    };

    List<GameObject> particleList = new List<GameObject>();
    
    public ParticleSystem rayParticle;
    GameObject Ldoor;
    GameObject Rdoor;
    GameObject Gear;
    int playFrame = 0;
    int particleFrame = 0;
    public int initParticleFrame = 0;
    TURN turn;
    E_ParticleColor color;
    [SerializeField] private ParticleSystem.MinMaxGradient goldColor;
    [SerializeField] private ParticleSystem.MinMaxGradient silverColor;
    [SerializeField] private ParticleSystem.MinMaxGradient bronzeColor;
    [SerializeField] private ParticleSystem particle;
    // Color color;
    // Start is called before the first frame update
    void Start()
    {
        playFrame = 0;
        particleFrame = initParticleFrame;
        turn = TURN.GEAR_TIME;
        Rdoor = transform.Find("door_R/door_R").gameObject;
        Ldoor = transform.Find("door_L/door_L").gameObject;
        Gear  = transform.Find("door_L/door_L/Gear/Gear").gameObject;

        //Assign the color to the particle
        ParticleSystem.MainModule main = particle.main;
        main.startColor = bronzeColor;

        startFlg = false;
    }

    // Update is called once per frame
    void Update()
    {
        //ê∂ê¨ä‘äu
        particleFrame--;
        Vector3 pos = Gear.transform.position;
        if (particleFrame <= 0 && isPlayEffect)
        {
            Debug.Log("on");
            particleFrame = initParticleFrame;
            particleList.Add(Instantiate(rayParticle, pos, this.transform.rotation).gameObject);
        }

        for (int i = particleList.Count-1; i >= 0; i--)
        {
            if (particleList[i].GetComponent<ParticleSystem>().isStopped)
            {
                Destroy(particleList[i].gameObject);
                particleList.RemoveAt(i);
            }
        }
        
        if (startFlg)
        {
            if (playFrame < openFrame)
            {
                playFrame++;
                if (turn == TURN.GEAR_TIME)
                {
                    if (playFrame < (openFrame-60))
                    {
                        Gear.transform.Rotate(new Vector3(0, 0, 1f));
                    }
                    if (playFrame >= openFrame)
                    {
                        playFrame = 0;
                        turn = TURN.DOOR_TIME;
                    }
                }
                else if(turn == TURN.DOOR_TIME)
                {
                    Ldoor.transform.Rotate(new Vector3(0, -1f, 0));
                    Rdoor.transform.Rotate(new Vector3(0, 1f, 0));
                }
                            
            }
        }
        else
        {
            startFlg = false;
       //     transform.GetChild(1).gameObject.GetComponent<Transform>().transform.Rotate(new Vector3(0, 0, 0));
       //     transform.GetChild(2).gameObject.GetComponent<Transform>().transform.Rotate(new Vector3(0, 0, 0));
        }
       // GetComponent<GoalScript>().ChangeColor(E_ParticleColor.BRONZE);
    }

    public bool SetStartFlg(bool _flg)
    {
        return startFlg = _flg;
    }
    public bool SetPlayEffectFlg(bool _flg)
    {
        return isPlayEffect = _flg;
    }

    //EnumÇÃE_ParticleColorÇ©ÇÁïœçXÇ∑ÇÈêFÇà¯êîÇ…ì¸ÇÍÇÈ
    public void ChangeColor(E_ParticleColor _col)
    {
        ParticleSystem.MainModule main = particle.main;
        if (_col == E_ParticleColor.GOLD)
        {
            Debug.Log("GoldColor");
            main.startColor = goldColor;
        }
        else if (_col == E_ParticleColor.SILVER)
        {
            main.startColor = silverColor;
        }
        else
        {
            main.startColor = bronzeColor;
        }
    }

    public void ClearParticle()
    {
        for (int i = particleList.Count - 1; i >= 0; i--)
        {
            Destroy(particleList[i].gameObject);
            particleList.RemoveAt(i);
        }
    }
}
