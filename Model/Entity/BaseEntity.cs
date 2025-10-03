namespace Entity;

public class BaseEntity
{
    public DateTime CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public string? Creator { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string? Updater { get; set; }
    public bool IsDelete { get; set; } = false;
}