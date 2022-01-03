using System.Collections;
using UnityEngine;
using DG.Tweening;


public class Pet : MonoBehaviour
{
    [Header("�÷��̾� ��ġ")]
    [SerializeField]
    Transform playerT;

    Vector3 targetPosition = Vector3.zero;

    int isRan = 0;

    //���� ������ + �Ѿƿ���
    public void MovePet()
    {
        if (!InGame.Instance.isRunning) return;
        else
        {

            if (isRan >= 40)
            {
                targetPosition.x = playerT.position.x;
                targetPosition.y = playerT.position.y;
            }
            else if (isRan < 40)
            {
                targetPosition.x = Random.Range(-10f, 10.1f);
                targetPosition.y = Random.Range(-4f, 4f);
            }

            targetPosition.z = 0f;
            
        if (!InGame.Instance.isRunning) return;
            transform.DOMove(targetPosition, InGame.Instance.petSpeed, false)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    if (InGame.Instance.petSpeed > 0.75f && !InGame.Instance.isSlow)
                    {
                        InGame.Instance.petSpeed -= 0.07f;
                        

                    }
                    if (isRan >= 40)
                    {
                        isRan = Random.Range(0, 101);
                        MovePet();
                    }
                    else
                    {
                        isRan = Random.Range(0, 101);
                        CircleMove();
                    }
                });
        }

    }

    //�̴Ͽ��ʹ� ����
    void CircleMove()
    {
        if (!InGame.Instance.isRunning) return;
        StartCoroutine(LeaveEnemy());
    }
    IEnumerator LeaveEnemy()
    {
        
        GameObject enemy = ObjectPool.Instance.GetObject(PoolObjectType.ENEMY);
        enemy.transform.position = transform.position;
        SoundManager.Instance.SFXPlay(0);
        MovePet();
        yield return new WaitForSeconds(13f);
        ObjectPool.Instance.ReturnObject(PoolObjectType.ENEMY, enemy);
        StopAllCoroutines();
    }

}

