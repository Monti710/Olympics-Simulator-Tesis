using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class SceneMan : MonoBehaviour
{
    [Header("Time")]
    public float timeLimit = 60f;
    private float timeRemaining;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI timeText1; // <-- ASEGÚRATE DE ASIGNAR ESTO

    [Header("Shots")]
    public int maxShots = 20;
    public Shooter shooter;
    public GameObject shotIconPrefab;
    public Transform shotsPanel;
    public Transform shotsPanel1; // <-- ASEGÚRATE DE ASIGNAR ESTO

    public TextMeshProUGUI shotsText;
    public TextMeshProUGUI shotsText1; // <-- ASEGÚRATE DE ASIGNAR ESTO

    public float iconSpacing = 30f;

    public int maxIconsPerRow = 10;
    public float lineSpacing = 40f;

    [Header("Points")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreText1; // <-- ASEGÚRATE DE ASIGNAR ESTO

    public PointCounter pointCounter;

    // =========================================================
    // NUEVAS VARIABLES PARA EL MENSAJE DE GAME OVER
    // =========================================================
    [Header("Game Over Screen")]
    public GameObject gameOverPanel; // Panel que se activa (Debe estar DESACTIVADO en el Inspector al inicio)
    public Text gameOverText; // TextMeshPro para el mensaje
    public float messageDisplayDuration = 5f; // Duración del mensaje antes de cargar la escena
    [TextArea(1, 3)]
    public string outOfTimeMessage = "¡Se acabó el tiempo!"; // Mensaje configurable
    [TextArea(1, 3)]
    public string outOfShotsMessage = "¡Te quedaste sin balas!"; // Mensaje configurable
    [TextArea(1, 3)]
    public string bothMessages = "¡Se acabó el tiempo y las balas!"; // Mensaje configurable

    // Bandera para asegurar que la lógica de Game Over solo se ejecute una vez
    private bool isGameOverHandled = false;

    // Se añade una variable para registrar si las balas ya se acabaron
    private bool shotsHaveRunOut = false;
    // =========================================================

    [Header("Scene Loader")]
    public string sceneToLoad;

    [Header("Scene Delay")]
    public float delayBeforeSceneLoad = 2f;

    void Start()
    {
        timeRemaining = timeLimit;
        shooter.SetMaxShots(maxShots);
        UpdateShotDisplay(maxShots);
        shotsHaveRunOut = false; // Reiniciar estado

        // NUEVO: Asegúrate de que el panel de Game Over esté oculto al inicio
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            timeRemaining = 0;
            // Si el tiempo se acaba, verificamos ambas condiciones (balas y tiempo)
            HandleGameOver(shotsHaveRunOut, true);
        }

        UpdateTimeDisplay();
        UpdateScoreText();
    }

    void UpdateTimeDisplay()
    {
        timeText.text = Mathf.Floor(timeRemaining / 60).ToString("00") + ":" + Mathf.Floor(timeRemaining % 60).ToString("00");

        if (timeText1 != null)
        {
            timeText1.text = Mathf.Floor(timeRemaining / 60).ToString("00") + ":" + Mathf.Floor(timeRemaining % 60).ToString("00");
        }
    }

    public void UpdateShotDisplay(int currentShots)
    {
        foreach (Transform child in shotsPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in shotsPanel1)
        {
            Destroy(child.gameObject);
        }

        shotsText.text = "Balas: " + currentShots.ToString();

        // Actualización para shotsText1
        if (shotsText1 != null)
        {
            shotsText1.text = "Balas: " + currentShots.ToString();
        }

        float yOffset = 0f;

        for (int i = 0; i < currentShots; i++)
        {
            GameObject shotIcon = Instantiate(shotIconPrefab, shotsPanel);
            GameObject shotIcon1 = Instantiate(shotIconPrefab, shotsPanel1);

            float xOffset = (i % maxIconsPerRow) * iconSpacing;
            if (i % maxIconsPerRow == 0 && i != 0)
            {
                yOffset -= lineSpacing;
            }
            shotIcon.transform.localPosition = new Vector3(xOffset, yOffset, 0);
            shotIcon1.transform.localPosition = new Vector3(xOffset, yOffset, 0);
        }

        if (currentShots == 0)
        {
            shotsHaveRunOut = true;
            // Si las balas llegan a cero, verificamos si el tiempo ya acabó.
            // Si el tiempo ya se acabó (o está a punto), HandleGameOver lo manejará.
            // Si las balas se acaban PRIMERO, llamamos a HandleGameOver.
            if (timeRemaining > 0)
            {
                HandleGameOver(true, false);
            }
            else
            {
                // Si las balas se acaban justo cuando el tiempo se acaba, 
                // HandleGameOver ya fue llamado por Update, pero por si acaso.
                HandleGameOver(true, true);
            }
        }
    }

    public void UpdateScoreText()
    {
        if (scoreText != null && pointCounter != null)
        {
            scoreText.text = "Puntos: " + pointCounter.GetPoints();

            if (scoreText1 != null)
            {
                scoreText1.text = "Puntos: " + pointCounter.GetPoints();
            }
        }
    }

    // =========================================================
    // MÉTODOS PARA MANEJAR EL FIN DE JUEGO (GAME OVER)
    // =========================================================

    // Recibe los estados de fin de juego desde el punto que lo llama.
    private void HandleGameOver(bool shotsAreOver, bool timeIsOver)
    {
        // Si el Game Over ya se está procesando, salimos.
        if (isGameOverHandled) return;

        // Solo procedemos si al menos una condición de fin de juego es verdadera
        if (shotsAreOver || timeIsOver)
        {
            isGameOverHandled = true; // Marcamos que el Game Over ha comenzado

            string message;

            // Lógica para determinar el mensaje correcto
            if (timeIsOver && shotsAreOver)
            {
                message = bothMessages;
            }
            else if (timeIsOver)
            {
                message = outOfTimeMessage;
            }
            else // if (shotsAreOver)
            {
                message = outOfShotsMessage;
            }

            // Llamamos a la corrutina que mostrará el mensaje y luego cargará la escena
            StartCoroutine(ShowGameOverMessage(message));
        }
    }

    private IEnumerator ShowGameOverMessage(string message)
    {
        // 1. Mostrar el panel y el texto
        if (gameOverPanel != null && gameOverText != null)
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = message;
        }

        // 2. Esperar el tiempo de visualización configurable
        yield return new WaitForSeconds(messageDisplayDuration);

        // 3. Cargar la escena (integrando el delayBeforeSceneLoad original)
        // Usaremos el delayBeforeSceneLoad original para el tiempo que tarda la escena en cargarse.
        yield return new WaitForSeconds(delayBeforeSceneLoad);

        // 4. Cargar la escena
        LoadScene();
    }

    // =========================================================

    public void LoadScene()
    {
        // Guarda la puntuación final 
        if (pointCounter != null)
        {
            PlayerPrefs.SetInt("FinalScore", pointCounter.GetPoints());
        }

        // Carga la escena de transición
        PlayerPrefs.SetString("NextScene", sceneToLoad);
        SceneManager.LoadScene("LoadingScene");
    }
}