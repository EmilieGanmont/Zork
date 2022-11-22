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
        _entries.Enqueue(textLine.gameObject);

        if(_entries.Count >= MaxEntries)
        {
            _entries.Dequeue();
        }
    }

    private Queue<GameObject> _entries = new Queue<GameObject>();
}
