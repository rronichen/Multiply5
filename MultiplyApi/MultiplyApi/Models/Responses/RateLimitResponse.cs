namespace MultiplyApi.Models.Responses;

public record RateLimitResponse(int RetryAfterSeconds, DateTime AvailableAt);