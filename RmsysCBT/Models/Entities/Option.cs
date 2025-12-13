namespace RMSYSCBT.Models.Entities;

public class Option
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;

    public Guid QuestionId { get; set; }
}
