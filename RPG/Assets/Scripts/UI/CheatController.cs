using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class CheatController : MonoBehaviour {
    public Dropdown addItemDropdown, teletransporteDropdown;
    public Dropdown escopoDropdown, tipoDropdown;
    public InputField variavelInput, valorInput;
    public Text variablesText;

     GameState oldState;
    bool storesState = false;


    void Start() {
        GerarDropdownItens();
        GerarDropdownTeletransporte();
        GerarDropdownTipo();
    }

    public void Entrar(GameState oldState) {
        this.oldState = oldState;
        storesState = true;
        Entrar();
    }

    public void Entrar() {
        GameManager.instance.Pausar();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameObject.SetActive(true);
        GerarDropdownEscopo();
        GerarVariaveisText();
    }

    public void Sair() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager.instance.Despausar();

        if (storesState)
            GameManager.instance.SetState(oldState);

        storesState = false;
        gameObject.SetActive(false);
    }

    void GerarDropdownItens() {
        ItemList list = Resources.Load<ItemList>("Itens/TodosItens");
        List<string> opcoes = new List<string>();

        foreach (RelacaoNomeItem relacao in list.itens) {
            string nome = relacao.nome;
            ItemData item = relacao.item;
            opcoes.Add(nome);
        }

        addItemDropdown.ClearOptions();
        addItemDropdown.AddOptions(opcoes);
    }

    void GerarDropdownTeletransporte() {
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        List<string> opcoes = new List<string>();

        foreach (SpawnPoint spawnPoint in spawnPoints) {
            string nome = spawnPoint.pointName;
            opcoes.Add(nome);
        }

        teletransporteDropdown.ClearOptions();
        teletransporteDropdown.AddOptions(opcoes);
    }

    void GerarDropdownEscopo() {
        List<string> opcoes = new List<string>();
        string[] valores = SaveSystem.instance.variables.GetEscopos();
        foreach (string valor in valores) {
            opcoes.Add(valor);
        }

        escopoDropdown.ClearOptions();
        escopoDropdown.AddOptions(opcoes);
    }

    void GerarDropdownTipo() {
        List<string> opcoes = new List<string>();
        string[] valores = SaveSystem.instance.variables.GetTipos();
        foreach (string valor in valores) {
            opcoes.Add(valor);
        }

        tipoDropdown.ClearOptions();
        tipoDropdown.AddOptions(opcoes);
    }

    public void GerarVariaveisText() {
        string escopo = escopoDropdown.options[escopoDropdown.value].text;
        string[] variaveis = SaveSystem.instance.variables.GetEscopo(escopo).GetVariables();

        string text = "";
        foreach (string variavel in variaveis) {
            text += variavel + "\n";
        }

        if (text == "")
            text = "Escopo selecionado ainda não possui variáveis.";

        variablesText.text = text;
    }

    ItemData GetItemDataFromTodos(string nome) {
        ItemList list = Resources.Load<ItemList>("Itens/TodosItens");
        foreach (RelacaoNomeItem relacao in list.itens) {
            if (relacao.nome == nome)
                return relacao.item;
        }

        return null;
    }

    public void AddItem() {
        int i = addItemDropdown.value;

        string id = addItemDropdown.options[i].text;
        ItemData data = GetItemDataFromTodos(id);   

        Player.instance.inventario.AddItem(data);
    }

    public void Teletransportar() {
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();

        int i = teletransporteDropdown.value;
        string nome = teletransporteDropdown.options[i].text;

        foreach (SpawnPoint spawnPoint in spawnPoints) {
            if (nome == spawnPoint.pointName){
                Player.instance.TeleportTo(spawnPoint.transform.position);
            }
        }
    }

    public void SetarVidaInfinita() {
        Player p = Player.instance;
        PossuiVida vida = p.GetComponent<PossuiVida>();
        vida.SobreescreverVida(9999);
    }

    public void SetarCalorInfinito() {
        Player p = Player.instance;
        p.SobreescreverCalor(9999);
    }

    public void SetarPecaInfinita() {
        Player p = Player.instance;
        p.AddPecas(9999);
    }

    public void VidaToMaxima() {
        Player p = Player.instance;
        PossuiVida vida = p.GetComponent<PossuiVida>();
        vida.CurarTotalmente();
    }

    public void SetarVariavel() {
        string escopo = escopoDropdown.options[escopoDropdown.value].text;
        string nome = variavelInput.text;
        string tipoString = tipoDropdown.options[tipoDropdown.value].text.ToLower();
        string valorString = valorInput.text;


        object valor = ConvertStringToType(valorString, tipoString);
        PrimitiveType tipo = PrimitiveVariable.GetVariableType(tipoString);

        SaveSystem.instance.variables.SetVariable(nome, tipo, valor, escopo);

        GerarVariaveisText();
    }

    object ConvertStringToType(string valorString, string tipo) {
        switch (tipo) {
            case "int":
                return int.Parse(valorString);
            case "float":
                return float.Parse(valorString);
            case "string":
                return valorString;
            case "bool":
                return bool.Parse(valorString);
            default:
                return null;
        }
    }

    public void ShowSaveFiles() {
        string folder = SaveSystem.instance.GetSaveFolder();
        folder = folder.Replace(@"/", @"\");
        System.Diagnostics.Process.Start("explorer.exe", "/select,"+folder);
    }

    public void MaximoNosStats() {
        Player p = Player.instance;
        p.stats.SetDestreza(5);
        p.stats.SetForca(5);
        p.stats.SetVida(5);
        p.stats.SetCalor(5);
    }
}
