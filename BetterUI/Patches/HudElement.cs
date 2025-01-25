using System;
using UnityEngine;

namespace BetterUI.Patches;

[Serializable]
public class HudElement
{
    // the serialization is NOT done with the unity serializer, so private members ARE serialized, and SerializeField, ISerializationCallbackReceiver and FormerlySerializedAs do not work

    private readonly string name;
    private readonly string displayName;
    private readonly string path;

    /// <summary>
    /// Layer Group where the element belongs
    /// </summary>
    private readonly Groups group;

    private float x;
    private float y;
    private float scale;

    // DO NOT rename this field. keep it as 'dimensions'. The deserialization used is NOT unity deserialization, so FormerlySerializedAs does NOT work
    private float dimensions;

    private float xDimensions;

    // AnchorMin
    private float anchorMinX;

    private float anchorMinY;

    // AnchorMax
    private float anchorMaxX;

    private float anchorMaxY;

    public HudElement(string name, string displayName, Groups group, string path, Vector2 position, float scale = 1f, float xDimensions = 1f, float yDimensions = 1f)
    {
        this.name = name;
        this.displayName = displayName;
        this.path = path;
        this.group = group;

        this.x = position.x;
        this.y = position.y;
        this.scale = scale;
        this.xDimensions = xDimensions;
        this.dimensions = yDimensions;
    }

    // intentionally no setter
    public string Name
    {
        get => this.name;
    }

    public string DisplayName
    {
        get => this.displayName;
    }

    public string Path
    {
        get => this.path;
    }

    public Groups Group
    {
        get => this.group;
    }

    public Vector2 Position
    {
        get => new Vector2(this.x, this.y);
        set
        {
            this.x = value.x;
            this.y = value.y;
        }
    }

    public Vector2 AnchorMin
    {
        get => new Vector2(anchorMinX, anchorMinY);
        set
        {
            this.anchorMinX = value.x;
            this.anchorMinY = value.y;
        }
    }

    public Vector2 AnchorMax
    {
        get => new Vector2(anchorMaxX, anchorMaxY);
        set
        {
            this.anchorMaxX = value.x;
            this.anchorMaxY = value.y;
        }
    }

    public float Scale
    {
        get => this.scale;
    }

    public float XDimensions
    {
        get => this.xDimensions;
    }

    public float YDimensions
    {
        get => this.dimensions;
    }

    // scale == 0 is intentionally still allowed
    public void ChangeScale(float change)
    {
        scale = (float)Math.Round(Mathf.Abs(scale + change), 1);
    }

    public void ChangeXDims(float change)
    {
        xDimensions = Mathf.Max(0.1f, (float)Math.Round(Mathf.Abs(xDimensions + change), 2));
    }

    public void ChangeYDims(float change)
    {
        dimensions = Mathf.Max(0.1f, (float)Math.Round(Mathf.Abs(dimensions + change), 2));
    }

    // called manually in CustomHud.Load because unitys ISerializationCallbackReceiver does not work with the used deserializer
    public void OnAfterDeserialize()
    {
        // this especially catches backwards compatibility, because all older setups will have xDimensions == 0 (default values do not work with the deserializer), so keep this as xDimensions = 1f;
        if (xDimensions < 0.1f)
        {
            xDimensions = 1f;
        }

        if (dimensions < 0.1f)
        {
            dimensions = 1f;
        }
    }
}

public readonly struct Element
{
    /// <summary>
    /// Used as an unique value, unique name to the element.
    /// If no path is given, this needs to be the elements path as well.
    /// </summary>
    private readonly string name; // We see this as unique. Might cause issues later on?

    /// <summary>
    /// Use custom name on the template when user edits HUD
    /// </summary>
    private readonly string displayName;

    /// <summary>
    /// On what layer group should the element be editable. This is as well the parent element where path is related.
    /// </summary>
    private readonly Groups group;

    /// <summary>
    /// Path of the element. Relative to parent.
    /// </summary>
    private readonly string locationPath;

    public string Name
    {
        get => name;
    }

    public string DisplayName
    {
        get => displayName;
    }

    public Groups Group
    {
        get => group;
    }

    public string LocationPath
    {
        get => locationPath;
    }

    public Element(string name, Groups group, string locationPath = "", string displayName = "")
    {
        this.name = name;
        this.group = group;
        this.locationPath = locationPath == "" ? name : locationPath;
        this.displayName = displayName == "" ? name : displayName;
    }
}

public enum Groups
{
    HudRoot,
    Inventory,
    Other // Is this enough, or should we just specify everything?
}

public enum ParentRoot
{
    Hud,
    Inventory,
    HudMessage,
    TopLeftMessage,
    Chat,
    EnemyHud,
    Store,
    Menu
}