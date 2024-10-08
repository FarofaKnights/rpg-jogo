using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcaoTriggaAnimator : Acao {
    public string idAnimator, triggerName;
    public bool isTrigger;

    public new static string[] GetParametrosUtilizados(){ return new string[] { "id", "stringValue", "boolValue" }; }
    public new static string[] GetParametrosTraduzidos(){ return new string[] { "Id do AnimatorObject", "Nome do Trigger", "É trigger" }; }

    public AcaoTriggaAnimator(string idMissao, string triggerName, bool isTrigger) {
        this.idAnimator = idMissao;
        this.triggerName = triggerName;
        this.isTrigger = isTrigger;
    }

    public AcaoTriggaAnimator(AcaoParams parametros): base(parametros) {
        idAnimator = parametros.id;
        triggerName = parametros.stringValue;
        isTrigger = parametros.boolValue;
    }

    public override void Realizar() {
        AnimatorObject[] animatorObjects = GameManager.instance.GetObjectsOfType<AnimatorObject>(true);
        foreach (AnimatorObject animatorObject in animatorObjects) {
            if (animatorObject.animatorId == idAnimator) {
                Animator animator = animatorObject.GetComponent<Animator>();
                if (isTrigger) {
                    animator.SetTrigger(triggerName);
                } else {
                    bool valor = animator.GetBool(triggerName);
                    animator.SetBool(triggerName, !valor);
                }
            }
        }
    }

}
