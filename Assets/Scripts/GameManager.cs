using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        Started,
        Running,
        GoalSequence,
        Stop,
        Won,
        Lost
    }

    private State _gameState;
    private int _totalGem;
    private int _gemCollectedInLevel;
    private int _gemMultiplier;
    private int _levelIndex;
    private bool _didPlayerWin;

    [Inject(InjectFrom.Anywhere)] public PlayerController playerController;
    [Inject(InjectFrom.Anywhere)] public Player player;
    [Inject(InjectFrom.Anywhere)] public LevelManager levelManager;
    [Inject(InjectFrom.Anywhere)] public CameraController cameraController;
    public GameObject environment;

    public int TotalGem => _totalGem;
    public int GemcCollectedInLevel => _gemCollectedInLevel;
    public int GemMultiplier => _gemMultiplier;
    public State GameState => _gameState;
    public int LevelIndex => _levelIndex;

    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // In first scene, make us the singleton.
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
            Destroy(gameObject); // On reload, singleton already set, so destroy duplicate.
    }

    private void OnEnable()
    {
        EventBroker.gemCollectedFromBonus += IncreaseGemFromBonus;
        EventBroker.gemReducedFromPurchase += ReduceGemFromPurchase;
        EventBroker.finishedLoading += FinishedLoadingCallback;
    }

    private void OnDisable()
    {
        EventBroker.gemCollectedFromBonus -= IncreaseGemFromBonus;
        EventBroker.gemReducedFromPurchase -= ReduceGemFromPurchase;
        EventBroker.finishedLoading -= FinishedLoadingCallback;
    }

    private void Start()
    {
        //Application.targetFrameRate = 60;
        _levelIndex = PlayerPrefs.GetInt(Settings.LAST_LEVEL_PLAYED, 0);
        _didPlayerWin = false;
        _gemCollectedInLevel = 0;
        _gemMultiplier = 0;
        _totalGem = PlayerPrefs.GetInt(Settings.GEM, 0);
        PokiAdManager.Instance.CommercialBreak();
    }

    private void Update()
    {
        //Debug.Log($"{totalGem}, {gemCollectedInLevel}");
        //Debug.Log(_gameState);
        switch (_gameState)
        {
            case State.Started:
                //Debug.Log(_levelIndex);
                _gemCollectedInLevel = 0;
                _gemMultiplier = 0;
                UIController.Instance.HideAllLevelTransitionUI();
                UIController.Instance.ShowMainMenu();

                playerController.InitStartingPos(PlayerPrefs.GetInt(Settings.STARTING_CUBE_NUMBER, 1));
                
                if (PlayerPrefs.GetInt("debug") != 1) levelManager.LoadLevel(_levelIndex);

                player.gameObject.GetComponent<ToggleRagdoll>().ToggleRagdollStatus(false);
                playerController.pathFollower.GetPath();

                _gameState = State.Running;
                EventBroker.CallHideLoadingScreen();
                break;
            case State.Running:
                break;
            case State.GoalSequence:
                cameraController.GoalSequenceCamera();
                if (_gemMultiplier == 0)
                    _gemMultiplier = playerController.NumberOfCube > 13 ? 20 :
                        playerController.NumberOfCube <= 2 ? 1 : playerController.NumberOfCube - 1;
                _didPlayerWin = true;
                _gameState = State.Running;
                break;
            case State.Stop:
                PokiAdManager.Instance.GameplayStop();
                _gameState = _didPlayerWin ? State.Won : State.Lost;
                break;
            case State.Won:
                if (_didPlayerWin)
                {
                    if (_gemMultiplier == 20)
                        PokiAdManager.Instance.HappyTime();
                    cameraController.LevelWonCamera();
                    UpdateTotalGem();
                    EventBroker.CallLevelWin();
                    _didPlayerWin = false;
                }

                break;
            case State.Lost:
                player.gameObject.GetComponent<ToggleRagdoll>().ToggleRagdollStatus(true);
                EventBroker.CallLevelLose();
                break;
        }
    }

    public void Init(bool retry)
    {
        PokiAdManager.Instance.GameplayStart();

        float val = Random.Range(0, 360);
        environment.transform.rotation = Quaternion.Euler(0, val, 0);
        
        if (retry) return;
        
        var lvlplayed = PlayerPrefs.GetInt(Settings.NUMBER_OF_LEVELS_PLAYED, 0);
        PlayerPrefs.SetInt(Settings.NUMBER_OF_LEVELS_PLAYED, lvlplayed + 1);
        if ((lvlplayed + 1) % 5 == 0 && lvlplayed >= 4) 
            PlayerPrefs.SetInt(Settings.LAST_BALLOON_LEVEL, lvlplayed);
        
        _levelIndex++;
        PlayerPrefs.SetInt(Settings.LAST_LEVEL_PLAYED, _levelIndex);

        _levelIndex = _levelIndex >= levelManager.levelPlaylist.Count
            ? Random.Range(0, levelManager.levelPlaylist.Count)
            : _levelIndex;
    }

    public void EndLevel()
    {
        _gameState = State.Stop;
    }

    public void LevelWin()
    {
        _didPlayerWin = true;
    }

    public void LevelLose()
    {
        _didPlayerWin = false;
    }

    public void IncreaseGem()
    {
        _gemCollectedInLevel++;
        EventBroker.CallUpdateGemUI();
    }

    private void IncreaseGemFromBonus(int val)
    {
        _gemCollectedInLevel = val;
        EventBroker.CallUpdateGemUI();
        _totalGem += val;
    }

    public void StartGoalSequence()
    {
        _gameState = State.GoalSequence;
    }

    private void UpdateTotalGem()
    {
        _totalGem += _gemCollectedInLevel;
        _gemCollectedInLevel *= _gemMultiplier;
        _totalGem += _gemCollectedInLevel;
        PlayerPrefs.SetInt(Settings.GEM, _totalGem);
    }

    private void FinishedLoadingCallback()
    {
        _gameState = State.Started;
    }
    
    private void ReduceGemFromPurchase(int amount)
    {
        _totalGem -= amount;
        PlayerPrefs.SetInt(Settings.GEM, _totalGem);
        EventBroker.CallUpdateGemUI();
    }

#if UNITY_EDITOR
    [ContextMenu("Reset Data")]
    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    
    [ContextMenu("Set Data")]
    public void SetPlayerPrefs()
    {
        PlayerPrefs.SetInt(Settings.LAST_LEVEL_PLAYED, 24);
    }
#endif
}