using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueAnimationEvents : MonoBehaviour {
    public AtaqueInstance ataqueInstance;

    public void SetAntecipacao() {
        if (ataqueInstance == null) return;
        
        //Debug.Log("Antecipacao");
        ataqueInstance.OnStateChange(AtaqueInstance.Estado.Antecipacao);
    }

    public void SetHit() {
        if (ataqueInstance == null) return;

        //Debug.Log("Hit");
        ataqueInstance.OnStateChange(AtaqueInstance.Estado.Hit);
    }

    public void SetRecovery() {
        if (ataqueInstance == null) return;

        //Debug.Log("Recovery");
        ataqueInstance.OnStateChange(AtaqueInstance.Estado.Recovery);
    }

    public void SetEnd() {
        if (ataqueInstance == null) return;

        //Debug.Log("End");
        ataqueInstance.OnStateChange(AtaqueInstance.Estado.End);
        ataqueInstance = null;
    }

    public void PodeCancelar() {
        if (ataqueInstance == null) return;

        ataqueInstance.PermitirCancelamento();
    }

    public void TriggerAHit() {
        if (ataqueInstance == null) return;

        ataqueInstance.TriggerAHit();
    }
    
}
