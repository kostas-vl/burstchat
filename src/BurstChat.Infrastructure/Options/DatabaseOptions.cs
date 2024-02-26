namespace BurstChat.Infrastructure.Options;

public class DatabaseOptions
{
    public string Provider { get; set; } = string.Empty;

    public string ConnectionString { get; set; } = string.Empty;
}
