# Gameplay loop
De maneira geral, o gameplay loop geral do jogo é: 
- O jogador, por meio de um hub interativo, escolhe um mundo, e uma missão dentro do mundo.
- Ao entrar no mundo, vários elementos do nível estarão ajustados conforme a missão escolhida (posicionamento de inimigos, plataformas, etc).
- O jogador completa a missão, que pode ter vários objetivos (chegar no final, coletar x objetos, etc).
- Durante essas missões, o jogador pode também coletar chaves especiais que desbloqueiam mais mundos dentro da hub.
= Após completar a missão, o jogador retorna para a hub.
# Mecânicas principais
## Mecânicas do Jogador
O jogador tem acesso a algumas mecânicas principais durante todo o jogo, já que elas estão atreladas ao protagonista, e não ao mundo.
- **Movimento:**  O jogador pode se movimentar em qualquer direção usando o joystick do controle ou em oito direções usando o teclado. Como o jogador tem total controle sobre a câmera, ele pode utilizá-la para se mover em qualquer direção com o teclado também. 
- **Pulo:** O jogador tamnbém tem um pulo, que é necessário para alcançar plataformas, atacar inimigos e desviar de projéteis.
- **Cabeçada:** Ao apertar o botão de pulo enquanto estiver no ar, o jogador ativa um rápido dash, que dá um pouco mais de alcance horizontal e também ataca certos tipos de inimigos específicos.
- **Gancho & Corda:** A principal mecânica diferenciada do jogo. Em diversos pontos em cada mundo, o jogador encontrará alvos. que podem ser utilizados como um ponto onde ele pode prender uma corda e se balançar sobre. Isso permite que o jogador alcance distâncias maiores que o alcance de seu jump+dash, principalmente se passar por vários alvos em sequência. Enquanto estiver pendurado, o jogador também pode subir e descer com a corda, dando a ele maior mobilidade vertical.
## Mecânicas em Mundos
Cada mundo do jogo possui uma variedade de mecânicas atreladas a ele. Embora algumas delas aparecam em quase todos os mundos, outras são específicas de cada mundo, de modo a trazer maior variedade de gameplay, mas mantendo uma base familiar para o jogador.
- **Botões:** Encontrados em todos os mundos, os botões são elementos básicos capazes de ativar ou desativar outros objetos no mundo quando apertados. Embora façam nada por si, estão sempre conectados com outras mecânicas. Botões no chão podem ser pressionados ao pular neles, enquanto que botões no ar precisam ser ativados com o gancho.
- **Ventiladores:** Ao serem ativados, criam uma corrente de ar constante na direção em que estão posicionados. Essa corrente então empurra o jogador, inimigos ou outros objetos, podendo atrapalhar ou ajudar sua progressão no mundo. 
- **Plataformas móveis:** Algumas plataformas se movem automaticamente, criando maior dificuldade ao cruzar pelo mundo. Outras plataformas podem ter sua posição escolhida pelo jogador, dentro de limitadas opções. Isso proporciona um elemento de puzzle para certas missões.
- **Jardins de flores:** Pequenos campos de flores espalhados pelo mundo. Apesar de parecerem decorativos, eles podem esconder coletáveis, incentivando a exploração de todos os cantos dos mundos.
## Coletáveis
- **Frutas:** Frutas são os coletáveis mais comuns do jogo. Estão presentes em abundância em todos os mundos, e sua função principal é de aumentar a pontuação do jogador. Algumas missões específicas podem também exigir que o jogador colete um número mínimo de frutas.
- **Chaves:** As chaves são um tipo de coletável secundário. Cada mundo possui um número específico de chaves escondidas, e elas podem ser utilizadas para desbloquear os próximos mundos.
- **"Coletável final":** (WIP) O coletável final é o objetivo principal de todas as missões. Ao serem coletados, eles desbloqueiam a missão seguinte dentro de um mundo.
## Inimigos
Os inimigos em geral não possuem muita complexidade. Seu uso primario é como obstáculos encontrados pelo jogador.
- **"Minion básico":** (WIP) um inimigo simples que percorre um caminho pré-determinado. Pode ser derrotado com um pulo ou uma cabeçada.
- **"Minion plataforma":** (WIP) um inimigo grande, que não pode ser derrotado. O jogador pode atrai-lo para lugares específicos para então usá-lo como plataforma.
- **"Pássaro drone":** (WIP) um inimigo que voa em um caminho pré-determinado. Por baixo, pode ser utilizado como um alvo para o gancho.
# Sistemas principais
## Sistema de mundos e níveis
- **Mundos:** O jogo é divido em mundos que têm temáticas diferentes tanto visualmente como mecanicamente, cada um pode ter elementos presentes em outros mundo mas contém usos únicos de mecânicas e assets visuais, além de missões diferentes.
- **Missões:** Cada mundo tem várias missões, cada missão explora uma parte diferente do mesmo mundo.
- **Hub:** A escolha de mundo e missão é feita por um hub central e navegável, inspirado no castelo da peach de Super Mario 64.
#Interface
## Interface do jogador
- **HUD:** A Hud é minimalista para mostrar mais o jogo e evitar informações excessivas ao jogador, a interface in-game mostrará a vida e o progresso das missão selecionada.
- **Menus:** Os menus contarão com opções de acessibilidade para melhorar a experiência do jogador e aumentar o público para o game.
