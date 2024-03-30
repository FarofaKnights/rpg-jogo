using UnityEngine;

[RequireComponent(typeof(Item))]
public abstract class Consumivel: TipoAbstrato {
    public abstract void Use();
    
    public override void FazerAcao() {
        Use();
        Destroy(gameObject);
    }
}