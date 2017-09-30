﻿using System;

namespace NFeFacil.ItensBD
{
    public sealed class Imagem
    {
        public Guid Id { get; set; }
        public DateTime UltimaData { get; set; }
        public byte[] Bytes { get; set; }
    }
}
