using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Implements sequencer command: "Fade(in|out, [, duration[, webcolor]])".
    /// 
    /// Arguments:
    /// -# in or out.
    /// -# (Optional) Duration in seconds. Default: 1.
    /// -# (Optional) Web color in "\#rrggbb" format. Default: Black.
    /// </summary>
    [AddComponentMenu("")] // Hide from menu.
    public class SequencerCommandFade : SequencerCommand
    {

        private const float SmoothMoveCutoff = 0.05f;

        private string direction;
        private float duration;
        private Color color;
        private bool fadeIn;
        float startTime;
        float endTime;
        private Canvas faderCanvas = null;
        private UnityEngine.UI.Image faderImage = null;

        public void Start()
        {
            // Get the values of the parameters:
            direction = GetParameter(0);
            duration = GetParameterAsFloat(1, 0);
            color = Tools.WebColor(GetParameter(2, "#000000"));
            if (DialogueDebug.LogInfo) Debug.Log(string.Format("{0}: Sequencer: Fade({1}, {2}, {3})", new System.Object[] { DialogueDebug.Prefix, direction, duration, color }));

            if (duration > SmoothMoveCutoff)
            {

                // Create fader canvas and image:
                faderCanvas = new GameObject("Fader", typeof(Canvas)).GetComponent<Canvas>();
                faderCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                faderCanvas.sortingOrder = 9999;
                faderImage = new GameObject("Fader Image", typeof(UnityEngine.UI.Image)).GetComponent<UnityEngine.UI.Image>();
                faderImage.transform.SetParent(faderCanvas.transform, false);
                faderImage.rectTransform.anchorMin = Vector2.zero;
                faderImage.rectTransform.anchorMax = Vector2.one;
                faderImage.sprite = null;

                // Set up duration:
                startTime = DialogueTime.time;
                endTime = startTime + duration;

                fadeIn = string.Equals(direction, "in", System.StringComparison.OrdinalIgnoreCase);

                if (fadeIn)
                {
                    faderImage.color = new Color(color.r, color.g, color.b, 1);
                }
                else
                {
                    faderImage.color = new Color(color.r, color.g, color.b, 0);
                }
            }
            else
            {
                Stop();
            }
        }

        public void Update()
        {
            // Keep smoothing for the specified duration:
            if ((DialogueTime.time < endTime) && (faderImage != null))
            {
                float elapsed = (DialogueTime.time - startTime) / duration;
                float alpha = fadeIn ? (1 - elapsed) : elapsed;
                faderImage.color = new Color(color.r, color.g, color.b, alpha);
            }
            else
            {
                Stop();
            }
        }

        public void OnDestroy()
        {
            if (faderCanvas != null) Destroy(faderCanvas.gameObject);
        }

    }

}
