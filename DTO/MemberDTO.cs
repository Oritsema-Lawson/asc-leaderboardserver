namespace APCServer.DTO;

public class MemberDTO {
    public string Username { get; } = String.Empty;
    public int Points { get; }

    public MemberDTO(string username, int points)
    {
        Username = username;
        Points = points;
    }
}

// Purely GAY HOMOSEXUAL GAY GAY