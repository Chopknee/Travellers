using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShopData : ScriptableObject {
    
    public string ShopName;
    public ItemType[] spawnableItems;//Items that are allowed to spawn at this shop
    public Collection[] collections;//The list of collections used for filtering allowed collections.
    public bool blacklistMode;//Wheather or not to use blacklist mode or whitelist mode.

    public ShopData GetNew() {
        ShopData sd = (ShopData)ScriptableObject.CreateInstance("ShopData");
        sd.ShopName = ShopName;
        sd.spawnableItems = new ItemType[spawnableItems.Length];
        System.Array.Copy(spawnableItems, sd.spawnableItems, spawnableItems.Length);
        sd.collections = new Collection[collections.Length];
        System.Array.Copy(collections, sd.collections, collections.Length);
        sd.blacklistMode = blacklistMode;
        return sd;
    }

}
