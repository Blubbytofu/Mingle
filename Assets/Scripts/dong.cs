using UnityEngine;

public class dong : MonoBehaviour
{
    public float amplitude = 1.0f;
    public float frequency = 1.0f;
    public Vector3 direction = Vector3.up;
    private Vector3 startPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float movement = amplitude * Mathf.Sin(Time.time * frequency * 2 * Mathf.PI);
        transform.position = startPos + direction.normalized * movement;
    }
}
