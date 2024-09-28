# Condição/SE_TEM_ITEM 

Uma condição que aciona quando um determinado item atinge uma determinada quantidade mínima no inventário.

| Atributo | Informativo |
| -- | -- |
| **Script** | [CondicaoTemItem](../../../RPG/Assets/Scripts/AcaoCondicao/Condicoes/CondicaoTemItem.cs) |
| **Nome** | Condição => SE_TEM_ITEM  |
| **Tipo** | Condição (Condição de Fala) |

| Parâmetro | Descrição |
| -- | -- |
| **Path do item** | O `id` do item |
| **Quantidade** | A quantidade mínima |
| **É dinâmico** | Sempre `true` ou trava a missão se não houver na primeira vez |

## ⚙️ Funcionamento

Checa se há um item em uma determinada quantidade mínima no inventário e se houver/quando haver, prossegue a missão. 

Pegue o `id` do item em questão indo diretamente no item localizado em alguma das pastas de `Resources/Itens` e clicando no botão "Copiar ID". Defina o `id` e a quantidade, que na maioria dos casos é 1.

Assim que o jogador tiver o que foi definido, a condição é concluída.
