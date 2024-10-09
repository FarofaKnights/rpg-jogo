using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcaoParamsAnimator : Acao {
    public string idAnimator, triggerName;
    public object valor;

    public new static string[] GetParametrosUtilizados(){ return new string[] { "id", "id2", "value" }; }
    public new static string[] GetParametrosTraduzidos(){ return new string[] { "Id do AnimatorObject", "Nome do parâmetro", "Valor" }; }

    public AcaoParamsAnimator(string idMissao, string triggerName, object valor) {
        this.idAnimator = idMissao;
        this.triggerName = triggerName;
        this.valor = valor;
    }

    public AcaoParamsAnimator(AcaoParams parametros): base(parametros) {
        idAnimator = parametros.id;
        triggerName = parametros.id2;
        valor = parametros.GetValue();
    }

    public override void Realizar() {
        AnimatorObject[] animatorObjects = GameManager.instance.GetObjectsOfType<AnimatorObject>(true);
        System.Action<Animator> setParameter = null;

        if(valor is int) {
            setParameter = (animator) => animator.SetInteger(triggerName, (int)valor);
        } else if(valor is float) {
            setParameter = (animator) => animator.SetFloat(triggerName, (float)valor);
        } else if(valor is bool) {
            setParameter = (animator) => animator.SetBool(triggerName, (bool)valor);
        } else {
            setParameter = (animator) => animator.SetTrigger(triggerName);
        }

        
        foreach (AnimatorObject animatorObject in animatorObjects) {
            if (animatorObject.animatorId == idAnimator) {
                Animator animator = animatorObject.GetComponent<Animator>();
                setParameter(animator);
            }
        }

        setParameter = null;
    }

}
