En el siguiente proyecto desarrollé un videojuego de cartas similar al Gwent. 

Menú Inicial: 

Al iniciarse la aplicación se carga la primera escena, es decir, el menú principal en el cual puedes elegir entre cerrar la aplicación, ir al menú de opciones o inciar la partida. 

En el menú de opciones puedes elegir entre alternar entre pantalla completa y modo ventana; ajustar el volumen y regresar al menú principal. 

Escena de juego: 

- 1er Turno de cada ronda: 

Al pulsar play se carga la siguiente escena empezando así la partida. 

Empieza el jugador 1, el cual  debe robar 10 cartas para desbloquear el campo de juego, el botón de pasar y la posibilidad de cambiar hasta 2 cartas por otras del mazo arrastrandolas hasta el propio mazo. Luego puede jugar una carta o pasar de turno (la carta de líder no se desbloquea hasta el 2do turno de cada ronda). Al hacerlo se pasaría al turno del rival el cual hace lo mismo. 

En la 2da y 3ra ronda se procede de manera similar solo que esta vez se roban solo 2 cartas y no hay posibilidad de cambiarlas. 

- Contador: 

A la izquierda hay un contador de rondas, victorias y puntuación actual de cada jugador. 

- Visualizador: 

Entre el contador y el tablero se muestra una versión ampliada de la carta por la que pase el cursor donde se puede leer la descripción de cada una, sus puntos de ataque y tipo de carta. 

- Cartas Lider: 

A la izquierda del centro del tablero están las cartas líder de cada jugador, cada una tiene un efecto diferente: la del jugador 1 impide que el otro gane un punto en caso de empate; la del jugador 2 mantiene una carta entre rondas.
Solo se pueden activar una sola vez por partida y a partir del 2do turno de cada ronda. 

- Cartas Clima: 

Se colocan a la derecha del centro del tablero, arriba afecta a {M}, en el medio a {R} y abajo a {S}.
Afectan a la fila seleccionada en general diviendo su puntaje a la mitad sin tener en cuenta las cartas héroe. 

- Cartas Despeje: 

Simplemte se arrastran hasta la carta clima que desees anular descartándose ambas a sus respectivos cementerios. 

- Cartas aumentos: 

Se arrastran hasta la derecha de la fila que desees incrementar.
Aumentan x2 el ataque de cada carta en la fila seleccionada y se descartan automáticamente. (No afectan a los héroes) 

Cartas señuelos:
Se arrastran hasta la carta por la que desses sustituirla, al hacerlo se devuelve la carta seleccionada a la mano. 

- Cartas Unidad: 

Se arrastran hasta la fila en la que se desee colocar, si coincide con alguno de sus tipo de carta entonces se coloca, de lo contrario se devuelve a la mano.
Pueden tener hasta 5 efectos distintos (no todas tiene efecto)
Las cartas de unidad de tipo héroe no les afecta ninguna carta especial 

- Fin de la ronda
Una ronda finaliza cuando ambos jugadores le dan al botón de pasar. Se comparan sus puntuaciones y gana el que tenga la mayor. Si ambas puntuaciones coinciden ganan el punto de victoria ambos a no ser que esté activa el efecto de líder del jugador 1, de ser así solo gana el punto este jugador. 

- Fin de la partida 

Gana quien llegue primero a 2 victorias. 

- Menú de pausa 

Se puede acceder a él tanto pulsando el botón  que aparece en la esquina superior izquierda como pulsando la tecla "escape".
En él se puede cambiar el volumen, regresar a la partida o volver al menú principal (esto implica perder el progreso de la partida). 

- Botón de pase 

Aparece a la izquierda de la esquina inferior izquierda del tablero. Al pulsarse implica que el jugador no jugará más en ese turno y pasa al del siguiente. Si ambos lo pulsan dejan de jugar por la ronda. Si alguien ya ganó entonces sale un mensaje de felicitaciones para el respectivo jugador. 

- Mensaje de felicitaciones
Aparece al ganar algún jugador y al pulsar "ok" te devuelve al menú principal donde puedes darle a play y volver a jugar otra partida.
