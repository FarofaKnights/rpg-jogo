# 🤔 Como criar uma missão?
Este documento tem o foco de ensinar de forma geral a como criar uma missão que estará disponível no jogo. Caso esteja tendo um problema com algum dos passos da missão, recomendo procurar a documentação especifica de cada um.

Uma missão é definida pelo ScriptableObject `QuestInfo`. Nele podemos definir informações básicas como nome e descrição, e também o próprio funcionamento da missão, como quais são os _passos_ da missão. Note que não definimos onde a missão se inicia no jogo, isto é tratado em [outro documento](./comecar_missao.md).

## 🫄 Criando o QuestInfo
Primeiro precisamos criar um ScriptableObject do tipo `QuestInfo` no projeto. Todas as missões que estão no jogo **devem** ser filhas diretas da pasta `Resources/Quests`, e qualquer QuestInfo fora da pasta será **ignorado** pelo sistema.

Para criar este ScriptableObject, entre na pasta desejada e clique com o botão direito. Um menu de opções deve aparecer, neste menu vá até `Create`, e então uma nova lista deve aparecer. No topo dessa lista você deve encontrar a opção `RPG`, lá é onde está localizado todos os ScriptableObjects exclusivos do projeto. Navegue até a opção `QuestInfo`e por fim clique nela para criar o objeto.

É importante **renomear o arquivo** com algo que identifique a missão, uma vez que o nome do arquivo será usado como `id` da missão, ou seja, não pode haver dois arquivos com o mesmo nome.

## 🧑‍🍼 Definindo valores básicos
Com o ScriptableObject recém criado, temos que definir as informações básicas da nossa missão.

Note que no menu do inspetor, a primeira propriedade é um checkbox com o label `Use Custom Editor`. Essa opção é utilizada apenas para debug do objeto cru (ou seja, sem os fru-frus do editor customizado) e deve ser ignorada na maioria dos casos.

O **nome da missão** é utilizado para identifica-la na lista de missões presente no menu do jogador, portanto deve ser um nome **sucinto** e **legível** que identifique bem a missão.

A **descrição da missão** só aparece ao selecionar a missão na lista de missões do menu do jogador, e tem como objetivo **contextualizar** o jogador sobre o que é a missão. Considere que um jogador pode estranhar de onde veio uma missão e a descrição pode **clarificar** sua duvida. Por exemplo: "Na fogueira, a caçadora disse que precisava conversar comigo em um local reservado. O que será que ela quer?"

A **condição da missão** pode ser vista abaixo de uma separação "Quest Requirements". Este campo define as condições necessárias para que uma missão esteja disponível. Caso seu valor seja `NULL` a missão já começa disponível. Clicando no seletor, uma lista de possiveis condições é exibida, essas são as mesmas condições utilizadas no sistema de Falas. Lembre de **sempre** definir o valor `É dinamico` como verdadeiro, isso garante que no momento que a condição se tornar verdadeira a missão estará disponível.

Uma vez que uma missão se torna disponível, ela não pode voltar a ficar indisponível, leve isso em consideração quando montar missões. Lembrando que, estar disponível não significa que a missão foi iniciada. Para definir onde a missão se inicia, veja o [tutorial de iniciar missão](./comecar_missao.md).

## 🚶 Definindo passos
Passos constituem o que é a missão na prática. A missão possui uma lista ordenada de passos, estes passos podem ser **condições**, ou seja, objetivos a serem cumpridos pelo jogador, ou **ações** que ocorrem durante o progresso da missão. Sempre que um passo é concluído, ele prossegue para o passo após ele. Nos casos de **condições**, o passo só será concluído quando a condição for cumprida. Já no caso de **ações**, no momento que entrar no passo, a ação definida será executada e logo em seguida irá prosseguir para o próximo passo.

Inicialmente começamos com uma lista vazia de passos. Podemos adicionar passos novos a lista através do botão de [+] localizado a direita do campo. Um passo possui alguns atributos a serem notados:

O **informativo** é o atributo utilizado para marcar textualmente o progresso da missão. Quando o jogador estiver neste passo, o texto irá aparecer no canto superior direito da tela, **indicando o que** o jogador **deve fazer**. O jogador também pode ver uma lista de todos os informativos anteriores ao passo atual através da lista de missões localizada no menu. Assim, podemos notar que informativos fazem mais sentido quando o passo é uma **condição**.

Temos o atributo **tipo**, presente apenas em passos diretamente filhos da lista principal. Por padrão seu valor é `PADRAO`, definindo que é apenas um passo normal. Você pode mudar o valor do atributo para `PAI` e isto irá definir que o passo é apenas um grupo de passos.

Passos do tipo `PAI` servem unicamente para organização da lista de passos, uma vez que na prática, ao chegar no passo do tipo `PAI`, o sistema irá percorrer os passos filhos e quando terminar, seguirá para o próximo passo. Ou seja,  sua utilização é totalmente opicional uma vez que **não há implicações mecânicas**. É recomendado que a maioria dos passos localizados na lista principal sejam do tipo `PAI`, e a lógica da missão, ou seja, os passos `PADRAO`, esteja distribuída como filhos destes grupos. Organize da forma que faça mais sentido e que deixe sua lista de passos **fácil de navegar**.

Por fim, temos o atributo **comportamento**. Este atributo define o comportamento do passo. Ao selecionar uma das opções de valores para este atributo, parâmetro adicionais serão exibidos para configurar seu comportamento. O funcionamento de cada um desses componentes está documentado, e listado corretamente no documento relativo a [passos](./passos.md).




