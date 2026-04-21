using UnityEngine;
using NUnit.Framework;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Tests for the DropZoneUI's item dropping and icon updating functionality
public class DropZoneUIEditTests : MonoBehaviour
{
    // Test if the DropZone correctly ignores a null dragged item without crashing
    [Test]
    public void DropZone_Ignores_Null_Dragged_Item()
    {
        var obj = new GameObject();
        var drop = obj.AddComponent<DropZoneUI>();

        var eventData = new PointerEventData(null);
        eventData.pointerDrag = null;

        drop.OnDrop(eventData);

        Assert.Pass(); 
    }

    // Test if the DropZone rejects an item of the wrong type
    [Test]
    public void DropZone_Rejects_Wrong_Item_Type()
    {
        var obj = new GameObject();
        var drop = obj.AddComponent<DropZoneUI>();

        var dragObj = new GameObject();
        var dragged = dragObj.AddComponent<DraggableItemUI>();

        var item = ScriptableObject.CreateInstance<ItemScript>();
        typeof(ItemScript)
            .GetField("itemType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(item, ItemScript.ItemType.Bait);

        dragged.item = item;

        var eventData = new PointerEventData(null);
        eventData.pointerDrag = dragObj;

        drop.OnDrop(eventData);

        Assert.IsFalse(dragged.wasDropped);
    }

    //Test if the DropZone accepts the correct item type and updates the icon accordingly
    [Test]
    public void DropZone_Accepts_Correct_Item()
    {
        var obj = new GameObject();
        var drop = obj.AddComponent<DropZoneUI>();

        var dragObj = new GameObject();
        var dragged = dragObj.AddComponent<DraggableItemUI>();

        var item = ScriptableObject.CreateInstance<ItemScript>();
        typeof(ItemScript)
            .GetField("itemType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(item, ItemScript.ItemType.Lure);

        dragged.item = item;

        var eventData = new PointerEventData(null);
        eventData.pointerDrag = dragObj;

        drop.OnDrop(eventData);

        Assert.IsTrue(dragged.wasDropped);
    }

    //Test if the Icon updates correctly when a valid item is dropped
    [Test]
    public void DropZone_Updates_Icon_On_Success()
    {
        var canvasObj = new GameObject();
        canvasObj.AddComponent<Canvas>();

        var obj = new GameObject();
        obj.transform.SetParent(canvasObj.transform);

        var drop = obj.AddComponent<DropZoneUI>();
        var image = obj.AddComponent<Image>();
        typeof(DropZoneUI)
            .GetField("icon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(drop, image);

        var dragObj = new GameObject();
        var dragged = dragObj.AddComponent<DraggableItemUI>();

        var item = ScriptableObject.CreateInstance<ItemScript>();
        typeof(ItemScript)
            .GetField("itemType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(item, ItemScript.ItemType.Lure);

        dragged.item = item;

        var eventData = new PointerEventData(null);
        eventData.pointerDrag = dragObj;

        drop.OnDrop(eventData);

        Assert.AreEqual(item.Icon, image.sprite);
    }
}
