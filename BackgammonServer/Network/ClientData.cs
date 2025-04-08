using Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonServer.Network
{
    internal class ClientData
    {
        public RSAEncryption RSAEncryption;
        public AESEncryption AESEncryption;
    }
}
