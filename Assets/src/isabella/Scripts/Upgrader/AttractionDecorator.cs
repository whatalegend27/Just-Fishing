public abstract class AttractionDecorator : IFishAttraction
{
    protected IFishAttraction wrapped;

    // Constructor to set the wrapped attraction, allowing for dynamic addition of behaviors
    public AttractionDecorator(IFishAttraction attraction)
    {
        wrapped = attraction;
    }

    // Virtual method to get the attraction value, which can be overridden by subclasses to modify behavior
    public virtual int GetAttraction()
    {
        return wrapped.GetAttraction();
    }
}