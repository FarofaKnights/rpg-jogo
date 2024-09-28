# QuestDeactivateStep

Uma ação que desativa um GameObject

**Script:** QuestDeactivateStep
**Nome:** Desativar objeto
**Tipo:** Ação
**Parâmetro:** Id do componente `QuestObject`

## ⚙️ Funcionamento

Escolha o GameObject em cena que você deseja desativar e adicione o componente `QuestObject`. No componente, referencie o **QuestInfo** da missão atual e defina um **id** para o componente. Esse id é especifico apenas de uma missão especifica, então não se preocupe em repetir id entre missões.

O código checa todos os componentes `QuestObject` que referenciam o **QuestInfo** da missão atual e o **id** do parâmetro e os desativa, sendo assim, você pode definir múltiplos QuestObject com mesma missão/id e todos eles serão desativados pelo mesmo comportamento.
