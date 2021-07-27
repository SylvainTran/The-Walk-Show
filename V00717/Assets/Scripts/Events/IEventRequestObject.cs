
public interface IEventRequestObject
{
    // The number of alive request objects sent back
    public int ElementCount();

    // The max of alive request objects
    public int Max();

    // Key
    public string Key();

    // Adds elements to array
    public void AddElement(Element element);
}
