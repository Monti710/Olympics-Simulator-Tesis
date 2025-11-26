using UnityEngine;
using TMPro; // Asegúrate de tener este namespace

public class PointDisplay : MonoBehaviour
{
    public PointCounter pointCounter; // Referencia al script PointCounter
    public TextMeshProUGUI scoreText; // Referencia al TextMeshPro donde se mostrará el puntaje
    public TextMeshProUGUI scoreText1;

    void Start()
    {
        // Asegúrate de que las referencias estén asignadas
        if (pointCounter == null || scoreText == null || scoreText1 == null)
        {
            Debug.LogError("PointCounter o TextMeshProUGUI no están asignados correctamente.");
        }
    }

    void Update()
    {
        // Actualiza el texto de puntaje cada cuadro
        if (pointCounter != null && scoreText != null && scoreText1 != null)
        {
            scoreText.text = "Puntos: " + pointCounter.GetPoints();
            scoreText1.text = "Puntos: " + pointCounter.GetPoints();
        }
    }
}
