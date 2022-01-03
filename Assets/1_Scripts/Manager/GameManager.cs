using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    GameState state = GameState.Main;

    int escTime = 0;
    bool isEsc = false;

    public int timeScore = 0;
    public int bestScore = 0;

    public bool isFir = false;
    public bool isSec = true;
    public bool isth = true;

    private void Awake()
    {
        //해상도 고정
        Screen.SetResolution(2960, 1440, true);

        Instance = this;
        if (Instance == null)
            Instance = FindObjectOfType<GameManager>();

    }

    private void Start()
    {
        //배경음 시작
        SoundManager.Instance.BGMPlay();
        bestScore = PlayerPrefs.GetInt("BESTSCORE", 0);
    }
    private void Update()
    {
        //백키 대응
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isEsc) return;
            isEsc = true;
            InvokeRepeating("EscTimer", 0f, 1f);
            
        }

    }
    //백키 대응 함수
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

    //씬 전환
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
                EnemyOff(1);
                EnemyOff(2);
                EnemyOff(3);
                SoundManager.Instance.BGMPlay();
                CancelInvoke("ScoreUP");
                break;
        }
    }

    //스코어 올리기
    public void ScoreUP()
    {
        timeScore++;
        if (timeScore > bestScore)
        {
            bestScore = timeScore;
            PlayerPrefs.SetInt("BESTSCORE", bestScore);
        }
        UIManager.Instance.ScoreUpText();
    }

    public void EnemyOff(int n)
    {
        if(n==1)
        {
            isFir = true;
        }
        else if(n==2)
        {
            isSec = true;
        }
        else if(n==3)
        {
            isth = true;
        }
    }
    public void EnemyOn(int n)
    {
        if(n==1)
        {
            isFir = false;
        }
        else if(n==2)
        {
            isSec = false;
        }
        else if(n==3)
        {
            isth = false;
        }
    }
}
