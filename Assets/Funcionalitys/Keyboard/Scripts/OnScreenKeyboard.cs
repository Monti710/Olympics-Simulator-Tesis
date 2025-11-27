using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrabdeepDhaliwal.OnScreenKeyboard
{
    public class OnScreenKeyboard : MonoBehaviour
    {
        [Header("Campo de texto destino")]
        public TMP_InputField targetInputField;

        [Header("Teclas normales")]
        public Key[] keys;

        [Header("Teclas especiales")]
        public Button shiftButton;
        public Button capsLockButton;
        public Button enterButton;
        public Button backspaceButton; // Back
        public Button deleteButton;    // Del
        public Button spaceButton;
        public Button leftArrowButton;
        public Button rightArrowButton;

        private bool isCapsLockOn = false;
        private bool isShiftPressed = false;

        // NUEVO: estado interno del texto y del cursor
        private string currentText = "";
        private int caretIndex = 0;

        // Caracter visual del “puntero” (línea)
        [SerializeField] private string caretVisual = "|";
        // Si quieres usar RichText (ej: colorear el cursor)
        [SerializeField] private bool useRichTextCaret = false;

        void Start()
        {
            if (targetInputField == null)
            {
                Debug.LogError("⚠️ No se ha asignado ningún TMP_InputField al teclado.");
                return;
            }

            // Opcional: desactivar el caret nativo para que no se vea
            targetInputField.caretColor = new Color(0, 0, 0, 0);
            // Opcional: para que el usuario no escriba con el teclado físico
            // targetInputField.readOnly = true;

            // Inicializar estado interno
            currentText = targetInputField.text;
            caretIndex = currentText.Length;
            UpdateVisualText();

            // Configurar las teclas normales
            foreach (var key in keys)
            {
                key.Setup();
                key.button.onClick.AddListener(() => OnKeyPress(key));
            }

            // Configurar las teclas especiales
            capsLockButton?.onClick.AddListener(ToggleCapsLock);
            shiftButton?.onClick.AddListener(ToggleShift);

            // BACK: borrar la última letra del texto
            backspaceButton?.onClick.AddListener(DeleteLastCharacter);

            // DEL: borrar TODO el contenido del input
            deleteButton?.onClick.AddListener(ClearAllText);

            spaceButton?.onClick.AddListener(() => InsertCharacter(" "));
            enterButton?.onClick.AddListener(SubmitText);
            leftArrowButton?.onClick.AddListener(() => MoveCaret(-1));
            rightArrowButton?.onClick.AddListener(() => MoveCaret(1));

            UpdateKeyLabels();
        }

        // ============================
        //    MANEJO DE TEXTO / CURSOR
        // ============================

        // Actualiza el texto visible en el TMP_InputField insertando el “puntero”
        private void UpdateVisualText()
        {
            if (targetInputField == null) return;

            if (currentText == null)
                currentText = "";

            // Aseguramos que el índice esté dentro de rango
            caretIndex = Mathf.Clamp(caretIndex, 0, currentText.Length);

            string visible = currentText;

            if (useRichTextCaret)
            {
                // Ejemplo con color blanco (asegúrate de que Rich Text esté activo en el TMP_InputField)
                visible = visible.Insert(caretIndex, "<color=#FFFFFFFF>" + caretVisual + "</color>");
            }
            else
            {
                visible = visible.Insert(caretIndex, caretVisual);
            }

            targetInputField.text = visible;
        }

        // Inserta texto en la posición del cursor
        private void InsertCharacter(string character)
        {
            if (string.IsNullOrEmpty(character)) return;

            caretIndex = Mathf.Clamp(caretIndex, 0, currentText.Length);
            currentText = currentText.Insert(caretIndex, character);
            caretIndex += character.Length;

            UpdateVisualText();
        }

        // BACK: elimina la ÚLTIMA letra del texto (sin importar donde esté el cursor)
        private void DeleteLastCharacter()
        {
            if (string.IsNullOrEmpty(currentText)) return;

            currentText = currentText.Remove(currentText.Length - 1, 1);
            // Después de borrar la última, colocamos el cursor al final
            caretIndex = currentText.Length;

            UpdateVisualText();
        }

        // DEL: elimina TODO el texto
        private void ClearAllText()
        {
            currentText = "";
            caretIndex = 0;
            UpdateVisualText();
        }

        // Mover el cursor manualmente con las flechas
        private void MoveCaret(int direction)
        {
            caretIndex = Mathf.Clamp(caretIndex + direction, 0, currentText.Length);
            UpdateVisualText();
        }

        // ============================
        //         TECLAS NORMALES
        // ============================

        private void OnKeyPress(Key key)
        {
            if (targetInputField == null) return;

            string value = GetKeyText(key);
            InsertCharacter(value);

            if (isShiftPressed)
            {
                isShiftPressed = false;
                UpdateKeyLabels();
            }
        }

        // ============================
        //        TECLAS ESPECIALES
        // ============================

        private void SubmitText()
        {
            Debug.Log("Texto ingresado: " + currentText);
            // Aquí puedes usar currentText directamente en lugar de targetInputField.text
        }

        private void ToggleCapsLock()
        {
            isCapsLockOn = !isCapsLockOn;
            UpdateKeyLabels();
        }

        private void ToggleShift()
        {
            isShiftPressed = !isShiftPressed;
            UpdateKeyLabels();
        }

        // ============================
        //    MAYÚSCULAS / SHIFT
        // ============================

        private string GetKeyText(Key key)
        {
            string text = key.primaryValue;

            if (text.Length == 1 && char.IsLetter(text[0]))
            {
                if (isShiftPressed ^ isCapsLockOn)
                    text = text.ToUpper();
                else
                    text = text.ToLower();
            }
            else if (isShiftPressed && !string.IsNullOrEmpty(key.secondaryValue))
            {
                text = key.secondaryValue;
            }

            return text;
        }

        private void UpdateKeyLabels()
        {
            foreach (var key in keys)
            {
                if (key.primaryKeyText == null) continue;

                if (key.primaryValue.Length == 1 && char.IsLetter(key.primaryValue[0]))
                {
                    key.primaryKeyText.text = (isShiftPressed ^ isCapsLockOn)
                        ? key.primaryValue.ToUpper()
                        : key.primaryValue.ToLower();
                }
                else
                {
                    key.primaryKeyText.text = isShiftPressed && !string.IsNullOrEmpty(key.secondaryValue)
                        ? key.secondaryValue
                        : key.primaryValue;
                }
            }
        }
    }
}
