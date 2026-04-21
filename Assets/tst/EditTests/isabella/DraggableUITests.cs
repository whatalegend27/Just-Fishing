using UnityEngine;
using NUnit.Framework;
using UnityEngine.UI;

// Tests for the DraggableItemUI's drag behavior and item management
public class DraggableUITests : MonoBehaviour
{

    // Tests that beginning a drag disables the grid layout and raycast target
    [Test]
    public void BeginDrag_Disables_Raycast_And_Grid()
    {
        var obj = new GameObject();
        var drag = obj.AddComponent<DraggableItemUI>();

        drag.grid = obj.AddComponent<GridLayoutGroup>();
        drag.item = ScriptableObject.CreateInstance<ItemScript>();

        drag.SendMessage("Awake");

        drag.OnBeginDrag(null);

        Assert.IsFalse(drag.grid.enabled);
    }

    // Tests that ending a drag re-enables the grid layout and raycast target
    [Test]
    public void BeginDrag_Resets_WasDropped()
    {
        var obj = new GameObject();
        var drag = obj.AddComponent<DraggableItemUI>();

        drag.item = ScriptableObject.CreateInstance<ItemScript>();
        drag.wasDropped = true;

        drag.SendMessage("Awake");
        drag.OnBeginDrag(null);

        Assert.IsFalse(drag.wasDropped);
    }

    // Tests that ending a drag re-enables the grid layout and raycast target
    [Test]
    public void EndDrag_Returns_To_Original_Parent_When_Not_Dropped()
    {
        var parent = new GameObject().transform;
        var obj = new GameObject();

        obj.transform.SetParent(parent);

        var drag = obj.AddComponent<DraggableItemUI>();
        drag.item = ScriptableObject.CreateInstance<ItemScript>();

        drag.SendMessage("Awake");
        drag.OnBeginDrag(null);
        drag.OnEndDrag(null);

        Assert.AreEqual(parent, obj.transform.parent);
    }

    // Tests that setting an item updates the item field
    [Test]
    public void SetItem_Updates_Item()
    {
        var obj = new GameObject();
        var drag = obj.AddComponent<DraggableItemUI>();

        var image = obj.AddComponent<Image>();
        drag.SendMessage("Awake");

        var item = ScriptableObject.CreateInstance<ItemScript>();
        drag.SetItem(item);

        Assert.AreEqual(item, drag.item);
    }
}
