namespace FreightAccounting.Core.Model.Users;

public record UpdateUserPasswordModel
{
    public string OldPassword { get; set; } = string.Empty;

    public string NewPassowrd { get; set; } = string.Empty;
}
