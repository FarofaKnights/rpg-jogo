# Ação/TRIGGAR_ANIMATOR

Uma ação que aciona um **trigger** em um animator.

| Atributo | Informativo |
| -- | -- |
| **Script** | [AcaoTriggaAnimator](../../../RPG/Assets/Scripts/AcaoCondicao/Acoes/AcaoTriggaAnimator.cs) |
| **Nome** | Ação => TRIGGAR_ANIMATOR|
| **Tipo** | Ação (Ação de Fala) |

  

| Parâmetro | Descrição |
| -- | -- |
| **Id  do AnimatorObject** |  O id definido no `AnimatorObject` do animator alvo |
| **Nome do Trigger** | Nome do trigger a ser acionado|


## ⚙️ Funcionamento
Você pode definir em fala ou na missão o comportamento de um Animator. Para isso, no Game Object do Animator alvo, adicione um componente `AnimatorObject` e nele defina um **id** único referente ao Animator (não a missão atual). Referencie esse id no parâmetro **Id do AnimatorObject**.

No seu Animator Controller, defina um trigger que controle um determinado comportamento desejado e  coloque seu nome nos parâmetros dessa ação.

Quando essa ação for tocada, o trigger será acionado imediatamente. Isso é útil especialmente em **Falas** para que o personagem se comporte de um jeito específico em uma determinada fala.