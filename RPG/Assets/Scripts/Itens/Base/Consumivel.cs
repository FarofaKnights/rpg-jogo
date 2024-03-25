using UnityEngine;

public abstract class Consumivel: TipoAbstrato {
    public abstract void Use();
    
    public override void FazerAcao() {
        Use();
    }
}