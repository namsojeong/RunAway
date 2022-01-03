using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    GameObject[] screen;
    [SerializeField]
    GameObject[] screenObj;


    [Header("인게임")]
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text shieldText;

    [Header("게임오버")]
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

    //패널 껐다키기
    public void OpenUI(GameObject panel)
    {
        panel.SetActive(true);
    }
    public void CloseUI(GameObject panel)
    {
        panel.SetActive(false);
    }

    //종료
    public void Quit()
    {
        Debug.Log("종료");
            Application.Quit();
    }

    //씬 업데이트
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

    //스코어 텍스트
    public void ScoreUpText()
    {
        scoreText.text = string.Format("{0}", GameManager.Instance.timeScore);
        shieldText.text = string.Format("{0}", InGame.Instance.shieldCount);
    }

    //게임오버 UI 업데이트
    public void OverScoreText()
    {
        overBestScoreText.text = string.Format("BEST\n{0}", GameManager.Instance.bestScore);
        overScoreText.text = string.Format("{0}", GameManager.Instance.timeScore);
    }

    //플레이버튼 함수
    public void StartPlay()
    {
        GameManager.Instance.EnemyOn(1);
        InGame.Instance.PlayerSetting();
        InGame.Instance.StartInvoke();
        GameManager.Instance.UpdateGameState(GameState.RUNNING);
    }

}
