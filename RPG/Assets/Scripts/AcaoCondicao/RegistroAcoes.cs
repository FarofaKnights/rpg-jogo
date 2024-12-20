using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AcoesRegistradas {
    NULL,
    SETAR_VARIAVEL,
    ADICIONAR_ITEM,
    REMOVER_ITEM,
    TRIGGAR_MISSAO,
    COMECAR_MISSAO,
    TRIGGAR_ANIMATOR,
    PARAMETRO_ANIMATOR,
    TROCAR_CAMERA,
    RODA_FUNCAO,
    TELEPORTA,
    MUDA_CENA,
    EDITAR_VARIAVEL,
    TOCAR_SOM,
}

public class RegistroAcoes {
    public static Dictionary<AcoesRegistradas, System.Type> acoesRegistradas = new Dictionary<AcoesRegistradas, System.Type>() {
        { AcoesRegistradas.SETAR_VARIAVEL, typeof(AcaoSetVariavel) },
        { AcoesRegistradas.ADICIONAR_ITEM, typeof(AcaoDaItem) },
        { AcoesRegistradas.REMOVER_ITEM, typeof(AcaoRemoveItem) },
        { AcoesRegistradas.TRIGGAR_MISSAO, typeof(AcaoTriggaMissao) },
        { AcoesRegistradas.COMECAR_MISSAO, typeof(AcaoComecaMissao) },
        { AcoesRegistradas.TRIGGAR_ANIMATOR, typeof(AcaoTriggaAnimator) },
        { AcoesRegistradas.PARAMETRO_ANIMATOR, typeof(AcaoParamsAnimator) },
        { AcoesRegistradas.TROCAR_CAMERA, typeof(AcaoChangeCam) },
        { AcoesRegistradas.RODA_FUNCAO, typeof(AcaoRodaFuncao) },
        { AcoesRegistradas.TELEPORTA, typeof(AcaoTeleporta) },
        { AcoesRegistradas.MUDA_CENA, typeof(AcaoMudaCena) },
        { AcoesRegistradas.EDITAR_VARIAVEL, typeof(AcaoEditVariavel) },
        { AcoesRegistradas.TOCAR_SOM, typeof(AcaoTocaSom) },
    };


    public static System.Type GetRegistro(AcoesRegistradas acao) {
        if (acoesRegistradas.ContainsKey(acao)) {
            return acoesRegistradas[acao];
        }

        return null;
    }

    public static AcoesRegistradas GetAcao(string acao) {
        return (AcoesRegistradas)System.Enum.Parse(typeof(AcoesRegistradas), acao);
    }

    public static string GetAcaoString(AcoesRegistradas acao) {
        return acao.ToString();
    }

    public static string[] GetAcoesString() {
        return System.Enum.GetNames(typeof(AcoesRegistradas));
    }
}
