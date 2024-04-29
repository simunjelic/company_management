using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.Enums
{
    public static class Extensions
    {
        public static string Describe(this AvailableActions action)
        {
            switch (action)
            {
                case AvailableActions.Uredi:
                    return "Uredi";
                case AvailableActions.Obriši:
                    return "Obriši";
                case AvailableActions.Dodaj:
                    return "Dodaj";
                case AvailableActions.Čitaj:
                    return "Čitaj";
                default:
                    return string.Empty;
            }
        }
    }
}
