using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Drop : MonoBehaviour {
    public Vector3 posicaoIndicador;
    public abstract void OnCollect();
}
