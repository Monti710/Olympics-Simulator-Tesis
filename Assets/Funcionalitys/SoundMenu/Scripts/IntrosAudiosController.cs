using UnityEngine;
using UnityEngine.UI;

public class IntrosAudiosController : MonoBehaviour
{
    // Componente AudioSource que se controlará
    public AudioSource audioSource;

    // Botón de Pausa/Reanudar (ya no necesitamos su componente Image)
    public Button pauseResumeButton;

    // Botón de Reiniciar Reproducción
    public Button restartButton;

    // GameObject que se mostrará cuando el audio esté reproduciéndose (el icono de Pausa)
    public GameObject pauseIconObject;

    // GameObject que se mostrará cuando el audio esté en pausa (el icono de Reproducir)
    public GameObject playIconObject;

    private void Awake()
    {
        // Asegurarse de que el AudioSource esté asignado
        if (audioSource == null)
        {
            // Intentar obtener el AudioSource en el mismo GameObject
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        // 1. Reproducir el audio tan pronto como el componente se active
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Stop();
            audioSource.Play();

            // Asegurar que el icono inicial sea el de PAUSA (ya que se está reproduciendo)
            UpdateIconObjects();
        }

        // Asignar las funciones a los eventos de click de los botones
        if (pauseResumeButton != null)
        {
            pauseResumeButton.onClick.AddListener(TogglePauseResume);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartAudio);
        }
    }

    private void OnDisable()
    {
        // Detener la reproducción cuando el componente se desactive
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // Quitar las funciones de los eventos de click para evitar errores al deshabilitar
        if (pauseResumeButton != null)
        {
            pauseResumeButton.onClick.RemoveListener(TogglePauseResume);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartAudio);
        }
    }

    /// <summary>
    /// Alterna entre Pausa y Reanudar la reproducción del AudioSource.
    /// </summary>
    public void TogglePauseResume()
    {
        if (audioSource == null) return;

        if (audioSource.isPlaying)
        {
            // Pausar
            audioSource.Pause();
        }
        else
        {
            // Reanudar
            audioSource.UnPause();
        }

        // Actualizar la visibilidad de los GameObjects de icono
        UpdateIconObjects();
    }

    /// <summary>
    /// Reinicia la reproducción del audio desde el inicio.
    /// </summary>
    public void RestartAudio()
    {
        if (audioSource == null || audioSource.clip == null) return;

        // Detener y Reproducir desde el inicio
        audioSource.Stop();
        audioSource.Play();

        // Asegurar que el icono sea el de PAUSA (ya que se está reproduciendo)
        UpdateIconObjects();
    }

    /// <summary>
    /// Activa/Desactiva los GameObjects de icono según el estado de reproducción.
    /// </summary>
    private void UpdateIconObjects()
    {
        if (pauseIconObject == null || playIconObject == null) return;

        bool isPlaying = audioSource.isPlaying;

        // Si está reproduciéndose (isPlaying es true):
        //   - Activamos el icono de PAUSA (para indicar que puedes pausar).
        //   - Desactivamos el icono de PLAY (para indicar que no necesitas reproducir).
        pauseIconObject.SetActive(isPlaying);

        // Si está en pausa o detenido (isPlaying es false):
        //   - Desactivamos el icono de PAUSA.
        //   - Activamos el icono de PLAY (para indicar que puedes reproducir/reanudar).
        playIconObject.SetActive(!isPlaying);
    }
}