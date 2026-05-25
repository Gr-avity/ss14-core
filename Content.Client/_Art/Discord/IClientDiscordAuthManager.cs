namespace Content.Client._Art;

public interface IClientDiscordOAuthManager
{
    void RequestLink();

    bool ContainsAny(ulong[] roles);

    void Initialize();
}