using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelSegment : MonoBehaviour
{
    [Header("Segment Properties")]
    public string segmentText = "";
    public Color segmentColor = Color.white;
    public int doomValue = 50;
    public string answerText = "";
    public float angle = 0f;
    
    [Header("Visual Components")]
    public MeshRenderer segmentRenderer;
    public TextMeshProUGUI segmentTextUI;
    
    private void Start()
    {
        InitializeSegment();
    }
    
    private void InitializeSegment()
    {
        // Set segment color
        if (segmentRenderer != null)
        {
            segmentRenderer.material.color = segmentColor;
        }
        
        // Set segment text
        if (segmentTextUI != null)
        {
            segmentTextUI.text = segmentText;
            segmentTextUI.color = Color.white;
        }
        
        // Calculate angle based on position in wheel
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
    
    public void HighlightSegment(bool highlight)
    {
        if (segmentRenderer != null)
        {
            Color targetColor = highlight ? Color.yellow : segmentColor;
            segmentRenderer.material.color = targetColor;
        }
    }
    
    public void SetSegmentData(string text, Color color, int doom, string answer)
    {
        segmentText = text;
        segmentColor = color;
        doomValue = doom;
        answerText = answer;
        
        if (segmentRenderer != null)
            segmentRenderer.material.color = color;
            
        if (segmentTextUI != null)
            segmentTextUI.text = text;
    }
}
