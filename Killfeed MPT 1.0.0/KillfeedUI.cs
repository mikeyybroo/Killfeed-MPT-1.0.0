using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace Killfeed_MPT
{
    public class KillfeedUI : MonoBehaviour
    {
        private const float messageHeight = 30f; // Height of each message
        private const float messageSpacing = 5f; // Spacing between messages
        private const float messageLifetime = 5f; // Lifetime of each message in seconds
        private const float verticalOffset = 25f; // Vertical offset for the new message

        private Canvas canvas; // Reference to the Canvas
        private List<GameObject> activeMessages = new List<GameObject>(); // List to track active messages

        // Ensure initialization is called before displaying the first message
        public void Initialize()
        {
            // Find or create Canvas object
            canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObject = new GameObject("Canvas");
                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
            }
        }

        public void DisplayKillMessage(string message)
        {
            // Initialize if not already done
            if (canvas == null)
                Initialize();

            // Move existing messages down and reset their timers
            MoveExistingMessagesDownAndResetTimers();

            // Instantiate TextMeshPro object
            GameObject textObject = new GameObject("KillMessage", typeof(TextMeshProUGUI));

            // Set parent to Canvas
            textObject.transform.SetParent(canvas.transform, false);

            // Get TextMeshPro component
            TextMeshProUGUI textMesh = textObject.GetComponent<TextMeshProUGUI>();
            if (textMesh == null)
                textMesh = textObject.AddComponent<TextMeshProUGUI>();

            // Set kill message text
            textMesh.text = message;

            // Set text alignment to top right
            textMesh.alignment = TextAlignmentOptions.TopRight;

            // Set smaller text size
            textMesh.fontSize = 14;

            // Set position to middle of the right side of the screen with dynamic Y position
            RectTransform rectTransform = textMesh.rectTransform;
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2(-10, -verticalOffset); // Adjust offset as needed

            // Add message to the list of active messages
            activeMessages.Add(textObject);

            // Start coroutine to destroy the message after the specified lifetime
            StartCoroutine(DestroyMessage(textObject));
        }

        private IEnumerator DestroyMessage(GameObject messageObject)
        {
            yield return new WaitForSeconds(messageLifetime);
            activeMessages.Remove(messageObject);
            Destroy(messageObject);
        }

        private void MoveExistingMessagesDownAndResetTimers()
        {
            foreach (var message in activeMessages)
            {
                RectTransform rectTransform = message.GetComponent<RectTransform>();
                rectTransform.anchoredPosition -= new Vector2(0, messageHeight + messageSpacing);
                StopCoroutine(DestroyMessage(message));
                StartCoroutine(DestroyMessage(message));
            }
        }
    }
}
