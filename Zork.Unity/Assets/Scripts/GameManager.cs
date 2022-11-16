using UnityEngine;
using Newtonsoft.Json;
using Zork.Common;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Location, Score, Moves;

    [SerializeField]
    private UnityInputService Input;

    [SerializeField]
    private UnityOutputService Output; 
    
    private void Awake()
    {
        TextAsset gameJson = Resources.Load<TextAsset>("GameJson");
        _game = JsonConvert.DeserializeObject<Game>(gameJson.text);
        _game.Run(Input, Output);
    }

    private Game _game;
}
