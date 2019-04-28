using UnityEngine;

public class ColorPickerTester : MonoBehaviour 
{

    public Renderer[] renderers;
    public ColorPicker picker;

    public Color Color;

	// Use this for initialization
	void Start () 
    {
        Color = renderers[0].material.color;
        picker.onValueChanged.AddListener(color =>
        {
            foreach (Renderer rend in renderers)
            {
                rend.material.color = color;
                Color = color;
            }
        });

		renderers[0].material.color = picker.CurrentColor;

        picker.CurrentColor = Color;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
