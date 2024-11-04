using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoad : MonoBehaviour
{
    public void Load(int Index) 
    {
        DataManager.instance.GameLoad(Index - 1);
    }
}
