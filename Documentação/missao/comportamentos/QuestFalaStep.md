# QuestFalaStep

Uma ação que começa um dialogo independente de onde esteja

| Atributo | Informativo |
| -- | -- |
| **Script** | QuestFalaStep |
| **Nome** | Começa dialogo |
| **Tipo** | Ação |
| **Parâmetro** | Nome da fala |

## ⚙️ Funcionamento

Escolha um nome para definir o dialogo, esse nome não terá nenhuma relevância e não será utilizado em nenhum momento, apenas para identificação (funcionamento interno do sistema). 

**ATENÇÃO: A ação só termina quando o dialogo é concluído.** Ou seja, enquanto o jogador não terminar o dialogo, a missão não progride.

## Alternativa
Como a descrição já diz, o comportamento começa um dialogo no momento que é ativo, e talvez isso não faça sentido para o seu caso. Um exemplo esperado pelo nome é que você poderia triggar uma conversa com um NPC, porém a realidade é que você vai triggar uma conversa e talvez o NPC possa estar perto, mas é diferente.

Uma alternativa para esse caso é usar uma [ação de ativar objeto](./QuestActivateStep.md) para ativar o NPC que você quer conversar, e este NPC ser dialogável. Depois, definir um passo de [condição de trigger](./QuestTriggerStep.md) aguardando um trigger e na fala do NPC utilizar a Ação de Fala "TRIGGAR_MISSAO" para ocorrer na última fala do personagem. Assim, você terá o mesmo comportamento, porém diretamente de uma conversa com o NPC.
