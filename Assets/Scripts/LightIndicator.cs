using UnityEngine;

public class LightIndicator : MonoBehaviour
{
    private static readonly int LightColor = Shader.PropertyToID("_Color");
    private static readonly int Flash = Shader.PropertyToID("_Flash");
    
    private bool _isOn;
    private bool _isFlashing;
    private MeshRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void UpdateLight(Color color, bool flashing = false)
    {
        _isOn = !_isOn;
        _renderer.material.SetColor(LightColor, color);
        _renderer.material.SetFloat(Flash, flashing ? 1 : 0);
    }
}
