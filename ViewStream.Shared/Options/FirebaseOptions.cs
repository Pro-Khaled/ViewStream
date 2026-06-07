using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Shared.Options
{
    public class FirebaseOptions
    {
        public const string SectionName = "Firebase";
        public string CredentialPath { get; set; } = "firebase-credentials.json";
    }
}
