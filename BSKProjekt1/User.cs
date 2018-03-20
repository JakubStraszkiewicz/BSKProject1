using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BSKProject1
{
    class User
    {
        public string name;
        public byte[] sessionKey;
        public RSAParameters publicKey;
        public RSAParameters privateKey;
    }
}
