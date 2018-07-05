﻿using BaseGeral.View;
using BaseGeral.ModeloXML.PartesDetalhes.PartesProduto.PartesImpostos;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using static BaseGeral.ExtensoesPrincipal;
using System.Reflection;

namespace BaseGeral.ModeloXML.PartesDetalhes.PartesTotal
{
    public sealed class ICMSTot
    {
        public ICMSTot() { }
        /// <summary>
        /// Calcula todos os valores automaticamente com base nos produtos da nota
        /// </summary>
        /// <param name="produtos">Lista com os produtos</param>
        public ICMSTot(IEnumerable<DetalhesProdutos> produtos)
        {
            foreach (var Produto in produtos)
            {
                var temISSQN = false;
                var prod = Produto.Produto;
                foreach (var imposto in Produto.Impostos.impostos)
                {
                    if (imposto is ISSQN)
                    {
                        temISSQN = true;
                    }
                    else
                    {
                        if (imposto is ICMS icms)
                        {
                            var corpo = icms.Corpo;
                            var tipo = corpo.GetType();
                            vBC += AgregarValor(nameof(vBC));
                            vICMS += AgregarValor(nameof(vICMS));
                            vICMSDeson += AgregarValor(nameof(vICMSDeson));
                            vFCPUFDest += AgregarValor(nameof(vFCPUFDest));
                            vICMSUFDest += AgregarValor(nameof(vICMSUFDest));
                            vICMSUFRemet += AgregarValor(nameof(vICMSUFRemet));
                            vBCST += AgregarValor(nameof(vBCST));
                            vST += AgregarValor("vICMSST");

                            double AgregarValor(string nomeElemento)
                            {
                                var valor = tipo.GetProperty(nomeElemento)?.GetValue(corpo) ?? 0d;
                                return (double)valor;
                            }
                        }
                        else if (imposto is II)
                        {
                            vII += Parse((imposto as II).vII);
                        }
                        else if (imposto is IPI ipi && ipi.Corpo is IPITrib trib)
                        {
                            var temp = trib.vIPI;
                            vIPI += string.IsNullOrEmpty(temp) ? 0 : Parse(temp);
                        }
                        else if (imposto is PIS pis)
                        {
                            if (pis.Corpo is PISAliq aliq)
                            {
                                vPIS += Parse(aliq.vPIS);
                            }
                            else if (pis.Corpo is PISQtde qtde)
                            {
                                vPIS += Parse(qtde.vPIS);
                            }
                            else if (pis.Corpo is PISOutr outr)
                            {
                                vPIS += Parse(outr.vPIS);
                            }
                        }
                        else if (imposto is COFINS cofins)
                        {
                            if (cofins.Corpo is COFINSAliq aliq)
                            {
                                vCOFINS += Parse(aliq.vCOFINS);
                            }
                            else if (cofins.Corpo is COFINSQtde qtde)
                            {
                                vCOFINS += Parse(qtde.vCOFINS);
                            }
                            else if (cofins.Corpo is COFINSOutr outr)
                            {
                                vCOFINS += Parse(outr.vCOFINS);
                            }
                        }
                    }
                }
                if (prod.InclusaoTotal == 1 && !temISSQN)
                {
                    vProd += prod.ValorTotal;
                    TryParse(prod.Frete, out double addFrete);
                    vFrete += addFrete;
                    TryParse(prod.Seguro, out double addSeguro);
                    vSeg += addSeguro;
                    TryParse(prod.Desconto, out double addDesconto);
                    vDesc += addDesconto;
                    TryParse(prod.DespesasAcessorias, out double addOutro);
                    vOutro += addOutro;
                    TryParse(Produto.Impostos.vTotTrib, out double addTotTrib);
                    vTotTrib += addTotTrib;
                }
                else if (temISSQN)
                {
                    vProdISSQN += prod.ValorTotal;
                }
            }
        }
        
        [XmlIgnore]
        public double vBC;
        [XmlElement("vBC", Order = 0), DescricaoPropriedade("Somatório da BC do ICMS")]
        public string VBC { get => ToStr(vBC); set => vBC = Parse(value); }

        [XmlIgnore]
        public double vICMS;
        [XmlElement("vICMS", Order = 1), DescricaoPropriedade("Somatório de ICMS")]
        public string VICMS { get => ToStr(vICMS); set => vICMS = Parse(value); }

