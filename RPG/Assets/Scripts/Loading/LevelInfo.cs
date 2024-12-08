using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RelacaoPrefabELocal {
    public string prefabPath;
    public string local;
}

[CreateAssetMenu(fileName = "LevelInfo", menuName = "RPG/Level Info", order = 51)]	
public class LevelInfo : ScriptableObject {
    public string nomeCena;
    public string pontoInicial;

    public RelacaoPrefabELocal[] prefabsAInstanciar;
    public string[] audioClipPaths;
    public string[] videoClipPaths;
}
