using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStartPoint : MonoBehaviour {
    public bool ativo;

    // Start is called before the first frame update
    void Start() {
        #if UNITY_EDITOR
        if (ativo) {
            StartCoroutine(SkipSomeFrames());
        }
        #endif
    }

    IEnumerator SkipSomeFrames() {
        yield return new WaitForSeconds(0.5f);

        Player.instance.TeleportTo(transform.position);
    }
}
