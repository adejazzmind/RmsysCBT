namespace RMSYSCBT.Models.Entities;

public class Question
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;

    public Guid CorrectOptionId { get; set; }
    public List<Option> Options { get; set; } = new();
}
