using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    GameState state = GameState.Main;

    int escTime = 0;
    bool isEsc = false;

    public int timeScore = 0;
    public int bestScore = 0;

    private void Awake()
    {
        //�ػ� ����
        Screen.SetResolution(2960, 1440, true);

        Instance = this;
        if (Instance == null)
            Instance = FindObjectOfType<GameManager>();

    }

    private void Start()
    {
        //����� ����
        SoundManager.Instance.BGMPlay();
    }
    private void Update()
    {
        //��Ű ����
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isEsc) return;
            isEsc = true;
            InvokeRepeating("EscTimer", 0f, 1f);
            
        }

    }
    //��Ű ���� �Լ�
    void EscTimer()
    {
        escTime++;
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if(escTime>=5)
        {
            CancelInvoke("EscTimer");
        }

    }

    //�� ��ȯ
    public void UpdateGameState(GameState state)
    {
        this.state = state;
        UIManager.Instance.UpdateState((int)state);
        switch (state)
        {
            case GameState.RUNNING:
                InGame.Instance.PlayerSetting();
                InGame.Instance.isRunning = true;
                timeScore = 0;
                UIManager.Instance.ScoreUpText();
                InvokeRepeating("ScoreUP", 1f, 1f);
                break;
            case GameState.Main:
                InGame.Instance.isRunning = false;
                CancelInvoke("ScoreUP");
                break;
            case GameState.Over:
                InGame.Instance.isRunning = false;
                SoundManager.Instance.BGMPlay();
                CancelInvoke("ScoreUP");
                break;
        }
    }

    //���ھ� �ø���
    public void ScoreUP()
    {
        timeScore++;
        if (timeScore > bestScore)
        {
            bestScore = timeScore;
        }
        UIManager.Instance.ScoreUpText();
    }


}
