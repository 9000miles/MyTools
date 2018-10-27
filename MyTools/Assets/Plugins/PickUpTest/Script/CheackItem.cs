using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EntryType
{
    Event,
    item
}
[System.Serializable]
public class CheackItem : MonoBehaviour
{
    public string itemName;
    public int id;
    public EntryType myEntryType = EntryType.Event;
    public bool isBreathed;//是否损坏
    public bool isFinish;
}
