using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

public class ScoreDisplay : MonoBehaviour
{
    public Text scoreText;
    public TMP_InputField nameInput;

    [Tooltip("Objeto que se desactiva al guardar el nombre")]
    public GameObject objectToDeactivate1;
    public GameObject objectToDeactivate2;

    [Tooltip("Objeto que se activa al guardar el nombre")]
    public GameObject objectToActivate;

    [Tooltip("Objeto que se muestra cuando el input está vacío")]
    public GameObject warningObject;

    [Tooltip("Duración del aviso en segundos")]
    public float warningDuration = 2f;

    private int finalScore;

    void Start()
    {
        finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        scoreText.text = "Puntos: " + finalScore;
        Debug.Log("Ruta del archivo de puntajes: " + Application.persistentDataPath);
    }

    public void SaveNameAndUpdateScore()
    {
        string playerName = nameInput.text.Trim();

        if (!string.IsNullOrEmpty(playerName) && playerName.EndsWith("|"))
        {
            // Remueve el último carácter (la 'l')
            playerName = playerName.Substring(0, playerName.Length - 1);
        }

        // Si el campo está vacío, mostrar advertencia y salir del método
        if (string.IsNullOrEmpty(playerName))
        {
            if (warningObject != null)
            {
                StartCoroutine(ShowWarning());
            }
            Debug.LogWarning("Debe escribir un nombre antes de continuar.");
            return;
        }

        string levelScene = PlayerPrefs.GetString("LevelScene", "DesertMap_Level1");  // Obtener el valor de PlayerPrefs

        // Llamar al método correspondiente para guardar el puntaje en la tabla adecuada
        SaveScoreForLevel(levelScene, playerName);
        objectToDeactivate1.SetActive(false);
        objectToDeactivate2.SetActive(false);
        objectToActivate.SetActive(true);
    }

    private void SaveScoreForLevel(string levelScene, string playerName)
    {
        // Crear el nombre del archivo correspondiente
        string path = GetPathForLevel(levelScene);

        // **MODIFICACIÓN CLAVE:**
        // Se elimina la lógica de LoadScores y la búsqueda de duplicados.
        // Ahora, siempre se crea un nuevo objeto ScoreData con los datos actuales.

        ScoreData newScore = new ScoreData
        {
            playerName = playerName,
            score = finalScore,
            // Como el tiempo será diferente, este nuevo registro siempre será único.
            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        };

        // Se llama a SaveScore. Se asume que este método en LocalScoreManager 
        // maneja la carga de la lista existente, la adición del nuevo registro 
        // (newScore) y el guardado de la lista completa.
        LocalScoreManager.SaveScore(newScore, path);
    }

    private string GetPathForLevel(string levelScene)
    {
        // Aquí se puede configurar un mapeo para los diferentes mapas y niveles
        return Application.persistentDataPath + "/" + levelScene + "_scores.json";
    }

    private IEnumerator ShowWarning()
    {
        warningObject.SetActive(true);
        yield return new WaitForSeconds(warningDuration);
        warningObject.SetActive(false);
    }
}
