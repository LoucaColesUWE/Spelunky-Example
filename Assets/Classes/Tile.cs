using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool isPartofChunk = false;
    public bool IsPartofChunk => isPartofChunk;
    public void UpdateTile(Sprite _newSprite, bool needsCollider = false, bool _isPartofChunk = false)
    {
        if (_isPartofChunk && !isPartofChunk)
        {
            isPartofChunk = true;
        }
        else if (!_isPartofChunk && isPartofChunk)
        {
            return;
        }
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = _newSprite;
        if (needsCollider)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }
}
