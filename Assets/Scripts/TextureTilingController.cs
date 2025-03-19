using UnityEngine;

public class TextureTilingController : MonoBehaviour
{
    // Desired world size of each tile
    public float tileSize = 2; // doesnt seem to matter

    void Start()
    {
        AdjustTextureTiling();
    }

    void AdjustTextureTiling()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Get the size of the object in world space
            Vector3 objectSize = renderer.bounds.size;

            float rotationAdjustment = Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
            
            // Calculate the tiling based on the object size and desired tile size
            Vector2 tiling = new Vector2(objectSize.x  / (tileSize* rotationAdjustment), objectSize.y / tileSize);

            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propBlock);
            
            // Set the tiling
            propBlock.SetVector("_BaseMap_ST", new Vector4(tiling.x, tiling.y, 0, 0));

            renderer.SetPropertyBlock(propBlock);
        }
    }
}
