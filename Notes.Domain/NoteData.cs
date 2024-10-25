namespace Notes.Domain;

public class NoteData
{
    public required Guid Id { get; init; }
    public Guid NoteId { get; init; }
    public required string Key { get; init; }
    public required string Value { get; init; }
}