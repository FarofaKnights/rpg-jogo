# Ação/TROCAR_CAMERA

Uma ação que troca a câmera ativa (ou retorna para a padrão)

| Atributo | Informativo |
| -- | -- |
| **Script** | [AcaoChangeCam](../../../RPG/Assets/Scripts/AcaoCondicao/Acoes/AcaoChangeCam.cs) |
| **Nome** | Ação => TROCAR_CAMERA|
| **Tipo** | Ação (Ação de Fala) |
| **Parâmetro** | O **id** da `CameraObject` (se vazio, câmera padrão) |

## ⚙️ Funcionamento

Você pode definir em fala ou na missão qual a câmera ativa. Para isso, no Game Object da Camera Virtual alvo (consultar Cinemachine), adicione um componente `CameraObject` e nele defina um **id** único referente a Camera (não a missão atual). Referencie esse id no parâmetro.

Quando essa ação for tocada, a prioridade da câmera virtual definida irá para **20** e permanecerá desse jeito até que outra ação (ou comportamento em jogo) mude isso. Recomendo que sempre termine retornando a câmera para a padrão (para não esquecer).