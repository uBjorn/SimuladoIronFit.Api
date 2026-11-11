namespace SimuladoIronFit.Api.Security
{
    public class AdminCredenciais
    {
        public string Email { get; init; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string DisplayName { get; init; } = "Administração IronFit";
    }
}
