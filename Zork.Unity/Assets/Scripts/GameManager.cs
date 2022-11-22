using UnityEngine;
using Newtonsoft.Json;
using Zork.Common;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Location, Score, Moves;

    [SerializeField]
    private UnityInputService InputService;

    [SerializeField]
    private UnityOutputService OutputService; 
    
    private void Awake()
    {
        TextAsset gameJson = Resources.Load<TextAsset>("GameJson");
        _game = JsonConvert.DeserializeObject<Game>(gameJson.text);
        _game.Player.LocationChange += Player_LocationChanged;
        _game.Run(InputService, OutputService);
    }

    private void Start()
    {
        InputService.ProcessInput();
        InputService.SetFocus();
        Location.text = _game.Player.CurrentRoom.Name;
    }

    private void Player_LocationChanged(object sender, Room location)
    {
        Location.text = location.Name;
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            InputService.ProcessInput();
            InputService.SetFocus();
        }


    }

    private Game _game;
}
