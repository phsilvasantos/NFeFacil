﻿using System.Security.Cryptography;

namespace ServidorCertificacao.Primitivos
{
    public struct CertificadoAssinatura
    {
        public RSA ChavePrivada { get; set; }
        public byte[] RawData { get; set; }
        public string XML { get; set; }
        public string Tag { get; set; }
        public string Id { get; set; }
    }
}
