using UnityEngine;
using UnityEngine.UI;
using Zork.Common;
using TMPro;
using System.Collections.Generic;

public class UnityOutputService : MonoBehaviour, IOutputService
{
    [SerializeField]
    private TextMeshProUGUI TextLinePrefab;

    [SerializeField]
    private Image NewLinePrefab; 

    [SerializeField]
    private Transform ContentTransform;

    [SerializeField]
    [Range (0, 50)]
    private int MaxEntries;

    public void Write(object obj)
    {
        ParseWriteLine(obj.ToString());
    }

    public void Write(string message)
    {
        ParseWriteLine(message);
    }

    public void WriteLine(object obj)
    {
        ParseWriteLine(obj.ToString());
    }

    public void WriteLine(string message)
    {
        ParseWriteLine(message);
    }

    public void ParseWriteLine(string message)
    {

        var textLine = Instantiate(TextLinePrefab, ContentTransform);
        textLine.text = message;

        var newLine = Instantiate(NewLinePrefab, ContentTransform);

        _entries.Enqueue(textLine.gameObject);
        _entries.Enqueue(newLine.gameObject);

        if (_entries.Count >= MaxEntries)
        {
            while(_entries.Count >= MaxEntries)
            {
                Destroy(_entries.Dequeue());
            }
        }
    }

    private Queue<GameObject> _entries = new Queue<GameObject>();
}
