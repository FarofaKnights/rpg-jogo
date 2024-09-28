# Ação/TRIGGAR_MISSAO

Uma ação que aciona um **trigger** em outra missão.

**Script:** AcaoTriggaMissao
**Nome:** Ação => TRIGGAR_MISSAO
**Tipo:** Ação (Ação de Fala)
**Parâmetros:**
- **Id da missão:** O nome do arquivo `QuestInfo` da missão alvo
- **Nome do Trigger:** Nome do trigger a ser acionado

## ⚙️ Funcionamento
Algumas missões utilizam o comportamento de [aguardar por um trigger](./QuestTriggerStep.md), e este trigger pode ser acionado de mais de uma forma. Essa ação é uma dessas formas, evidentemente.

Nesse caso, você poderia utilizar esse comportamento para prosseguir com uma outra missão além da atual. Porém, o caso que é mais utilizado é dentro de uma Fala, e não como um comportamento de passo.

Essa ação é uma Ação de Fala, assim como todas listadas nesta documentação com ´Acao_´ na frente. Por ser uma ação de fala, é uma ação que pode ser ativadas durante uma fala. Assim achamos mais utilidades deste componente, como prosseguir uma missão após conversar com um NPC.
