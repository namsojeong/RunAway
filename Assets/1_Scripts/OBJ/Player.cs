using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{

    SpriteRenderer spriteRenderer;

    float speed = 10f;

    Vector2 mousPos, transPos, targetPos;

    bool isShield = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!InGame.Instance.isRunning) return;
        //�̵�
        if (Input.GetMouseButton(0))
        {
            MovePos();
        }
        Move();

    }

    //�̵��Լ�
    private void MovePos()
    {
        mousPos = Input.mousePosition;
        SoundManager.Instance.Click();
        transPos = Camera.main.ScreenToWorldPoint(mousPos);
        targetPos = new Vector3(transPos.x, transPos.y, 0);
    }
    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
    }

    //����� ��
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!InGame.Instance.isRunning) return;
        if (collision.transform.tag == "PET" || collision.transform.tag == "PUSH")
        {
            if (isShield) return;
            SoundManager.Instance.SFXPlay(4);
            if (InGame.Instance.shieldCount >= 1)
            {
                InGame.Instance.shieldCount--;
                UIManager.Instance.ScoreUpText();
                StartCoroutine(Shield());
            }
            else
            {
                GameOver();
            }
        }
        if (collision.transform.tag == "SLOWITEM")
        {
            InGame.Instance.isSmall = true;
            SoundManager.Instance.SFXPlay(1);
            ObjectPool.Instance.ReturnObject(PoolObjectType.SLOW, collision.gameObject);
            InGame.Instance.Small();
        }
        if (collision.transform.tag == "SHIELD")
        {
            InGame.Instance.shieldCount++;
            SoundManager.Instance.SFXPlay(7);
            ObjectPool.Instance.ReturnObject(PoolObjectType.SHIELD, collision.gameObject);
        }
        if(collision.transform.tag=="SCALE")
        {
            StartCoroutine(ScaleItem());
            SoundManager.Instance.SFXPlay(5);
            ObjectPool.Instance.ReturnObject(PoolObjectType.SCALE, collision.gameObject);
        }
    }

    //����ũ�� ��� �� ����Ʈ
    IEnumerator ScaleItem()
    {
        float scale = Random.Range(0.5f, 2.5f);
        spriteRenderer.color = Color.cyan;
        transform.DOScale(new Vector3(scale, scale, 1f), 1.5f);
        yield return new WaitForSeconds(5f);
        transform.DOScale(new Vector3(1.6f, 1.6f, 1f), 1.5f);
        spriteRenderer.color = Color.white;
    }

    //���� ��� �� ����Ʈ
    IEnumerator Shield()
    {
        isShield = true;
        for (int i = 0; i < 4; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(.2f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(.2f);
        }
        isShield = false;
    }

    //���ӿ��� �� �Լ�(�ʱ�ȭ)
    void GameOver()
    {
        if (InGame.Instance.isSmall)
        {
            CancelInvoke("Slow");
            speed = 10f;
            InGame.Instance.isSmall = false;
        }
        isShield = false;
        spriteRenderer.color = Color.white;
        InGame.Instance.isRunning = false;
        targetPos = new Vector2(0, 0);
        InGame.Instance.PlayerSetting();
        InGame.Instance.OvertInvoke();
        UIManager.Instance.OverScoreText();
        SoundManager.Instance.BGMStop();
        GameManager.Instance.UpdateGameState(GameState.Over);
    }

}
