using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InGame : MonoBehaviour
{
    public static InGame Instance = null;

    [Header("플레이어")]
    [SerializeField]
    GameObject player;


    [Header("바이러스")]
    [SerializeField]
    GameObject[] ai;

    int aiCount = 3;
    public int aiNow = 1;
    
    public int shieldCount = 0;
    public float petSpeed = 3f;

    [Header("네모 바 적")]
    [SerializeField]
    GameObject push;
    [SerializeField]
    GameObject warn;

    int randomPushPos = 0;
    int pushDir = 0;
    float pushSpeed = 1f;

    float pushDelay = 3f;

    [Header("오브젝트 풀")]
    [SerializeField]
    GameObject pool;

    public bool isShield = false;
    public bool isSmall = false;
    bool isPush = false;
    public bool isRunning = false;
    public bool isStart = false;


    Coroutine SmallC;
    Coroutine ShieldC;
    Coroutine ScaleC;

    private void Awake()
    {
        Instance = this;
        if (Instance == null)
            Instance = FindObjectOfType<InGame>();
    }

    private void Update()
    {
        //상태
        if (isRunning)
        {
            if (GameManager.Instance.timeScore >= 40)
            {
                if (isPush) return;
                //푸쉬 에너미 생성 시작
                InvokeRepeating("PushPosition", 0f, pushDelay);
            }
            if(GameManager.Instance.timeScore==150)
            {
                if (aiNow == 2) return;
                aiNow = 2;
                ai[1].SetActive(true);
            }
            else if(GameManager.Instance.timeScore==500)
            {
                if (aiNow == 3) return;
                aiNow = 3;
                ai[2].SetActive(true);
            }
        }
        else
        {
            //아니면 중지
            CancelInvoke("PushPosition");
        }
    }

    public void StartInvoke()
    {
        float ranDelaySlow = Random.Range(18f, 40f);
        float ranDelayShield = Random.Range(30f, 55f);
        float ranDelayScale = Random.Range(18f, 45f);
        InvokeRepeating("SmallSpawn", ranDelaySlow, ranDelaySlow);
        InvokeRepeating("RandomScaleSpawn", ranDelayScale, ranDelayScale);
        InvokeRepeating("ShieldSpawn", ranDelayShield, ranDelayShield);
    }
    public void OvertInvoke()
    {
        CancelInvoke("SlowSpawn");
        CancelInvoke("ShieldSpawn");
        CancelInvoke("RandomScaleSpawn");
    }

    //제자리
    public void PlayerSetting()
    {
        DOTween.KillAll();

        //위치 제자리
        for(int i=0;i<aiCount;i++)
        {
        ai[i].transform.position = new Vector3(-9, -3.5f, 0);
            ai[i].SetActive(false);
        }
        ai[0].SetActive(true);
        player.transform.position = new Vector3(0, 0, 0);
        push.transform.position = new Vector3(17f, 0f, 0f);

        PlayerPrefs.SetFloat("PET_SPEED", 3f);
        petSpeed = 3f;
        warn.SetActive(false);
        shieldCount = 0;
        aiNow = 1;
        isPush = false;
        isShield = false;
        isStart = true;

        for(int i=2;i<aiCount;i++)
        {
            GameManager.Instance.EnemyOff(i);
        }

        //오브젝트 리턴
        for (int i = 0; i < pool.transform.childCount; i++)
        {
            ObjectPool.Instance.ReturnObject(PoolObjectType.ENEMY, pool.transform.GetChild(i).gameObject);
        }

    }

    //푸쉬 함수 및 경고
    public void PushPosition()
    {
        if (!isRunning || isPush) return;
        isPush = true;
        pushDir = Random.Range(0, 2);
        randomPushPos = Random.Range(4, -4);
        if (pushDir == 0)
        {
            push.transform.position = new Vector3(17f, randomPushPos, 0);
            warn.transform.localPosition = new Vector3(-1.7f, 0, 0);
        }
        else
        {
            push.transform.position = new Vector3(-17f, randomPushPos, 0);
            warn.transform.localPosition = new Vector3(1.7f, 0, 0);
        }
        warn.SetActive(true);
        StartCoroutine(WarnTime());
    }
    void WarnTimer()
    {

        warn.SetActive(false);
        if (!isRunning || !isPush) return;
        SoundManager.Instance.SFXPlay(6);
        push.transform.DOMoveX(push.transform.position.x * (-1), 2f)
            .OnComplete(() =>
            {
                isPush = false;
                if (pushSpeed >= 0.5)
                    pushSpeed -= 0.05f;
                pushDelay = Random.Range(10f, 30f);
                SoundManager.Instance.SFXStop();
                SoundManager.Instance.SFXPlay(2);
                StopCoroutine("WarnTime");
            });
    }
    IEnumerator WarnTime()
    {
        for (int i = 0; i < 3; i++)
        {
            SoundManager.Instance.SFXPlay(3);
            warn.SetActive(true);
            yield return new WaitForSeconds(pushSpeed);
            warn.SetActive(false);
            yield return new WaitForSeconds(pushSpeed);
        }
        WarnTimer();
    }

    //슬로우 아이템
    public void Small()
    {
        isSmall = true;
        for(int i=0;i<aiCount;i++)
        {
        ai[i].transform.DOScale(new Vector3(0.5f, 0.5f, 1f), 1f);
        }
        StartCoroutine(SmallTime());
    }
    IEnumerator SmallTime()
    {
        yield return new WaitForSeconds(5f);
        for(int i=0;i<aiCount;i++)
        {
        ai[i].transform.DOScale(new Vector3(1.3f, 1.3f, 1f), 1f);
        }
        isSmall = false;

    }
    void SmallSpawn()
    {
        SmallC = StartCoroutine(SmallOn());
    }
    IEnumerator SmallOn()
    {
        GameObject potion = ObjectPool.Instance.GetObject(PoolObjectType.SLOW);
        potion.transform.position = new Vector3(Random.Range(-10f, 10f), Random.Range(-4f, 4f), 0f);
        yield return new WaitForSeconds(12f);
        ObjectPool.Instance.ReturnObject(PoolObjectType.SLOW, potion);
        StopCoroutine(SmallC);
    }
    
    //쉴드 아이템
    void ShieldSpawn()
    {
        ShieldC = StartCoroutine(ShieldOn());
    }
    IEnumerator ShieldOn()
    {
        GameObject potion = ObjectPool.Instance.GetObject(PoolObjectType.SHIELD);
        potion.transform.position = new Vector3(Random.Range(-10f, 10f), Random.Range(-4f, 4f), 0f);
        yield return new WaitForSeconds(10f);
        ObjectPool.Instance.ReturnObject(PoolObjectType.SHIELD, potion);
        StopCoroutine(ShieldC);
    }
    
    //랜덤 크기 아이템
    void RandomScaleSpawn()
    {
        ScaleC = StartCoroutine(RandomScaleOn());
    }
    IEnumerator RandomScaleOn()
    {
        GameObject potion = ObjectPool.Instance.GetObject(PoolObjectType.SCALE);
        potion.transform.position = new Vector3(Random.Range(-10f, 10f), Random.Range(-4f, 4f), 0f);
        yield return new WaitForSeconds(10f);
        ObjectPool.Instance.ReturnObject(PoolObjectType.SHIELD, potion);
        StopCoroutine(ScaleC);
    }

}
