namespace Lab;

public interface ICrudRepository<in T>
{
    void LoadData();
    void InsertRecord(T record);
    void UpdateRecord(T record);
    void DeleteRecord(T record);
}
