using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DualPosition {
    public Vector3 pos1;
    public Vector3 pos2;

    public DualPosition(Vector3 pos1, Vector3 pos2) {
        this.pos1 = pos1;
        this.pos2 = pos2;
    }
}

public class CacadoraArena : MonoBehaviour {
    public float areaSpawn = 20f;
    public float areaVisivel = 10f;

    public Vector3 GetVectorFromAngle(float angle) {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
    }

    public DualPosition GetRandomOutInPosition() {
        float angle = Random.Range(0, 360);
        Vector3 outPosition = transform.position + GetVectorFromAngle(angle) * areaSpawn;
        Vector3 inPosition = transform.position + GetVectorFromAngle(angle) * areaVisivel;
        return new DualPosition(outPosition, inPosition);
    }

    public DualPosition GetRandomInOutPosition() {
        float angle = Random.Range(0, 360);
        Vector3 inPosition = transform.position + GetVectorFromAngle(angle) * areaVisivel;
        Vector3 outPosition = transform.position + GetVectorFromAngle(angle) * areaSpawn;
        return new DualPosition(inPosition, outPosition);
    }

    public DualPosition GetPositionXAnglesFromPos(Vector3 pos, float angle) {
        // 1. Descobrir o angulo da posição inicial
        Vector3 direction = pos - transform.position;
        float angleRad = Mathf.Atan2(direction.z, direction.x);
        float angleDeg = angleRad * (180 / Mathf.PI);

        // 2. Calcular os angulos finais
        float angle1 = angleDeg - angle / 2;
        float angle2 = angleDeg + angle / 2;

        // 3. Calcular as posições finais
        Vector3 pos1 = transform.position + GetVectorFromAngle(angle1) * areaSpawn;
        Vector3 pos2 = transform.position + GetVectorFromAngle(angle2) * areaSpawn;

        return new DualPosition(pos1, pos2);
    }

    public DualPosition GetPairFromPos(Vector3 pos) {
        Vector3 direction = pos - transform.position;
        float angleRad = Mathf.Atan2(direction.z, direction.x);
        float angleDeg = angleRad * (180 / Mathf.PI);

        Vector3 pos1 = transform.position + GetVectorFromAngle(angleDeg) * areaVisivel;
        Vector3 pos2 = transform.position + GetVectorFromAngle(angleDeg) * areaSpawn;

        return new DualPosition(pos1, pos2);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaSpawn);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, areaVisivel);
    }
}
