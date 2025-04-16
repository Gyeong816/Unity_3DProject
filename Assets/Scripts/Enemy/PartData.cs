using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Parts
{
    Unknown,
    Body,
    Head
}

[System.Serializable]
public class PartData
{
    public Parts partType;
    public Collider collider;
    public float damageValue = 1f;
}
