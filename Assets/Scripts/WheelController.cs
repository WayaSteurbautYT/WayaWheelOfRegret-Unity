using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WheelController : MonoBehaviour
{
    [Header("Wheel Settings")]
    public float spinDuration = 3f;
    public float spinSpeed = 720f; // degrees per second
    public AnimationCurve spinCurve;
    
    [Header("Wheel Segments")]
    public List<WheelSegment> segments = new List<WheelSegment>();
    public Transform wheelTransform;
    
    [Header("Audio")]
    public AudioSource spinSound;
    public AudioSource stopSound;
    
    private bool isSpinning = false;
    private float currentRotation = 0f;
    private float targetRotation = 0f;
    private WheelSegment selectedSegment = null;
    
    public System.Action<WheelSegment> OnWheelStopped;
    
    private void Start()
    {
        InitializeWheel();
    }
    
    private void InitializeWheel()
    {
        if (segments.Count == 0)
        {
            CreateDefaultSegments();
        }
        
        // Position segments in a circle
        float angleStep = 360f / segments.Count;
        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i] != null)
            {
                float angle = i * angleStep;
                segments[i].SetSegmentData(
                    GetSegmentText(i),
                    GetSegmentColor(i),
                    GetSegmentDoom(i),
                    GetSegmentAnswer(i)
                );
                segments[i].angle = angle;
            }
        }
    }
    
    private void CreateDefaultSegments()
    {
        // Create 12 default segments for the wheel
        string[] texts = { "DO IT", "REGRET IT", "CHAOS", "MAYBE", "NOPE", "ASK AGAIN", 
                           "DEFINITELY", "NEVER", "ALWAYS", "SOMETIMES", "PERHAPS", "ABSOLUTELY" };
        
        for (int i = 0; i < 12; i++)
        {
            GameObject segmentObj = new GameObject($"Segment_{i}");
            segmentObj.transform.SetParent(wheelTransform);
            
            WheelSegment segment = segmentObj.AddComponent<WheelSegment>();
            segments.Add(segment);
        }
    }
    
    private string GetSegmentText(int index)
    {
        string[] texts = { "DO IT", "REGRET IT", "CHAOS", "MAYBE", "NOPE", "ASK AGAIN", 
                           "DEFINITELY", "NEVER", "ALWAYS", "SOMETIMES", "PERHAPS", "ABSOLUTELY" };
        return texts[index % texts.Length];
    }
    
    private Color GetSegmentColor(int index)
    {
        // Alternate between red and black like a roulette wheel
        return index % 2 == 0 ? Color.red : Color.black;
    }
    
    private int GetSegmentDoom(int index)
    {
        // Random doom value between 10-90
        return Random.Range(10, 91);
    }
    
    private string GetSegmentAnswer(int index)
    {
        string[] answers = { "Yes, absolutely!", "No, don't do it!", "Maybe reconsider...", 
                         "Go for it!", "Think twice...", "Trust your gut...",
                         "Not worth it!", "Seize the moment!", "Wait and see...",
                         "Jump right in!", "Better not...", "Perfect timing!" };
        return answers[index % answers.Length];
    }
    
    public void SpinWheel()
    {
        if (isSpinning) return;
        
        StartCoroutine(SpinCoroutine());
    }
    
    private IEnumerator SpinCoroutine()
    {
        isSpinning = true;
        
        // Play spin sound
        if (spinSound != null && !spinSound.isPlaying)
        {
            spinSound.Play();
        }
        
        // Calculate random target rotation
        float randomSpins = Random.Range(3f, 6f);
        targetRotation = currentRotation + (randomSpins * 360f) + Random.Range(0f, 360f);
        
        float spinTimer = 0f;
        
        while (spinTimer < spinDuration)
        {
            spinTimer += Time.deltaTime;
            
            // Use animation curve for smooth deceleration
            float curveValue = spinCurve.Evaluate(spinTimer / spinDuration);
            float rotationThisFrame = spinSpeed * Time.deltaTime * curveValue;
            
            currentRotation += rotationThisFrame;
            wheelTransform.localRotation = Quaternion.Euler(0, 0, currentRotation);
            
            yield return null;
        }
        
        // Snap to nearest segment
        SnapToNearestSegment();
        
        // Stop spin sound
        if (spinSound != null && spinSound.isPlaying)
        {
            spinSound.Stop();
        }
        
        // Play stop sound
        if (stopSound != null)
        {
            stopSound.Play();
        }
        
        isSpinning = false;
        
        // Trigger event
        OnWheelStopped?.Invoke(selectedSegment);
    }
    
    private void SnapToNearestSegment()
    {
        if (segments.Count == 0) return;
        
        // Find the segment closest to the current rotation
        float normalizedRotation = currentRotation % 360f;
        if (normalizedRotation < 0) normalizedRotation += 360f;
        
        int closestIndex = 0;
        float closestDistance = float.MaxValue;
        
        for (int i = 0; i < segments.Count; i++)
        {
            float segmentAngle = segments[i].angle;
            float distance = Mathf.Abs(Mathf.DeltaAngle(normalizedRotation, segmentAngle));
            
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        
        // Snap to the closest segment
        targetRotation = currentRotation - closestDistance;
        currentRotation = targetRotation;
        wheelTransform.localRotation = Quaternion.Euler(0, 0, currentRotation);
        
        selectedSegment = segments[closestIndex];
        
        // Highlight the selected segment
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].HighlightSegment(i == closestIndex);
        }
    }
    
    public bool IsSpinning()
    {
        return isSpinning;
    }
    
    public WheelSegment GetSelectedSegment()
    {
        return selectedSegment;
    }
    
    public void ResetWheel()
    {
        currentRotation = 0f;
        targetRotation = 0f;
        selectedSegment = null;
        isSpinning = false;
        wheelTransform.localRotation = Quaternion.identity;
        
        // Clear highlights
        foreach (var segment in segments)
        {
            segment.HighlightSegment(false);
        }
    }
}
