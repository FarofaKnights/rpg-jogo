# QuestGrupoInimigosStep

Uma condição que espera todos os inimigos de um grupo (definido por `EnemyGroup`) terem morrido.

| Atributo | Informativo |
| -- | -- |
| **Script** | [QuestGrupoInimigosStep](../../../RPG/Assets/Resources/QuestSteps/QuestGrupoInimigosStep.cs) |
| **Nome** | Grupo de inimigos |
| **Tipo** | Condição |

| Parâmetro | Descrição |
| -- | -- |
| **EnemyGroup Group ID** | ID do `EnemyGroup` |
| **Show Quantity** | Boleano que define se deve mostrar o progresso no informativo |

## ⚙️ Funcionamento

Para que o comportamento funcione, você primeiro precisa ter um grupo de inimigos definido em sua cena. Para isso, coloque um componente `EnemyGroup` em algum GameObject que não seja os inimigos (recomendo ser o pai dos inimigos). Nesse componente, referencie todos os inimigos que fazem parte do grupo e adicione um ID para o grupo. Mantenha atenção em não usar IDs repetidos, uma vez que o grupo não é exclusivo de uma missão e pode entrar em conflito com outra se o ID for o mesmo.

O grupo e os inimigos vão permanecer em seu estado e não haverá nenhuma alteração, caso você queira que eles só apareçam na hora que o passo começar, é melhor definir um `QuestObject` e um passo de ativação antes deste.

Recomendo também colocar o `SaveIntegridade` em cada inimigo para que eles não apareçam de novo quando reloadar a cena. Não se preocupe, utilizar isso não causa conflito com o progresso da missão.

Você também pode mostrar o progresso da missão (quantos inimigos faltam matar) através do *Show Quantity*, porém esta informação só aparecerá na indicação do HUD, e não na listagem do Menu de Missões.

