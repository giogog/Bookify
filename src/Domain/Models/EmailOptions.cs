namespace Domain.Models;

public class EmailOptions
{
    public required string MailServer { get; set; }
    public required int MailPort { get; set; }
    public required string SenderName { get; set; }
    public required string FromEmail { get; set; }
    public required string Password { get; set; }

}
