# Ação/RODA_FUNCAO
Uma ação que aciona uma **função** em um componente.

| Atributo | Informativo |
| -- | -- |
| **Script** | [AcaoRodaFuncao](../../../RPG/Assets/Scripts/AcaoCondicao/Acoes/AcaoRodaFuncao.cs) |
| **Nome** | Ação => RODA_FUNCAO|
| **Tipo** | Ação (Ação de Fala) |

  

| Parâmetro | Descrição |
| -- | -- |
| **Id do RefObject** |  O id definido no `RefObject` do objeto alvo |
| **Nome da função** | Nome da função a ser acionada (por mensagem)|


## ⚙️ Funcionamento
Você pode rodar uma função em fala ou na missão. Para isso, no Game Object que contém o componente com a função alvo, adicione um componente `RefObject` e nele defina um **id** único referente ao objeto. Referencie esse id no parâmetro **Id do RefObject**.

Note que não é possível passar parâmetros para a função, ela apenas será chamada.

Quando essa ação for tocada, a função será acionado imediatamente. Isso é útil especialmente em **Boss** para trocar de fase através de uma função.