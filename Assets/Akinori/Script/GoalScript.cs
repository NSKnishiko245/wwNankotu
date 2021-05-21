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

    enum TURN{
        GEAR_TIME,
        DOOR_TIME,
    };

    
    ParticleSystem rayParticle;
    GameObject Ldoor;
    GameObject Rdoor;
    GameObject Gear;
    int playFrame = 0;
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
        turn = TURN.GEAR_TIME;
        Rdoor = transform.Find("door_R/door_R").gameObject;
        Ldoor = transform.Find("door_L/door_L").gameObject;
        Gear  = transform.Find("door_L/door_L/Gear/Gear").gameObject;


        // goldColor = new ParticleSystem.MinMaxGradient();
        // goldColor.mode = ParticleSystemGradientMode.TwoColors;
        //goldColor.colorMin = Color.red;
        //goldColor.colorMax = Color.green;

        //Assign the color to the particle
        ParticleSystem.MainModule main = particle.main;
        main.startColor = bronzeColor;
        //Debug.Log(particle.name);
        startFlg = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            startFlg = true;
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
}
