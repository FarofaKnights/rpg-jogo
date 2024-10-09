# Ação/PARAMETRO_ANIMATOR

Uma ação que altera o valor de algum parâmetro em um animator.

| Atributo | Informativo |
| -- | -- |
| **Script** | [AcaoParamsAnimator](../../../RPG/Assets/Scripts/AcaoCondicao/Acoes/AcaoParamsAnimator.cs) |
| **Nome** | Ação => PARAMETRO_ANIMATOR|
| **Tipo** | Ação (Ação de Fala) |

| Parâmetro | Descrição |
| -- | -- |
| **Id do AnimatorObject** | O id definido no `AnimatorObject` do animator alvo |
| **Nome do Parâmetro** | Nome do parâmetro a ser alterado|
| **Tipo** | Tipo do parâmetro (int, float ou bool, outro tipos são tratados como trigger) |
| **Valor** | Valor que o parâmetro irá assumir (em caso de Trigger, campo ignorado) |


## ⚙️ Funcionamento

Você pode definir em fala ou na missão o comportamento de um Animator. Para isso, no Game Object do Animator alvo, adicione um componente `AnimatorObject` e nele defina um **id** único referente ao Animator (não a missão atual). Referencie esse id no parâmetro **Id do AnimatorObject**.

No seu Animator Controller, defina um parâmetro que controle um determinado comportamento desejado e coloque seu nome nos parâmetros dessa ação, além do tipo e o valor desejado.

Quando essa ação for tocada, o parâmetro receberá o valor imediatamente, caso ele seja tratado como um trigger, também acontecerá no mesmo momento. Isso é útil especialmente em **Falas** para que o personagem se comporte de um jeito específico em uma determinada fala.