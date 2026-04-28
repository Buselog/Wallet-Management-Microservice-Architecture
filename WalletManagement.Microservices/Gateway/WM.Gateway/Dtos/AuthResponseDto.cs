namespace WM.Gateway.Dtos
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string CustomerNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
