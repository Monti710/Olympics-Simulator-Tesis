using UnityEngine;
using UnityEngine.XR.Management; // Necesario para el subsistema
using UnityEngine.InputSystem.XR;
using System.Collections;
using System.Linq;

public class StartupRecenter : MonoBehaviour
{
    [Tooltip("El Transform que contiene la cámara real (Main Camera).")]
    public Transform mainCameraTransform;

    void Start()
    {
        // Esto asegura que la inicialización del subsistema XR haya terminado.
        // Es más robusto que un simple 'yield return null'.
        StartCoroutine(ExecuteRecenterAfterInit());
    }

    IEnumerator ExecuteRecenterAfterInit()
    {
        // Espera hasta que el subsistema XR esté inicializado y haya un HMD presente.
        // Esto puede tomar varios frames.
        while (mainCameraTransform.localRotation == Quaternion.identity)
        {
            yield return null;
        }

        // 1. Obtener la rotación Y local actual de la cámara.
        // Esta es la rotación de la cabeza del jugador relativa al XR Origin.
        float cameraYaw = mainCameraTransform.localRotation.eulerAngles.y;

        // 2. Determinar la Rotación Deseada del XR Origin.
        // Queremos que el XR Origin (transform.rotation) gire para que la cámara (mainCameraTransform)
        // termine apuntando hacia adelante (0 grados globalmente o hacia tu menú).

        // La rotación que debes aplicar al XR Origin es el negativo de la rotación actual de la cabeza.
        // Si el jugador está rotado 180 grados localmente, el origen debe rotar -180 grados.

        // Creamos la rotación de compensación.
        Quaternion compensationRotation = Quaternion.Euler(0f, -cameraYaw, 0f);

        // 3. Aplicar la compensación al XR Origin.
        // Giramos el XR Origin por la rotación de compensación.
        transform.rotation *= compensationRotation;

        Debug.Log($"Recenter aplicado. Compensación Yaw: {-cameraYaw} grados.");
    }
}