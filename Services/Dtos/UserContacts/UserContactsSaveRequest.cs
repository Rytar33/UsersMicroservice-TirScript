﻿namespace Services.Dtos.UserContacts;

public record UserContactsSaveRequest(int UserId, List<UserContactItem> Contacts);