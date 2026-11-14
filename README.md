# Recuperatorio TP N° 2 — Programación de Videojuegos I

Prototipo 3D desarrollado en Unity para el Recuperatorio del Trabajo Práctico N° 2 de la cátedra **Programación de Videojuegos I** (TUDIVJ — Facultad de Ingeniería, UNJu).

---

## Datos del Estudiante

- **Apellido y Nombre: Ramos Luca Joaquin
- **LU / Legajo:  TUV000403
- **Cátedra:** Programación de Videojuegos I (Plan 2025)
- **Versión de Unity:** Unity 6000.3.27f1 LTS
- **Repositorio:** https://github.com/lukjoako/RamosLuca_TP2_Recuperatorio

---

## Objetivo y Gameplay

El objetivo del juego es recorrer el escenario 3D para **recolectar ítems de datos** y **activar terminales/puertas interactivas** a distancia (mediante Raycast frontal con la tecla `E`) hasta alcanzar el puntaje necesario para obtener la **Victoria** (`Victory`), evitando perder toda la salud ante los drones centinelas (`GameOver`).

Durante la partida, el jugador debe administrar su supervivencia frente a la patrulla y persecución de la IA generada por el **Spawner** (con un máximo de 2 enemigos activos en el mapa):
- Si recibe daño, puede acercarse a la **Estación de Curación** para recuperar vida de forma progresiva antes de que entre en tiempo de recarga.
- Puede recoger el **Power-Up de Invisibilidad** para volverse temporalmente indetectable ante los drones y atravesar la zona vigilada.
- Debe tener cuidado con las **Zonas de Barro**, ya que reducen su velocidad de desplazamiento mientras permanece dentro de ellas.
- el jugador puede abrir la puerta con 10 puntos pero para obtener la victoria hace falta recolectar todos los datos repartidos por el mapa

<img width="635" height="316" alt="gameplay 2 pvj" src="https://github.com/user-attachments/assets/8f0579ed-f03c-4525-912d-0401395ca253" />


---

## Controles

| Tecla | Acción |
| :--- | :--- |
| `W` / `S` o `↑` / `↓` | Avanzar y retroceder |
| `A` / `D` o `←` / `→` | Rotar al personaje sobre su eje |
| `Espacio` | Saltar (validado con chequeo de suelo por Raycast) |
| `E` | Interactuar a distancia con puertas/terminales (Raycast frontal) |
| `P` o `Escape` | Pausar / Reanudar partida (`Time.timeScale`) |
| `R` | Reiniciar partida al finalizar (`Victory` / `GameOver`) |

---

## Arquitectura y Correcciones Aplicadas

En esta versión de recuperatorio se reestructuró el proyecto desde cero aplicando una arquitectura modular y desacoplada:

- **Desacoplamiento del `GameManager`:** El `GameManager` funciona como Singleton independiente que administra los estados `Start`, `Playing`, `Paused`, `Victory` y `GameOver`, escuchando los eventos `OnHealthChanged` y `OnPlayerDied` de `PlayerHealth` sin sobrecargar de lógica al jugador.
- **Detección e Interacción por Raycast y `LayerMask`:**
  - `PlayerController` proyecta un rayo desde `groundCheck` hacia abajo filtrando únicamente la capa `Ground` para validar el salto limpio.
  - `PlayerInteraction` proyecta un rayo frontal desde `RayOrigin` al presionar `E`, filtrando únicamente la capa `Interactable` mediante `IInteractable`, evitando cualquier autodaño al jugador.
- **Corrección de Cámara y Nuevo Input System:** Se migraron las lecturas de teclado al paquete `UnityEngine.InputSystem` (`Keyboard.current`) y se desacopló la rotación de `CameraFollow` (usando `Vector3.SmoothDamp` + `yawRotation`) respecto al movimiento del jugador para evitar giros circulares.
- **Sistema de Salud e Inmunidad:** `PlayerHealth` implementa un tiempo de inmunidad (`immunityDuration`) entre ataques para impedir que el contacto con el enemigo consuma toda la vida en un solo frame.
- **Escenario con Terrain Tool y NavMesh:** Terreno modelado con desniveles, rampas, textura aplicada mediante *Terrain Layer*, árboles y rocas con colisionadores integrados al `NavMeshSurface` para la navegación del `EnemySentinel`.

---

## Requisito Extra: Estación de Curación (`HealingStation.cs`)

Plataforma 3D identificable mediante iluminación (`Point Light`) y sistema de partículas (`ParticleSystem`) con un `Collider` en modo *Trigger*:

1. **Curación progresiva:** Al permanecer un jugador herido dentro de la zona, restaura `+10 HP` cada `1.5` segundos (no cura el 100% de forma instantánea).
2. **Límite de carga y Cooldown:** Posee una carga máxima de `50 HP` por ciclo. Al agotarse, entra en un estado deshabilitado durante `10` segundos antes de recargarse.
3. **Retroalimentación:** Durante el enfriamiento se apagan la luz y las partículas de la estación y se emiten mensajes de estado en la consola (`"Estación en Recarga"` / `"Estación recargada y lista"`).

---

## Requisitos Opcionales Implementados (3)

### 1. Opción B — Power-Up de Invisibilidad (`InvisibilityPowerUp.cs`)
Ítem recolectable rotatorio que al ser recogido por el jugador ejecuta `PlayerHealth.ActivateInvisibility(duration)`. Mientras `IsInvisible` es `true`, el script `EnemySentinel` ignora al jugador en su chequeo de detección y continúa su patrulla normal hasta que finaliza el temporizador.

### 2. Opción D — Zona de Desaceleración / Barro (`SlowZone.cs`)
Área delimitada con un `Collider` en modo *Trigger* que reduce el multiplicador de velocidad del jugador (`PlayerController.SetSpeedMultiplier(0.4f)`) en `OnTriggerEnter` y lo restablece a `1.0f` al salir en `OnTriggerExit`.

### 3. Opción E — Spawner Temporizado de Enemigos (`EnemySpawner.cs`)
Generador temporizado que verifica cada `5` segundos la cantidad total de instancias activas de `EnemySentinel` en la escena. Si hay menos de **2 enemigos** en juego, busca una posición válida sobre la malla de navegación mediante `NavMesh.SamplePosition` e instancia un nuevo dron hasta alcanzar el máximo permitido.

---

## Capturas de Ejecución

aqui muestro como el enemigo sigue la ultima ubicacion del player, aunque si es constante, para que el jugador deba perderlo por los arboles y las rocas

<img width="1366" height="768" alt="para el readme" src="https://github.com/user-attachments/assets/4f4d86d4-2394-4566-9541-b444ab0f3cb6" />

gameplay del juego 

<img width="633" height="249" alt="gameplay pvj" src="https://github.com/user-attachments/assets/7b2ed2dc-e6a4-423d-bd6c-8893f177920f" />

## Correcciones y Notas

Me falto hacer un mejor mapa
los unicos enemigo tienen su ruta ya predefinidas y cuando ya no ven al jugador ignoran las colisiones del mapa
esta implementado que si se destruye los drones puedan aparecer mas (aunque no hay forma de destruirlo)
la puerta esta integrada pero de una forma mala en todo el mapa
las rocas no estaban integradas en el mapa en el terrain ya que me aparecia que el navmesh lo tomaba y no iba a poder cumplir con mi vision del juego de chocarse con las rocas para perder al enemigo
