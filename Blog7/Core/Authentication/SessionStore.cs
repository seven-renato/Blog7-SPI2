namespace Blog7.Core.Authentication;

public static class SessionStore
{
    private static readonly Dictionary<string, int> _sessions = new();

    public static void AddSession(string sessionId, int userId)
    {
        _sessions[sessionId] = userId;
    }

    public static bool TryGetUserId(string sessionId, out int userId)
    {
        return _sessions.TryGetValue(sessionId, out userId);
    }

    public static void RemoveSession(string sessionId)
    {
        _sessions.Remove(sessionId);
    }
}