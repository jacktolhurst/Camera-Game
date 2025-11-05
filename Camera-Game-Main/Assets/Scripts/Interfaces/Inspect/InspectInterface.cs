public interface Inspect
{
    bool inspecting { get; }

    void Inspect();
    void CloseInspection();

}