using UnityEngine;

[RequireComponent(typeof(Item))]
public abstract class Consumivel: TipoAbstrato {
    public override bool IsInstanciavel { get { return false; } }
    public abstract void Use();
    
    public override void FazerAcao() {
        Use();
    }
}