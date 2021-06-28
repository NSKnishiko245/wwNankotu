using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    private bool isNoSilverGear = false;        //true:銀の歯車がない。false:銀の歯車がまだある
    enum TURN
    {
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

    public AudioClip sound1;
    AudioSource audioSource;

    public int silverLimit { get; private set; } = 0;
    public int nowRotateNum { get; private set; } = 0;

    int testNum = 0;
    bool testFlg = false;   //一度きり
    private Vector3 pPos;
    private Vector3 perticlePos;
    // Color color;
    // Start is called before the first frame update
    void Start()
    {
        playFrame = 0;
        particleFrame = initParticleFrame;
        turn = TURN.GEAR_TIME;
        Rdoor = transform.Find("door_R/door_R").gameObject;
        Ldoor = transform.Find("door_L/door_L").gameObject;
        Gear = transform.Find("door_L/door_L/Gear/Gear2").gameObject;
        perticlePos = transform.Find("door_huti/door 1").gameObject.transform.position;
        //Assign the color to the particle
        ParticleSystem.MainModule main = particle.main;
        main.startColor = bronzeColor;
        startFlg = false;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.Space) && !isNoSilverGear)
        //{
        //    //SetGearVibrateTime(60);
        //    testNum++;
        //    SetSilverState(5, testNum);
        //}

        if (!testFlg && (isNoSilverGear || Input.GetKeyDown(KeyCode.U)) && Gear.GetComponent<VibrateController>().vibrateTime <= 0)
        {

            GameObject preGear = transform.Find("door_L/door_L/Gear").gameObject;
            float z = this.transform.position.z;

            audioSource.PlayOneShot(sound1);
            preGear.transform.DORotate(Vector3.right * 700f, 2f, mode: RotateMode.WorldAxisAdd);
            preGear.transform.DOJump(new Vector3(this.transform.position.x, -14f, this.transform.position.z * 75f), jumpPower: 4f, numJumps: 2, duration: 10f).OnComplete(() =>
            {
                Gear.SetActive(false);
            });

            testFlg = true;
        }
        //生成間隔
        particleFrame--;
        if (particleFrame <= 0 && isPlayEffect)
        {
            particleFrame = initParticleFrame;
            if (startFlg)
            {
                particleList.Add(Instantiate(rayParticle, pPos, this.transform.rotation).gameObject);
            }
            else
            {
                perticlePos = transform.Find("door_huti/door 1").gameObject.transform.position;
                particleList.Add(Instantiate(rayParticle, perticlePos, this.transform.rotation).gameObject);
            }
        }

        for (int i = particleList.Count - 1; i >= 0; i--)
        {
            if (particleList[i].GetComponent<ParticleSystem>().isStopped)
            {
                Destroy(particleList[i].gameObject);
                particleList.RemoveAt(i);
            }
        }


        //ゴール演出
        if (startFlg)
        {

            if (playFrame < openFrame)
            {
                playFrame++;
                if (turn == TURN.GEAR_TIME)
                {
                    if (playFrame < (openFrame - 60))
                    {
                        Gear.transform.Rotate(new Vector3(0, 0, 1f));
                    }
                    if (playFrame >= openFrame)
                    {
                        playFrame = 0;
                        turn = TURN.DOOR_TIME;
                    }
                }
                else if (turn == TURN.DOOR_TIME)
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
        if (!startFlg) pPos = Gear.transform.position;
        return startFlg = _flg;
    }
    public bool SetPlayEffectFlg(bool _flg)
    {
        return isPlayEffect = _flg;
    }

    public bool SetIsNoSilverGear(bool _flg)
    {
        return isNoSilverGear = _flg;
    }

    public GameObject GetGear()
    {
        return Gear;
    }

    public void SetGearVibrateTime(float _time)
    {
        Gear.GetComponent<VibrateController>().SetVibrateTime(_time);
    }

    //EnumのE_ParticleColorから変更する色を引数に入れる
    public void ChangeColor(E_ParticleColor _col)
    {
        ParticleSystem.MainModule main = particle.main;
        if (_col == E_ParticleColor.GOLD)
        {
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

    /*
     * 銀の歯車を持っているかどうか
     * true：持っていない
     * false：持っている
     */
    public bool GetIsNoSilverGear()
    {
        return isNoSilverGear;
    }

    /*
     * 折った回数に応じて歯車の揺れの大きさ、揺れる時間をセット
     * 回数上限を超えるとisNoSilverGerをtrueにする
     * 
     *  _limit    ：銀の歯車の所得条件の折れる回数
     *  _nowRotate：現在の折った回数
     * 
     */
    public void SetSilverState(int _limit, int _nowRotate)
    {
        // Gear.GetComponent<VibrateController>().ResetPosition();
        silverLimit = _limit;
        nowRotateNum = _nowRotate;
        if (silverLimit < nowRotateNum)
        {

            isNoSilverGear = true;
        }
        else
        {
            float token, per, speed;
            int time;
            per = (_nowRotate / (_limit + 1)) + 1;
            token = per * 40;
            time = (int)per * 20;
            speed = per * 800;
            Gear.GetComponent<VibrateController>().SetVibrateRange(token);
            Gear.GetComponent<VibrateController>().SetSpeed(speed);

            Gear.GetComponent<VibrateController>().SetVibrateTime(time);
        }
    }
}
