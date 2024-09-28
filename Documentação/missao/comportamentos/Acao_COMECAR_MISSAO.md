# Ação/COMECAR_MISSAO

Uma ação que tenta começar uma missão.

| Atributo | Informativo |
| -- | -- |
| **Script** | AcaoComecaMissao |
| **Nome** | Ação => COMECAR_MISSAO |
| **Tipo** | Ação (Ação de Fala) |
| **Parâmetro** | O nome do arquivo `QuestInfo` da missão |

## ⚙️ Funcionamento
Você pode começar uma missão de algumas formas diferentes, veja mais sobre no [tutorial de começar uma missão](../comecar_missao.md). Esta é uma dessas formas.

Em suma, essa ação tenta iniciar a missão passada por parâmetro. Note que ela **tenta** mas pode não conseguir, caso os requisitos da missão não sejam cumpridos. Diferente do [comportamento de fala](./QuestFalaStep.md), essa ação não aguarda o jogador terminar algo, uma vez que isto seria uma condição e não uma ação propriamente dita. Portanto, caso a ação falhe em iniciar a missão, a missão atual continuará sem problemas.

Para resolver o problema apontado anteriormente, defina a condição da missão que você quer iniciar em um passo anterior a este, assim, quando o script tentar iniciar a missão, seus requisitos já terão sido cumpridos. Você também poderia tirar os requisitos da missão que você quer iniciar, uma vez que ela só será iniciada por esta ação.

Essa ação é uma Ação de Fala, assim como todas listadas nesta documentação com ´Acao_´ na frente. Por ser uma ação de fala, é uma ação que pode ser ativadas durante uma fala. Assim achamos mais utilidades deste componente, como iniciar uma missão após conversar com um NPC.
