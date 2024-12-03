using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Abismo : MonoBehaviour {
    void Awake() {
        Debug.Log("Substituir pelo script correto, tenho como objetivo excluir o script Abismo até o final da build, conto com a colaboração de todos! Clique nos ... do componente e vai ter a opção 'Setup Correct Script', só clicar nela :)");
        SetupCorrectScript();
    }

    [ContextMenu("Setup Correct Script")]
    public void SetupCorrectScriptMenu() {
        SetupCorrectScript(true);
    }
    
    public void SetupCorrectScript(bool imediato = false) {
        OnTrigger onT = gameObject.GetComponent<OnTrigger>();
        if (onT == null) onT = gameObject.AddComponent<OnTrigger>();

        FallTrigger fallT = gameObject.GetComponent<FallTrigger>();
        if (fallT == null) {
            fallT = gameObject.AddComponent<FallTrigger>();
            fallT.contarComoMorte = true;
        }

        if (gameObject.layer != LayerMask.NameToLayer("Trigger")) {
            gameObject.layer = LayerMask.NameToLayer("Trigger");
        }
        
        if (imediato) DestroyImmediate(this);
        else Destroy(this);
    }
}
