# 📚 Backlog
Arquivo reservado para listar tudo que precisamos arrumar eventualmente mas não é importante o suficiente, ou não qualifica, para entrar na **Planilha™**.

Sempre que alguém lembrar de algo, seja erro, seja observação, é aconselhável que seja anotado e commitado neste arquivo. Assim que corrigido/implementado, podemos remover o campo da lista. 

## 🥵 Urgente
Prioridade em implementação (mais que os itens da planilha)

- Mudar como as informações são apresentadas no tutorial (UI) 

## 😥 Alta
Deve ser implementado assim que possível 

Não tem nada aqui UwU

## 😑 Média
Tem que ser implementado até o fim do projeto mas não precisa ser agora

- Melhorar a tela de créditos (mais créditos para o nosso amigo lucas sabino pfv 🙏, gerente de produção de assets, diretor de animação, gerente de arte, personagens, texturas, mais bonito do grupo, melhores piadas, mais alto.)
- Fazer a tragetória do tijolo respeitar as leis da física de um lançamento
- Inimigos vão atrás do player quando levam dano
- Colocar itens secretos na industria (?)

## 😌 Baixa
Fará falta se não for implementado mas pode não ser
- Fazer configuração de gráficos (gama, etc)
- Melhorar visualização da vida do inimigo
- Selecionar um consumível com hotkey
- Indicador de localização no mapa


## 🤠 Nenhuma
Algo adicional que não fará falta se não for implementado, mas seria legal ter (como game juices e afins)

- Armas e braços novos do player
- Knockback de dano (no player e do player)

# Anotação de bugs 🐜
Bugs que eu estou encontrando enquanto jogo o jogo, vou anotando eles aqui e usando o sistema de prioridade do Juan para indicar quais bugs são mais quebrados do que outros e afetam mais a experiencia do jogador.

## 🤯 Urgente
Bugs que afetam o progresso do jogador e o impedem de continuar o jogo, tem que ser corrigidos imediatamente.

- Os elevadores da fase do predio demoram pra funciomar
- Portas na fase do henri não abrem


## 😣 Alta
Bugs que atrapalham o progresso, mas não necessariamente o impedem de jogar ou terminar o jogo.

- Não tem como coletar os itens de chave e força, fora do tutorial
- No fim da fase da industria, logo antes do save point do boss, os alvos não ativam a animação de abrir o portão. Para que o jogador pelo possa progredir no jogo, coloquei uma função que faz o potão desaparecer, mas fica feio pra caramba.
- Refazer o bake no predio, pq os inimigos podem atravessar paredes
- O tiro sempre mira pro chão, seria bom se a camera tivesse um limite do quanto pra baixo ela pode olhar

## 🤔 Media
Bugs que afetam a experiencia em geral e fazem o jogo ser pior.
- Na fase da industria, todos os colisões que se mexem não funcionam, isso inclue o balde no segundo cano puzzle, que deveria poder derrubar o jogador do cano e dificultar ele acertar os canos.
- Na represa (antes de cruzar o rio) na fase da cidade, tem um carro com lod invertido, ele desaparece assim que chega perto e fica visivel de longe
- Robo fornalha demora um tempo pra voltar pro estado de ataque, e fica uns frames esquisitão em t-pose dps q toma dano.

## 😕 Baixa
Similar ao medio, mas com prioridade menor.
- Tem um robo tijolo na cidade (coordenada -79.23247, 7.14317, 76.2052) perto da caçadora de ciborgues que ficam afundando e subindo no chão o tempo todo, ele sobe quanto toma dano. Tem outro na vendinha do vendedor que também ta com esse bug
- Na fase cidade, as fogueiras tão com um bloom explosivo, ta muito brilhante. Mas esse bloom so aparece de um angulo


## 🥱nenhuma
Bugs que seriam melhor corrigidos, mas não atrapalham o jogo.

Não tem nada aqui UwU