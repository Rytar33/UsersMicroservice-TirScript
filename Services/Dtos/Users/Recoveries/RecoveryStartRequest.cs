﻿namespace Services.Dtos.Users.Recoveries;

public record RecoveryStartRequest(string Email, string? RequestCode);