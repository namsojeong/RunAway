using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    GameObject[] screen;
    [SerializeField]
    GameObject[] screenObj;

    [Header("�ΰ���")]
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text shieldText;

    [Header("���ӿ���")]
    [SerializeField]
    Text overScoreText;
    [SerializeField]
    Text overBestScoreText;

    private void Awake()
    {
        Instance = this;
        if (Instance == null)
            Instance = FindObjectOfType<UIManager>();

        UpdateState(0);
    }

    //�� ������Ʈ
    public void UpdateState(int sc)
    {
        for (int i = 0; i < screen.Length; i++)
        {
            screen[i].SetActive(false);
            screenObj[i].SetActive(false);
        }
        if (sc == 2)
        {
            screen[0].SetActive(true);
            screenObj[0].SetActive(true);
        }
        screen[sc].SetActive(true);
        screenObj[sc].SetActive(true);

    }

    //���ھ� �ؽ�Ʈ
    public void ScoreUpText()
    {
        scoreText.text = string.Format("{0}", GameManager.Instance.timeScore);
        shieldText.text = string.Format("{0}", InGame.Instance.shieldCount);
    }

    //���ӿ��� UI ������Ʈ
    public void OverScoreText()
    {
        overBestScoreText.text = string.Format("BEST {0}", GameManager.Instance.bestScore);
        overScoreText.text = string.Format("{0}", GameManager.Instance.timeScore);
    }

    //�÷��̹�ư �Լ�
    public void StartPlay()
    {

        InGame.Instance.PlayerSetting();
        InGame.Instance.StartInvoke();
        GameManager.Instance.UpdateGameState(GameState.RUNNING);
    }


}
