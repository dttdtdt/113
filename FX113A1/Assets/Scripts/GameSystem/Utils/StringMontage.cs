using System.Collections.Generic;
using System.Linq;

namespace GameSystem.Utils
{
    public static class StringMontage
    {
        public static string Montage(IEnumerable<string> description)
        {
            if (description == null) return string.Empty;
            return string.Join("\n", description.Where(text => !string.IsNullOrWhiteSpace(text)));
        }
    }
}
