using System;

namespace KeyVaultExplorer.Exceptions;

public class KeyVaultItemNotFoundException : Exception
{
    public KeyVaultItemNotFoundException()
    {
    }

    public KeyVaultItemNotFoundException(string message)
        : base(message)
    {
    }

    public KeyVaultItemNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class KeyVaultItemNotFailedToUpdate : Exception
{
    public KeyVaultItemNotFailedToUpdate()
    {
    }

    public KeyVaultItemNotFailedToUpdate(string message)
        : base(message)
    {
    }

    public KeyVaultItemNotFailedToUpdate(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class KeyVaultInSufficientPrivileges : Exception
{
    public KeyVaultInSufficientPrivileges()
    {
    }

    public KeyVaultInSufficientPrivileges(string message)
        : base(message)
    {
    }

    public KeyVaultInSufficientPrivileges(string message, Exception inner)
        : base(message, inner)
    {
    }
}