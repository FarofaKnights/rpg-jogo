# Como começar uma missão?
Este documento parte do pressuposto que você já criou uma missão da forma correta. Caso este não seja o caso, recomendo consultar o tutorial de [como criar uma missão](./criar_missao.md).

Uma missão só pode ser iniciada caso seus requisitos sejam cumpridos. Os requisitos da missão são descritos no campo de `condicao` da própria. Enquanto o requisito não for cumprido, a missão não pode começar e qualquer tentativa de inicia-la não irá funcionar. Caso o requisito seja `NULL` ou a condição for cumprida, a missão irá para o estado de `podeComecar`.

Apenas no estado de `podeComecar` que uma missão poderá ser iniciada. Note que neste estado, ainda não começamos a missão, pois é necessário que alguma funcionalidade externa a ative. Atualmente há duas maneiras de ativar uma missão:

 - Ativar por um Trigger
 - Ativar pela Ação de Fala

## Ativando por um Trigger
É possível ativar uma missão quando o jogador entrar em um determinado trigger. Para isso temos que utilizar o componente `QuestPoint`. 

Escolha um lugar do seu mapa para colocar o seu trigger. Tenha em mente que este trigger só estará ativo se a missão se encontrar no estado `podeComecar` e permanecerá inativo para todos os outros estados.

No trigger, adicione o componente `QuestPoint` e referencie o `QuestInfo` referente a missão que ele pretende iniciar. 



Com isto, o sistema de iniciar uma missão já está pronto. Note que caso a missão apresente um requisito de inicio não cumprido, o trigger irá automaticamente se desativar, até que este seja cumprido. A mesma coisa vale para o momento que a missão iniciar e o trigger irá automaticamente se desativar.


## Ativando pela Ação de Fala
Talvez você precise que a missão comece após conversar com um NPC. O sistema atual de Fala permite que você defina uma ação para ocorrer quando determinada fala for impressa na tela. Esse sistema pode ser utilizado em outros lugares além da Fala, um exemplo disso é o QuestStep de Ação, que permite executar uma "ação de fala" como um passo da missão. 

Entre as ações de fala disponíveis há uma chamada `COMECAR_MISSAO`, que como o nome já diz, tem como objetivo começar uma missão. Veja mais sobre essa ação em sua [documentação](./comportamentos/Acao_COMECAR_MISSAO.md).

Essa ação requer um parâmetro `Id da Missão`, que é utilizado para referenciar a missão que a ação tentará começar. O `Id da Missão` é o nome do arquivo Scriptable do `QuestInfo`.

No momento, e só no momento, que esta ação for executada, caso a missão esteja no estado de `podeComecar`, ela irá ser iniciada. Note que chamar esta ação enquanto a missão ainda aguarda requerimentos irá resultar em nada.


