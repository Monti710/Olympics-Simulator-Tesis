using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using TMPro;

public class ScoreboardLoader : MonoBehaviour
{
    [Header("Referencias UI")]
    public Text contentText;  // Donde se mostrarán los puntajes
    public ScrollRect scrollRect;  // Componente ScrollRect para el desplazamiento
    public Text headerText;  // Nuevo campo para el encabezado (mostrar mapa y nivel)
    public TextMeshProUGUI headerTrophys;

    // Nuevos campos para mostrar los primeros 3 puntajes
    public TextMeshProUGUI top1Text;  // Para el top 1
    public TextMeshProUGUI top2Text;  // Para el top 2
    public TextMeshProUGUI top3Text;  // Para el top 3

    void Start()
    {
        // Al inicio, cargamos los puntajes del nivel/mapa actual
        MostrarListaDePuntajes();
    }

    // Función que carga y muestra los puntajes de la tabla correspondiente al nivel/mapa
    public void MostrarListaDePuntajes()
    {
        // Obtener el valor de PlayerPrefs que indica el mapa y nivel actual
        string levelScene = PlayerPrefs.GetString("LevelScene", "DesertMap_Level1");  // Default: "DesertMap_Level1"
        string path = GetPathForLevel(levelScene);  // Construir la ruta del archivo JSON

        // Cargar los puntajes desde el archivo correspondiente
        ScoreList scoreList = LocalScoreManager.LoadScores(path);

        // Si no hay datos, mostrar mensaje de "No se encontró ningún dato"
        if (scoreList == null || scoreList.scores == null || scoreList.scores.Count == 0)
        {
            headerText.text = "No se encontró la lista";  // Cambiar encabezado
            headerTrophys.text = "No se encontró la lista";
            contentText.text = "No se encontró ningún dato.";  // Mensaje de error en el contenido

            // Mostrar "No disponible" en los top 1, top 2 y top 3
            top1Text.text = "No disponible";
            top2Text.text = "No disponible";
            top3Text.text = "No disponible";
            return;
        }

        // Obtener el nombre del mapa y el nivel desde PlayerPrefs
        string[] splitScene = levelScene.Split('_');
        string mapName = splitScene[0];
        string level = splitScene[1];

        // Generar el encabezado dinámico
        string mapDescription = GetMapDescription(mapName);
        string levelDescription = GetLevelDescription(level);
        headerText.text = $"{mapDescription}, {levelDescription}";  // Mostrar en la parte superior
        headerTrophys.text = $"{mapDescription}, {levelDescription}";

        // Ordenar los puntajes en orden descendente
        var listaOrdenada = scoreList.scores.OrderByDescending(s => s.score).ToList();

        // Asignar los top 1, top 2 y top 3
        if (listaOrdenada.Count >= 1)
            top1Text.text = $"{listaOrdenada[0].playerName} - {listaOrdenada[0].score} puntos";
        else
            top1Text.text = "No disponible";

        if (listaOrdenada.Count >= 2)
            top2Text.text = $"{listaOrdenada[1].playerName} - {listaOrdenada[1].score} puntos";
        else
            top2Text.text = "No disponible";

        if (listaOrdenada.Count >= 3)
            top3Text.text = $"{listaOrdenada[2].playerName} - {listaOrdenada[2].score} puntos";
        else
            top3Text.text = "No disponible";

        // Crear el texto que se mostrará en la UI
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("<b>🏆 Tabla de Puntajes 🏆</b>");
        sb.AppendLine("");  // Agregar espacio adicional entre la cabecera y los datos

        // Recorrer la lista de puntajes y agregar la información de cada uno al texto
        for (int i = 0; i < listaOrdenada.Count; i++)
        {
            var data = listaOrdenada[i];
            string fecha = string.IsNullOrEmpty(data.date) ? "sin fecha" : data.date;
            sb.AppendLine($"{i + 1}. {data.playerName} - {data.score} puntos - {fecha}");
        }

        // Mostrar el texto generado en la UI
        contentText.text = sb.ToString();

        // Desplazamiento automático hacia el primer puntaje (opcional)
        StartCoroutine(ScrollToTop());
    }

    // Corrutina para desplazar el ScrollRect hacia el principio (opcional)
    private IEnumerator ScrollToTop()
    {
        yield return null;  // Esperar un frame para que se actualice el layout
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;  // Desplazar al principio (top)
        }
    }

    // Obtener la ruta del archivo JSON para el nivel y mapa actual
    private string GetPathForLevel(string levelScene)
    {
        return Application.persistentDataPath + "/" + levelScene + "_scores.json";
    }

    // Obtener la descripción del mapa correspondiente
    private string GetMapDescription(string mapName)
    {
        switch (mapName)
        {
            case "DesertMap":
                return "Mapa del desierto";
            case "StadiumMap":
                return "Mapa del estadio";
            case "SabanaMap":
                return "Mapa de la sabana";
            default:
                return "Mapa desconocido";
        }
    }

    // Obtener la descripción del nivel correspondiente
    private string GetLevelDescription(string level)
    {
        switch (level)
        {
            case "Level1":
                return "Nivel de dificultad Fácil";
            case "Level2":
                return "Nivel de dificultad Medio";
            case "Level3":
                return "Nivel de dificultad Difícil";
            default:
                return "Nivel desconocido";
        }
    }
}
