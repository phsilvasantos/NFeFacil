// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace System.Security.Cryptography.Xml
{
    internal class CanonicalXml
    {
        private CanonicalXmlDocument _c14nDoc;
        private C14NAncestralNamespaceContextManager _ancMgr;

        // private static string defaultXPathWithoutComments = "(//. | //@* | //namespace::*)[not(self::comment())]";
        // private static string defaultXPathWithoutComments = "(//. | //@* | //namespace::*)";
        // private static string defaultXPathWithComments = "(//. | //@* | //namespace::*)";
        // private static string defaultXPathWithComments = "(//. | //@* | //namespace::*)";

        internal CanonicalXml(XmlDocument document) : this(document, false) { }
        internal CanonicalXml(XmlDocument document, bool includeComments)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            _c14nDoc = new CanonicalXmlDocument(true, includeComments);
            _c14nDoc.LoadXml(document.OuterXml);
            _ancMgr = new C14NAncestralNamespaceContextManager();
        }

        internal CanonicalXml(XmlNodeList nodeList, bool includeComments)
        {
            if (nodeList == null)
                throw new ArgumentNullException(nameof(nodeList));

            XmlDocument doc = Utils.GetOwnerDocument(nodeList);
            if (doc == null)
                throw new ArgumentException("nodeList");

            _c14nDoc = new CanonicalXmlDocument(false, includeComments);
            _c14nDoc.LoadXml(doc.OuterXml);
            _ancMgr = new C14NAncestralNamespaceContextManager();

            MarkInclusionStateForNodes(nodeList, doc, _c14nDoc);
        }

        private static void MarkNodeAsIncluded(XmlNode node)
        {
            if (node is ICanonicalizableNode)
                ((ICanonicalizableNode)node).IsInNodeSet = true;
        }

        private static void MarkInclusionStateForNodes(XmlNodeList nodeList, XmlDocument inputRoot, XmlDocument root)
        {
            CanonicalXmlNodeList elementList = new CanonicalXmlNodeList();
            CanonicalXmlNodeList elementListCanonical = new CanonicalXmlNodeList();
            elementList.Add(inputRoot);
            elementListCanonical.Add(root);
            int index = 0;

            do
            {
                XmlNode currentNode = (XmlNode)elementList[index];
                XmlNode currentNodeCanonical = (XmlNode)elementListCanonical[index];
                XmlNodeList childNodes = currentNode.ChildNodes;
                XmlNodeList childNodesCanonical = currentNodeCanonical.ChildNodes;
                for (int i = 0; i < childNodes.Count; i++)
                {
                    elementList.Add(childNodes[i]);
                    elementListCanonical.Add(childNodesCanonical[i]);

                    if (Utils.NodeInList(childNodes[i], nodeList))
                    {
                        MarkNodeAsIncluded(childNodesCanonical[i]);
                    }

                    XmlAttributeCollection attribNodes = childNodes[i].Attributes;
                    if (attribNodes != null)
                    {
                        for (int j = 0; j < attribNodes.Count; j++)
                        {
                            if (Utils.NodeInList(attribNodes[j], nodeList))
                            {
                                MarkNodeAsIncluded(childNodesCanonical[i].Attributes.Item(j));
                            }
                        }
                    }
                }
                index++;
            } while (index < elementList.Count);
        }

        internal byte[] GetBytes()
        {
            StringBuilder sb = new StringBuilder();
            _c14nDoc.Write(sb, _ancMgr);
            UTF8Encoding utf8 = new UTF8Encoding(false);
            return utf8.GetBytes(sb.ToString());
        }

        internal byte[] GetDigestedBytes(HashAlgorithm hash)
        {
            var bytes = new List<byte>();
            _c14nDoc.WriteHash(hash, _ancMgr, bytes);
            byte[] dados = bytes.ToArray();
            return hash.ComputeHash(dados, 0, dados.Length);
        }
    }
}
