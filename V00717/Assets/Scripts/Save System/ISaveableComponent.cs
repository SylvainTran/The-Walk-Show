public interface ISaveableComponent
{
    // Save changes to file (/colonists.txt) and exit colonist creation menu
    public void Save(bool checkMaxElements); // check for max elements before saving?
}