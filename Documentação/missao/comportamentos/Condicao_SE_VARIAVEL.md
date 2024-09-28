# Condição/SE_VARIAVEL

Uma condição que aciona quando uma determinada variável de ambiente respeita uma condição definida.

| Atributo | Informativo |
| -- | -- |
| **Script** | [CondicaoIfVariavel](../../../RPG/Assets/Scripts/AcaoCondicao/Condicoes/CondicaoIfVariavel.cs) |
| **Nome** | Condição => SE_VARIAVEL |
| **Tipo** | Condição (Condição de Fala) |

| Parâmetro | Descrição |
| -- | -- |
| **Variável** | O nome da variável a ser comparada |
| **Comparação** | Tipo de comparação |
| **Tipo** | Tipo da variável (int, float, bool ou string) |
| **Valor** | Valor a ser comparado com a variável |
| **Variável global** | Se é uma variável disponível globalmente ou apenas na fase atual |
| **É dinâmico** | Sempre `true` ou trava a missão se não for verdadeiro na primeira vez |

## ⚙️ Funcionamento

Checa se há uma variável de ambiente respeita uma condição, e caso respeite/quando respeitar, prossegue a missão.

Bem auto explicativo, você pode definir uma variável no momento que quiser, através da [Ação de Fala de setar variável](./Acao_SETAR_VARIAVEL.md), e quando ela se tornar verdadeira para a comparação, a missão é concluída.