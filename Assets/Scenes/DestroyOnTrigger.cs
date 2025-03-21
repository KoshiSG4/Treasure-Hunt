using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class DestroyOnTrigger : MonoBehaviour
{
    public string m_Tag = "Player";
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == m_Tag)
            Destroy(gameObject);
    }
}