        [XmlIgnore]
        public double vICMSDeson;
        [XmlElement("vICMSDeson", Order = 2), DescricaoPropriedade("Somatório do ICMS desonerado")]
        public string VICMSDeson { get => ToStr(vICMSDeson); set => vICMSDeson = Parse(value); }

        [XmlIgnore]
        public double vFCPUFDest;
        [XmlElement("vFCPUFDest", Order = 3), DescricaoPropriedade("Somatório do ICMS relativo ao FCP da UF destino")]
        public string VFCPUFDest { get => ToStr(vFCPUFDest); set => vFCPUFDest = Parse(value); }

        [XmlIgnore]
        public double vICMSUFDest;
        [XmlElement("vICMSUFDest", Order = 4), DescricaoPropriedade("Somatório do ICMS interestadual para a UF destino")]
        public string VICMSUFDest { get => ToStr(vICMSUFDest); set => vICMSUFDest = Parse(value); }

        [XmlIgnore]
        public double vICMSUFRemet;
        [XmlElement("vICMSUFRemet", Order = 5), DescricaoPropriedade("Somatório do ICMS interestadual para a UF remetente")]
        public string VICMSUFRemet { get => ToStr(vICMSUFRemet); set => vICMSUFRemet = Parse(value); }

        [XmlIgnore]
        public double vBCST;
        [XmlElement("vBCST", Order = 6), DescricaoPropriedade("Somatório da BC ST")]
        public string VBCST { get => ToStr(vBCST); set => vBCST = Parse(value); }

        [XmlIgnore]
        public double vST;
        [XmlElement("vST", Order = 7), DescricaoPropriedade("Somatório do ICMS ST")]
        public string VST { get => ToStr(vST); set => vST = Parse(value); }

        [XmlIgnore]
        public double vProd;
        [XmlElement("vProd", Order = 8), DescricaoPropriedade("Somatório do valor dos produtos")]
        public string VProd { get => ToStr(vProd); set => vProd = Parse(value); }

        [XmlIgnore]
        public double vFrete;
        [XmlElement("vFrete", Order = 9), DescricaoPropriedade("Somatório de valor do Frete")]
        public string VFrete { get => ToStr(vFrete); set => vFrete = Parse(value); }

        [XmlIgnore]
        public double vSeg;
        [XmlElement("vSeg", Order = 10), DescricaoPropriedade("Somatório valor do seguro")]
        public string VSeg { get => ToStr(vSeg); set => vSeg = Parse(value); }

        [XmlIgnore]
        public double vDesc;
        [XmlElement("vDesc", Order = 11), DescricaoPropriedade("Somatório do desconto")]
        public string VDesc { get => ToStr(vDesc); set => vDesc = Parse(value); }

        [XmlIgnore]
        public double vII;
        [XmlElement("vII", Order = 12), DescricaoPropriedade("Somatório de II")]
        public string VII { get => ToStr(vII); set => vII = Parse(value); }

        [XmlIgnore]
        public double vIPI;
        [XmlElement("vIPI", Order = 13), DescricaoPropriedade("Somatório de IPI")]
        public string VIPI { get => ToStr(vIPI); set => vIPI = Parse(value); }

        double vPIS;
        [XmlElement("vPIS", Order = 14), DescricaoPropriedade("Somatório de PIS")]
        public string VPIS { get => ToStr(vPIS); set => vPIS = Parse(value); }

        double vCOFINS;
        [XmlElement("vCOFINS", Order = 15), DescricaoPropriedade("Somatório de COFINS")]
        public string VCOFINS { get => ToStr(vCOFINS); set => vCOFINS = Parse(value); }

        [XmlIgnore]
        public double vOutro;
        [XmlElement("vOutro", Order = 16), DescricaoPropriedade("Somatório dos valores adicionais")]
        public string VOutro { get => ToStr(vOutro); set => vOutro = Parse(value); }

        public double vNF
        {
            get
            {
                var valores = new double[] { vProd, vST, vFrete, vSeg, vOutro, vII, vIPI, vProdISSQN };
                return valores.Sum() - (vDesc + vICMSDeson);
            }
        }

        [XmlElement("vNF", Order = 17), DescricaoPropriedade("Valor total da NF")]
        public string VNF
        {
            get => ToStr(vNF);
            set { }
        }

        [XmlIgnore]
        double vTotTrib;
        [XmlElement("vTotTrib", Order = 18), DescricaoPropriedade("Valor total dos tributos")]
        public string VTotTrib { get => ToStr(vTotTrib); set => vTotTrib = Parse(value); }

        double vProdISSQN;
    }
}