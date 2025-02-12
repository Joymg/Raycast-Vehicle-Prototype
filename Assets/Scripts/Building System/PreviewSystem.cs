using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{


    [SerializeField] 
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField] 
    private Material previewMaterialsPrefab;
    private Material previewMaterialInstance;

    private Renderer[] cellIndicatorRenderers;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialsPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderers = GetComponentsInChildren<Renderer>();
    }

    public void StartPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3((float)size.x/2, 1, (float)size.y/2);
        }
        cellIndicator.SetActive(true);
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (var index = 0; index < materials.Length; index++)
            {
                materials[index] = previewMaterialInstance;
            }

            renderer.materials = materials;
        }
    }

    public void StopPreview()
    {
        cellIndicator.SetActive(false);
        Destroy(previewObject);
    }

    public void UpdatePreviewPosition(Vector3 position, Vector3 cursorOffset, bool isValid)
    {
        MovePreview(position);
        MoveCursor(position + cursorOffset);
        ApplyFeedback(isValid);
    }

    public void ApplyFeedback(bool isValid)
    {
        Color c = isValid ? Color.white : Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;

        foreach (Renderer renderer in cellIndicatorRenderers)
        {
            renderer.material.color = c;
        }
    }

    public void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position ;
        cellIndicator.transform.position +=
            new Vector3(cellIndicator.transform.localScale.x, 0, cellIndicator.transform.localScale.z);
    }

    public void MovePreview(Vector3 position)
    {
        previewObject.transform.position = position;
    }

    public void RotatePreview(int turns)
    {
        previewObject.transform.rotation = Quaternion.identity;
        previewObject.transform.Rotate(Vector3.up, ObjectPlacer.ROTATION_ANGLE * turns);
        
    }
}
