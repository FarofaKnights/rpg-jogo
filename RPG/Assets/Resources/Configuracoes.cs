using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reservado para as configurações do sistema (da unity, não do jogo)
[CreateAssetMenu(fileName = "Configuracoes", menuName = "RPG/ProjectConfigs")]
public class Configuracoes : ScriptableObject {
    public string nomeDoJogo;
    public GameObject indicadorPrefab; 
}