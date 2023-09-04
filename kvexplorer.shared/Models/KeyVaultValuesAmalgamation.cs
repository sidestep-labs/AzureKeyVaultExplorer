using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kvexplorer.shared.Models;

public class KeyVaultValuesAmalgamation
{

    KeyVaultItemType Type { get; set; }

}



[Flags]
public enum KeyVaultItemType
{
    Certificate, 
    Secret,
    Key
}