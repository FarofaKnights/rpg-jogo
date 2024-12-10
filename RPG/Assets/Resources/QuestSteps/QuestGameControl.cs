using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum GameControlOptions { NULL, TimeScale, CursorLock, CursorVisible, HUDVisible, PlayerControlsEnabled, InimigosEnabled, IsCutsceneMode }

public class QuestGameControl : QuestStep, IQuestInformations {
    public QuestInfo questInfo;
    public GameControlOptions gameControlOption;
    public bool controlValue;
    bool happened = false;


    void Start() {
        SetControl();
    }

    public override bool IsEfeitoPersistente { get { return true; } }

    public void SetControl() {
        if (happened) {
            return;
        }

        switch (gameControlOption) {
            case GameControlOptions.TimeScale:
                Time.timeScale = controlValue ? 1 : 0;
                break;
            case GameControlOptions.CursorLock:
                Cursor.lockState = controlValue ? CursorLockMode.Locked : CursorLockMode.None;
                break;
            case GameControlOptions.CursorVisible:
                Cursor.visible = controlValue;
                break;
            case GameControlOptions.HUDVisible:
                UIController.HUD.gameObject.SetActive(controlValue);
                break;
            case GameControlOptions.PlayerControlsEnabled:
                if (Player.instance != null && Player.instance.gameObject != null)
                    Player.instance.SetarControle(controlValue);
                break;
            case GameControlOptions.InimigosEnabled:
                GameManager.instance.SetInimigosAtivos(controlValue);
                break;
            case GameControlOptions.IsCutsceneMode:
                if (controlValue || GameManager.instance.state == GameState.Cutscene) {
                    GameManager.instance.SetCutsceneMode(controlValue);
                }
                break;
        }

        happened = true;
        FinishStep();
    }

    public void HandleQuestInformations(QuestInfo questInfo, string parameter) {
        this.questInfo = questInfo;
        this.questId = questInfo.questId;


        string[] parameters = SeparateParameters(parameter);

        GameControlOptions control = (GameControlOptions)System.Enum.Parse(typeof(GameControlOptions), parameters[0]);
        bool value = parameters.Length > 1 ? bool.Parse(parameters[1]) : true;

        this.gameControlOption = control;
        this.controlValue = value;

        SetControl();
    }

    #if UNITY_EDITOR
    public override string GetEditorName() { return "AÇÃO: Controlar jogo"; }

    public override string GetEditorParameters(CurrentStepAcaoInfo stepInfo) {
        string[] parameters = SeparateParameters(stepInfo.step.parameter ?? "");
        
        string control = parameters.Length > 0 ? parameters[0] : "NULL";
        GameControlOptions gameControlOption = (GameControlOptions)System.Enum.Parse(typeof(GameControlOptions), control);
        gameControlOption = (GameControlOptions)EditorGUILayout.EnumPopup("Control:", gameControlOption);
        control = gameControlOption.ToString();

        bool controlValue = true;

        try {
            controlValue = parameters.Length > 1 ? bool.Parse(parameters[1]) : true;
        } catch { }

        controlValue = EditorGUILayout.Toggle("Valor:", controlValue);
        string value = controlValue.ToString();

        return JoinParameters(new string[] { control, value });
    }
    #endif
}
