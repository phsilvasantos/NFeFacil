﻿using BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes;
using System;

namespace BibliotecaCentral.ItensBD
{
    public sealed class EmitenteDI
    {
        public Guid Id { get; set; }
        public DateTime UltimaData { get; set; }

        public long CNPJ { get; set; }
        public string Nome { get; set; }
        public string NomeFantasia { get; set; }
        public long InscricaoEstadual { get; set; }
        public string IEST { get; set; }
        public string IM { get; set; }
        public string CNAE { get; set; }
        public int RegimeTributario { get; set; }

        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public int CodigoMunicipio { get; set; }
        public string NomeMunicipio { get; set; }
        public string SiglaUF { get; set; }
        public int CEP { get; set; }
        public int CPais { get; set; } = 1058;
        public string XPais { get; set; } = "Brasil";
        public string Telefone { get; set; }

        public EmitenteDI() { }
        public EmitenteDI(Emitente other)
        {
            CNPJ = other.CNPJ;
            Nome = other.Nome;
            NomeFantasia = other.NomeFantasia;
            InscricaoEstadual = other.InscricaoEstadual;
            IEST = other.IEST;
            IM = other.IM;
            CNAE = other.CNAE;
            RegimeTributario = other.RegimeTributario;

            Logradouro = other.Endereco.Logradouro;
            Numero = other.Endereco.Numero;
            Complemento = other.Endereco.Complemento;
            Bairro = other.Endereco.Bairro;
            CodigoMunicipio = other.Endereco.CodigoMunicipio;
            NomeMunicipio = other.Endereco.NomeMunicipio;
            SiglaUF = other.Endereco.SiglaUF;
            CEP = int.Parse(other.Endereco.CEP);
            CPais = other.Endereco.CPais;
            XPais = other.Endereco.XPais;
            Telefone = other.Endereco.Telefone;
        }

        public Emitente ToEmitente()
        {
            return new Emitente
            {
                CNPJ = CNPJ,
                Nome = Nome,
                NomeFantasia = NomeFantasia,
                InscricaoEstadual = InscricaoEstadual,
                IEST = IEST,
                IM = IM,
                CNAE = CNAE,
                RegimeTributario = RegimeTributario,
                Endereco = new ModeloXML.PartesProcesso.PartesNFe.EnderecoCompleto
                {
                    Logradouro = Logradouro,
                    Numero = Numero,
                    Complemento = Complemento,
                    Bairro = Bairro,
                    CodigoMunicipio = CodigoMunicipio,
                    NomeMunicipio = NomeMunicipio,
                    SiglaUF = SiglaUF,
                    CEP = CEP.ToString(),
                    CPais = CPais,
                    XPais = XPais,
                    Telefone = Telefone
                }
            };
        }
    }
}
