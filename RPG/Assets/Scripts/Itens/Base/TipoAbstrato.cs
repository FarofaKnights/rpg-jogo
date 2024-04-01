using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TipoAbstrato : MonoBehaviour {
    public virtual bool IsInstanciavel { get { return true; } }
    public virtual bool IsUsavel { get { return true; } }

    public abstract void FazerAcao();
}
