using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDataPersistence
{
    void LoadData(GameData data);
    void SaveData(ref GameData data);

}
