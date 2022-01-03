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
    GameObject ai;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    public float petSpeed = 3f;
    public int shieldCount = 0;

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
    public bool isSlow = false;
    bool isPush = false;

    public bool isRunning = false;

    Coroutine SlowC;
    Coroutine ShieldC;

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
            if (GameManager.Instance.timeScore >= 50)
            {
                if (isPush) return;
                //푸쉬 에너미 생성 시작
                InvokeRepeating("PushPosition", 0f, pushDelay);
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
        float ranDelaySlow = Random.Range(20f, 40f);
        float ranDelayShield = Random.Range(30f, 65f);
        InvokeRepeating("SlowSpawn", ranDelaySlow, ranDelaySlow);
        InvokeRepeating("ShieldSpawn", ranDelayShield, ranDelayShield);
    }
    public void OvertInvoke()
    {
        CancelInvoke("SlowSpawn");
        CancelInvoke("ShieldSpawn");
    }

    //제자리
    public void PlayerSetting()
    {
        DOTween.KillAll();

        //위치 제자리
        ai.transform.position = new Vector3(-9, -3.5f, 0);
        player.transform.position = new Vector3(0, 0, 0);
        push.transform.position = new Vector3(20f, 0f, 0f);

        PlayerPrefs.SetFloat("PET_SPEED", 3f);
        petSpeed = 3f;
        warn.SetActive(false);
        shieldCount = 0;
        isPush = false;
        isShield = false;

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
            push.transform.position = new Vector3(-20f, randomPushPos, 0);
            warn.transform.localPosition = new Vector3(1.4f, 0, 0);
        }
        else
        {
            push.transform.position = new Vector3(20f, randomPushPos, 0);
            warn.transform.localPosition = new Vector3(-1.4f, 0, 0);
        }
        warn.SetActive(true);
        StartCoroutine(WarnTime());
    }
    void WarnTimer()
    {

        warn.SetActive(false);
        if (!isRunning || !isPush) return;
        push.transform.DOMoveX(push.transform.position.x * (-1), 2f)
            .OnComplete(() =>
            {
                isPush = false;
                if (pushSpeed >= 0.5)
                    pushSpeed -= 0.05f;
                pushDelay = Random.Range(0, 10f);
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
    public void Slow()
    {
        PlayerPrefs.SetFloat("PET_SPEED", InGame.Instance.petSpeed);
        isSlow = true;
        StartCoroutine(SlowTime());
    }
    IEnumerator SlowTime()
    {

        petSpeed = 5f;
        spriteRenderer.color = Color.green;
        yield return new WaitForSeconds(5f);
        spriteRenderer.color = Color.white;
        petSpeed = PlayerPrefs.GetFloat("PET_SPEED");
        isSlow = false;

    }
    void SlowSpawn()
    {
        SlowC = StartCoroutine(SlowOn());
    }
    IEnumerator SlowOn()
    {
        GameObject potion = ObjectPool.Instance.GetObject(PoolObjectType.SLOW);
        potion.transform.position = new Vector3(Random.Range(-10f, 10f), Random.Range(-4f, 4f), 0f);
        yield return new WaitForSeconds(10f);
        ObjectPool.Instance.ReturnObject(PoolObjectType.SLOW, potion);
        StopCoroutine(SlowC);
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

}
