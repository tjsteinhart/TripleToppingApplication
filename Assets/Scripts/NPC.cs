using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, Interactable
{
    [SerializeField] Material highlightMaterial;
    [SerializeField] string materialBool;

    // Start is called before the first frame update
    void Start()
    {
        UnHighlightInteractable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HighlightInteractable()
    {
        highlightMaterial.SetInt(materialBool, 1);

    }

    public void UnHighlightInteractable()
    {
        highlightMaterial.SetInt("Boolean_1B41AF11", 0);
    }

    public void PlayerInteraction()
    {
        
    }
}
