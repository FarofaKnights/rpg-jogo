# Ação/TELEPORTA
Uma ação que teleportransporta o player para uma posição do mapa.

| Atributo | Informativo |
| -- | -- |
| **Script** | [AcaoTeleporta](../../../RPG/Assets/Scripts/AcaoCondicao/Acoes/AcaoTeleporta.cs) |
| **Nome** | Ação => TELEPORTA|
| **Tipo** | Ação (Ação de Fala) |

  

| Parâmetro | Descrição |
| -- | -- |
| **Id do RefObject** |  O id definido no `RefObject` do objeto alvo |


## ⚙️ Funcionamento
Crie um Game Object na posição que o Player será teletransportado, adicione um componente `RefObject` e nele defina um **id** único referente ao objeto. Referencie esse id no parâmetro **Id do RefObject**.

Quando essa ação for tocada,o player será teletransportado imediatamente.