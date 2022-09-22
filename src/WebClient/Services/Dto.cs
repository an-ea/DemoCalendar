namespace WebClient.Services;

public record NewEventDto(string Subject, string Description, DateTime Begin, DateTime End);

public record EventDto(int Id, string Subject, string Description, DateTime Begin, DateTime End)
    : NewEventDto(Subject, Description, Begin, End);

public record NewEventInfoDto(int Id);