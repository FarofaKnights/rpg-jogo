# QuestTriggerStep

Uma condição aguarda um trigger ser ativado

| Atributo | Informativo |
| -- | -- |
| **Script** | QuestTriggerStep |
| **Nome** | Trigger  |
| **Tipo** | Condição |

| Parâmetro | Descrição |
| -- | -- |
| **Nome do Trigger** | Nome do trigger aguardado |
| **Objeto do Trigger (opcional)** | ID do `QuestObject` que aparecerá apenas enquanto o trigger não foi ativo |

## ⚙️ Funcionamento

Há duas formas de triggar este comportamento. 

A primeira maneira, e forma original, é através de um trigger na cena com o componente `ActivateQuestTrigger`. Este componente tem que estar ligado a um objeto com Collider setado como Trigger. No componente, referencie o `QuestInfo` da missão atual e o trigger a ser ativado na missão. Não esqueça de conferir se *ActivateOnTriggerEnter*  está ativo. Se você quiser, você também pode colocar um componente `QuestObject` nesse trigger e referencia-lo no parâmetro opcional *Objeto do Trigger*, isso vai garantir que ele será ativo quando o comportamento começar e será desativado quando o jogador entrar no trigger (recomendado).

A segunda maneira é através de uma [Ação de Fala](./Acao_TRIGGAR_MISSAO.md).
