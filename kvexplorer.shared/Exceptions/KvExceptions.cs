using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kvexplorer.shared.Exceptions;

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