# 🏃 Passos e como usar

Este arquivo foi feito para explicar o funcionamento de passos no sistema de missão. É esperado que antes de mexer nos passos, você saiba como [criar uma missão](./criar_missao.md) e também como definir o que [começa a missão](./comecar_missao.md).

Passos são exatamente o que define o comportamento da missão do momento de você a inicia até o momento que termina. Eles podem ser **ações** ou **condições**, onde *ações* são apenas efeitos que ocorrem em determinado momento da missão e *condições* são requisitos que impedem de prosseguir a missão até que sejam cumpridos.

Passos são localizados em uma lista no Scriptable de `QuestInfo`, podendo ser um passo *padrão*  ou *pai*. Mais sobre isso é descrito no tópico "Definindo passos" do tutorial de [criar uma missão](./criar_missao.md). 

## 🦶 O comportamento de um passo

Um passo possui o atributo `comportamento` que define qual vai ser seu comportamento, obviamente. Há uma lista de comportamentos que você pode usar e eles são separados entre *ação* e *condição*. Cada comportamento tem seus parâmetros específicos e funcionam de formas diferentes, sendo assim, listamos e documentos o funcionamento de todos os comportamentos predefinidos:

## Condições
|Nome|Descrição|
|--|--|
| [Trigger](./comportamentos/QuestTriggerStep.md) | Aguarda um trigger ser ativado (por script) |
| [Grupo de inimigos](./comportamentos/QuestGrupoInimigosStep.md) | Matar um grupo de inimigos |
| [Esperar segundos](./comportamentos/QuestWaitSecondsStep.md) | Aguarda tantos segundos |
| [Condição/SE_TEM_ITEM](./comportamentos/Condicao_SE_TEM_ITEM.md) | *Condição de Fala* que compara a quantidade de um determinado item no inventário |
| [Condição/SE_VARIAVEL](./comportamentos/Condicao_SE_VARIAVEL.md) | *Condição de Fala* que compara valores de variáveis de ambiente |

## Ações
|Nome|Descrição|
|--|--|
| [Ativar objeto](./comportamentos/QuestActivateStep.md) | Ativa um GameObject |
| [Desativar objeto](./comportamentos/QuestDeactivateStep.md) | Desativa um GameObject |
| [Começa dialogo](./comportamentos/QuestFalaStep.md) | Começa um dialogo independente de onde esteja |
| [Ação/SETAR_VARIAVEL](./comportamentos/Acao_SETAR_VARIAVEL.md) | *Ação de Fala* que define um valor a uma variável de ambiente |
| [Ação/ADICIONAR_ITEM](./comportamentos/Acao_ADICIONAR_ITEM.md) | *Ação de Fala* que adiciona um item no inventário |
| [Ação/REMOVER_ITEM](./comportamentos/Acao_REMOVER_ITEM.md) | *Ação de Fala* que remove um item no inventário |
| [Ação/TRIGGAR_MISSAO](./comportamentos/Acao_TRIGGAR_MISSAO.md) | *Ação de Fala* que ativa um trigger em uma missão |
| [Ação/COMECAR_MISSAO](./comportamentos/Acao_COMECAR_MISSAO.md) | *Ação de Fala* que tenta iniciar uma missão |
| [Ação/TRIGGAR_ANIMATOR](./comportamentos/Acao_TRIGGAR_ANIMATOR.md) | *Ação de Fala* que ativa um trigger em um animator |
| [Ação/PARAMETRO_ANIMATOR](./comportamentos/Acao_PARAMETRO_ANIMATOR.md) | *Ação de Fala* que seta o valor de um parâmetro em um animator |
| [Ação/TROCAR_CAMERA](./comportamentos/Acao_TROCAR_CAMERA.md) | *Ação de Fala* que troca a câmera ativa (ou retorna para a padrão) |
| [Ação/RODA_FUNCAO](./comportamentos/Acao_RODA_FUNCAO.md) | *Ação de Fala* que roda uma função em um componente |
| [Ação/TELEPORTA](./comportamentos/Acao_TELEPORTA.md) | *Ação de Fala* que teletransporta o jogador |


## 🤓 Comportamentos customizados
Um comportamento, na prática, é definido por um prefab que é instanciado no momento que um passo é executado e destruído quando o passo é finalizado. Sendo assim você pode criar seus próprios comportamentos e definir diretamente no passo da missão. O que acontece com os passos predefinidos é que eles estão setados na pasta `Resources/QuestSteps`, fazendo que apareçam listados para serem utilizados facilmente. 

O beneficio de comportamentos predefinidos é que você pode criar um efeito mais genéricos e definir *parâmetros* para customizar este efeito. Sendo assim, utilizamos comportamentos customizados **apenas** em caso específicos, e se for algo que será utilizado mais vezes, é melhor definir um novo comportamento predefinido.



