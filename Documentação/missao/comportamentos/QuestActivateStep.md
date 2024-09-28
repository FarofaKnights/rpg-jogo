# QuestActivateStep

Uma ação que ativa um GameObject

| Atributo | Informativo |
| -- | -- |
| **Script** | [QuestActivateStep](../../../RPG/Assets/Resources/QuestSteps/QuestActivateStep.cs) |
| **Nome** | Ativar objeto |
| **Tipo** | Ação |
| **Parâmetro** | Id do componente `QuestObject` |

## ⚙️ Funcionamento

Escolha o GameObject em cena que você deseja ativar e adicione o componente `QuestObject`. No componente, referencie o **QuestInfo** da missão atual e defina um **id** para o componente. Esse id é especifico apenas de uma missão especifica, então não se preocupe em repetir id entre missões.

O código checa todos os componentes `QuestObject` que referenciam o **QuestInfo** da missão atual e o **id** do parâmetro e os ativa, sendo assim, você pode definir multiplos QuestObject com mesma missão/id e todos eles serão ativados pelo mesmo comportamento.
