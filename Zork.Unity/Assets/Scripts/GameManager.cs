using UnityEngine;
using Newtonsoft.Json;
using Zork.Common;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI LocationText, ScoreText, StatusText, MovesText;

    [SerializeField]
    private UnityInputService InputService;

    [SerializeField]
    private UnityOutputService OutputService; 
    
    private void Awake()
    {
        TextAsset gameJson = Resources.Load<TextAsset>("GameJson");
        _game = JsonConvert.DeserializeObject<Game>(gameJson.text);
        _game.Player.LocationChange += Player_LocationChanged;

        _game.Player.MovesChanged += Player_MovesChanged;
        _game.Player.ScoreChanged += Player_ScoreChanged;
        _game.Player.StatusChanged += Player_StatusChanged;
        _game.Run(InputService, OutputService);
    }

    private void Start()
    {
        InputService.ProcessInput();
        InputService.SetFocus();
        LocationText.text = _game.Player.CurrentRoom.Name;
        ScoreText.text = $"Score: {_game.Player.Score}";
        MovesText.text = $"Moves: {_game.Player.Moves}";
        StatusText.text = "Healthy";
    }

    private void Player_LocationChanged(object sender, Room location)
    {
        LocationText.text = location.Name;
    }

    private void Player_ScoreChanged(object sender, int score)
    {
        ScoreText.text = $"Score: {score}";
    }

    private void Player_StatusChanged(object sender, int currentHealth)
    {
        if (currentHealth == _game.Player.MaxHealth )
        {
            StatusText.text = "Healthy";
        }
        else if (currentHealth <= 0)
        {
            StatusText.text = "Dead";
        }
        else
        {
            StatusText.text = "Wounded";
        }
    }

    private void Player_MovesChanged(object sender, int moves)
    {
        MovesText.text = $"Moves: {moves}";
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            InputService.ProcessInput();
            InputService.SetFocus();
        }

        if(_game.IsRunning == false)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    private Game _game;
}
