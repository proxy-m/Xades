// XadesSignedXml.cs

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Microsoft.Xades
	{
	public class XadesSignedXml : System.Security.Cryptography.Xml.SignedXml
		{
		#region Constants

		/// <summary>
		/// The XAdES XML namespace URI
		/// </summary>
		public const string XadesNamespaceUri = "http://uri.etsi.org/01903/v1.3.2#";

		/// <summary>
		/// Mandated type name for the Uri reference to the SignedProperties element
		/// </summary>
		public const string SignedPropertiesType = "http://uri.etsi.org/01903#SignedProperties";

		#endregion Constants

		#region Private variables

		private static readonly string[] idAttrs = new string[]
		{
			"_id",
			"_Id",
			"_ID"
		};

		private XmlDocument cachedXadesObjectDocument;
		private string signedPropertiesIdBuffer;
		private string signatureValueId;

		#endregion Private variables

		#region Public properties

		/// <summary>
		/// Setting this property will add an ID attribute to the SignatureValue element. This is
		/// required when constructing a XAdES-T signature.
		/// </summary>
		public string SignatureValueId
			{
			get
				{
				return this.signatureValueId;
				}

			set
				{
				this.signatureValueId = value;
				}
			}

		#endregion Public properties

		#region Constructors

		/// <summary>
		/// Default constructor for the XadesSignedXml class
		/// </summary>
		public XadesSignedXml() : base()
			{
			this.cachedXadesObjectDocument = null;
			}

		/// <summary>
		/// Constructor for the XadesSignedXml class
		/// </summary>
		/// <param name="signatureElement">XmlElement used to create the instance</param>
		public XadesSignedXml(XmlElement signatureElement) : base(signatureElement)
			{
			this.cachedXadesObjectDocument = null;
			}

		/// <summary>
		/// Constructor for the XadesSignedXml class
		/// </summary>
		/// <param name="signatureDocument">XmlDocument used to create the instance</param>
		public XadesSignedXml(System.Xml.XmlDocument signatureDocument) : base(signatureDocument)
			{
			this.cachedXadesObjectDocument = null;
			}

		#endregion Constructors

		#region Public methods

		/// <summary>
		/// Returns the XML representation of the this object Не удалять!!!
		/// </summary>
		/// <returns>XML element containing the state of this object</returns>
		public new XmlElement GetXml()
			{
			XmlElement retVal;
			XmlNodeList xmlNodeList;
			XmlNamespaceManager xmlNamespaceManager;

			retVal = base.GetXml();
			if (this.signatureValueId != null && this.signatureValueId != "")
				{ //Id on Signature value is needed for XAdES-T. We inject it here.
				xmlNamespaceManager = new XmlNamespaceManager(retVal.OwnerDocument.NameTable);
				xmlNamespaceManager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
				xmlNodeList = retVal.SelectNodes("ds:SignatureValue", xmlNamespaceManager);
				if (xmlNodeList.Count > 0)
					{
					((XmlElement) xmlNodeList[0]).SetAttribute("Id", this.signatureValueId);
					}
				}

			// Add "ds" namespace prefix to all XmlDsig nodes in the signature
			SetPrefix("ds", retVal);

			return retVal;
			}

		/// <summary>
		/// Overridden virtual method to be able to find the nested SignedProperties element inside
		/// of the XAdES object
		/// </summary>
		/// <param name="xmlDocument">Document in which to find the Id</param>
		/// <param name="idValue">Value of the Id to look for</param>
		/// <returns>XmlElement with requested Id</returns>
		public override XmlElement GetIdElement(XmlDocument xmlDocument, string idValue)
			{
			// check to see if it's a standard ID reference
			XmlElement retVal = null;

			if (Signature != null && Signature.SignedInfo != null && Signature.SignatureValue != null)
				{
				var signature = new XmlDocument();
				signature.AppendChild(signature.ImportNode(Signature.GetXml(), true));
				signature.DocumentElement.SetAttribute("xmlns:ds", SignedXml.XmlDsigNamespaceUrl);
				retVal = base.GetIdElement(signature, idValue);
				if (retVal != null)
					{
					return retVal;
					}

				// if not, search for custom ids
				foreach (string idAttr in idAttrs)
					{
					retVal = signature.SelectSingleNode("//*[@" + idAttr + "=\"" + idValue + "\"]") as XmlElement;
					if (retVal != null)
						{
						return retVal;
						}
					}
				}

			if (idValue == this.signedPropertiesIdBuffer)
				{
				var xmlDocumentCloned = new XmlDocument();
				xmlDocumentCloned.LoadXml(xmlDocument.OuterXml);

				var signedDataContainer = this.GetIdElement(xmlDocumentCloned, "signed-data-container");
				signedDataContainer.InsertBefore(xmlDocumentCloned.ImportNode(cachedXadesObjectDocument.DocumentElement, true), signedDataContainer.FirstChild);

				//xmlDocumentCloned.DocumentElement.AppendChild(xmlDocumentCloned.ImportNode(cachedXadesObjectDocument.DocumentElement, true));

				retVal = base.GetIdElement(xmlDocumentCloned, idValue);
				if (retVal != null)
					{
					return retVal;
					}

				// if not, search for custom ids
				foreach (string idAttr in idAttrs)
					{
					retVal = this.cachedXadesObjectDocument.SelectSingleNode("//*[@" + idAttr + "=\"" + idValue + "\"]") as XmlElement;
					if (retVal != null)
						{
						break;
						}
					}
				}
			else
				{
				if (xmlDocument != null)
					{
					retVal = base.GetIdElement(xmlDocument, idValue);
					if (retVal != null)
						{
						return retVal;
						}

					// if not, search for custom ids
					foreach (string idAttr in idAttrs)
						{
						retVal = xmlDocument.SelectSingleNode("//*[@" + idAttr + "=\"" + idValue + "\"]") as XmlElement;
						if (retVal != null)
							{
							break;
							}
						}
					}
				}

			return retVal;
			}

		/// <summary>
		/// Add a XAdES object to the signature
		/// </summary>
		/// <param name="xadesObject">XAdES object to add to signature</param>
		public void AddXadesObject(XadesObject xadesObject)
			{
			Reference reference;
			DataObject dataObject;
			XmlElement bufferXmlElement;

			dataObject = new DataObject();
			dataObject.Id = xadesObject.Id;
			dataObject.Data = xadesObject.GetXml().ChildNodes;
			this.AddObject(dataObject); //Add the XAdES object

			reference = new Reference();
			reference.DigestMethod = "http://www.w3.org/2001/04/xmldsig-more#gostr3411";
			signedPropertiesIdBuffer = xadesObject.QualifyingProperties.SignedProperties.Id;
			reference.Uri = "#" + signedPropertiesIdBuffer;
			reference.Type = SignedPropertiesType;
			this.AddReference(reference); //Add the XAdES object reference

			this.cachedXadesObjectDocument = new XmlDocument();
			bufferXmlElement = xadesObject.GetXml();

			// Add "ds" namespace prefix to all XmlDsig nodes in the XAdES object
			SetPrefix("ds", bufferXmlElement);

			this.cachedXadesObjectDocument.PreserveWhitespace = true;
			this.cachedXadesObjectDocument.LoadXml(bufferXmlElement.OuterXml); //Cache to XAdES object for later use
			}

		#endregion Public methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="node"></param>
		private void SetPrefix(String prefix, XmlNode node)
			{
			if (node.NamespaceURI == SignedXml.XmlDsigNamespaceUrl)
				{
				node.Prefix = prefix;
				}

			foreach (XmlNode child in node.ChildNodes)
				{
				SetPrefix(prefix, child);
				}

			return;
			}

		/// <summary>
		/// Copy of System.Security.Cryptography.Xml.SignedXml.ComputeSignature() which will end up
		/// calling our own GetC14NDigest with a namespace prefix for all XmlDsig nodes
		/// </summary>
		public void ComputeSignature(Func<byte[], byte[]> ExternalHasher, Func<byte[], byte[]> ExternalSigner,bool ReverseSignature)
			{
			byte[] bSignedInfoHash;
			bSignedInfoHash = GetSignedInfoHash(ExternalHasher);
			byte[] bSignature = ExternalSigner(bSignedInfoHash);

			if (ReverseSignature)
				{
				Array.Reverse(bSignature);
				}

			this.m_signature.SignatureValue = bSignature;
			}


		/// <summary>
		/// Calculates standalone SignedInfo hash
		/// </summary>
		/// <param name="ExternalHasher"></param>
		public byte[] GetSignedInfoHash(Func<byte[], byte[]> ExternalHasher)
			{
			byte[] SignedInfoHash;
			BuildDigestedReferences(ExternalHasher);
			SignedInfoHash = GetC14NDigest(ExternalHasher, "ds");
			return SignedInfoHash;
			}

		/// <summary>
		/// Copy of System.Security.Cryptography.Xml.SignedXml.BuildDigestedReferences() which will
		/// add a "ds" namespace prefix to all XmlDsig nodes
		/// </summary>
		private void BuildDigestedReferences(Func<byte[], byte[]> ExternalHasher)
			{
			ArrayList references = this.SignedInfo.References;

			//this.m_refProcessed = new bool[references.Count];
			Type SignedXml_Type = typeof(SignedXml);
			FieldInfo SignedXml_m_refProcessed = SignedXml_Type.GetField("m_refProcessed", BindingFlags.NonPublic | BindingFlags.Instance);
			SignedXml_m_refProcessed.SetValue(this, new bool[references.Count]);

			//this.m_refLevelCache = new int[references.Count];
			FieldInfo SignedXml_m_refLevelCache = SignedXml_Type.GetField("m_refLevelCache", BindingFlags.NonPublic | BindingFlags.Instance);
			SignedXml_m_refLevelCache.SetValue(this, new int[references.Count]);

			//ReferenceLevelSortOrder comparer = new ReferenceLevelSortOrder();
			Assembly System_Security_Assembly = Assembly.Load("System.Security, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
			Type ReferenceLevelSortOrder_Type = System_Security_Assembly.GetType("System.Security.Cryptography.Xml.SignedXml+ReferenceLevelSortOrder");
			ConstructorInfo ReferenceLevelSortOrder_Constructor = ReferenceLevelSortOrder_Type.GetConstructor(new Type[] { });
			Object comparer = ReferenceLevelSortOrder_Constructor.Invoke(null);

			//comparer.References = references;
			PropertyInfo ReferenceLevelSortOrder_References = ReferenceLevelSortOrder_Type.GetProperty("References", BindingFlags.Public | BindingFlags.Instance);
			ReferenceLevelSortOrder_References.SetValue(comparer, references, null);

			ArrayList list2 = new ArrayList();
			foreach (Reference reference in references)
				{
				list2.Add(reference);
				}

			list2.Sort((IComparer) comparer);

			//CanonicalXmlNodeList refList = new CanonicalXmlNodeList();
			Type CanonicalXmlNodeList_Type = System_Security_Assembly.GetType("System.Security.Cryptography.Xml.CanonicalXmlNodeList");
			ConstructorInfo CanonicalXmlNodeList_Constructor = CanonicalXmlNodeList_Type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);
			Object refList = CanonicalXmlNodeList_Constructor.Invoke(null);

			//
			MethodInfo CanonicalXmlNodeList_Add = CanonicalXmlNodeList_Type.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);

			foreach (DataObject obj2 in this.m_signature.ObjectList)
				{
				XmlElement xml = obj2.GetXml();

				SetPrefix("ds", xml); // <---

				CanonicalXmlNodeList_Add.Invoke(refList, new object[] { xml });
				}

			//
			FieldInfo SignedXml_m_containingDocument = SignedXml_Type.GetField("m_containingDocument", BindingFlags.NonPublic | BindingFlags.Instance);
			Type Reference_Type = typeof(Reference);
			MethodInfo Reference_UpdateHashValue = Reference_Type.GetMethod("UpdateHashValue", BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (Reference reference2 in list2)
				{
				if (reference2.DigestMethod == null)
					{
					reference2.DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";
					}

				object m_containingDocument = SignedXml_m_containingDocument.GetValue(this);

				// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Обновляется значение хэша
				// Reference_UpdateHashValue.Invoke(reference2, new object[] { m_containingDocument,
				// refList }); !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

				______UpdateHashValue(reference2, m_containingDocument as XmlDocument, refList as XmlNodeList, ExternalHasher);

				if (reference2.Id != null)
					{
					//refList.Add(reference2.GetXml());
					XmlElement xml = reference2.GetXml();
					SetPrefix("ds", xml); // <---
					CanonicalXmlNodeList_Add.Invoke(refList, new object[] { xml });
					}
				}
			}

		internal void ______UpdateHashValue(Reference reference, XmlDocument document, XmlNodeList refList, Func<byte[], byte[]> ExternalHasher)
			{
			reference.DigestValue = ______CalculateHashValue(reference, document, refList, ExternalHasher);
			}

		// https://referencesource.microsoft.com/#System.Security/system/security/cryptography/xml/reference.cs,b30e729b1c224492,references

		// What we want to do is pump the input throug the TransformChain and then hash the output of
		// the chain document is the document context for resolving relative references
		internal byte[] ______CalculateHashValue(Reference reference, XmlDocument document,/* CanonicalXmlNodeList*/XmlNodeList refList, Func<byte[], byte[]> ExternalHasher)
			{
			//// refList is a list of elements that might be targets of references
			//// Now's the time to create our hashing algorithm

			//// Let's go get the target.
			string baseUri = (document == null ? System.Environment.CurrentDirectory + "\\" : document.BaseURI);
			Stream hashInputStream = null;

			XmlResolver resolver = null;
			byte[] hashval = null;

			if (reference.Uri[0] == '#')
				{
				// // If we get here, then we are constructing a Reference to an embedded DataObject
				// // referenced by an Id = attribute. Go find the relevant object
				bool discardComments = true;

				Type Utils_Type = Type.GetType("System.Security.Cryptography.Utils");

				// "signed-data-container"
				string idref = GetIdFromLocalUri(reference.Uri, out discardComments);

				XmlElement elem = GetIdElement(document, idref);

				MethodInfo Reference_GetPropagatedAttributes = Utils_Type.GetMethod("GetPropagatedAttributes", BindingFlags.NonPublic | BindingFlags.Static);

				List<XmlNode> m_namespaces = null;

				if (elem != null)
					m_namespaces = GetPropagatedAttributes(elem.ParentNode as XmlElement);

				XmlDocument normDocument = PreProcessElementInput(elem, resolver, baseUri);

				// // Add the propagated attributes
				AddNamespaces(normDocument.DocumentElement, m_namespaces);

				resolver = new XmlSecureResolver(new XmlUrlResolver(), baseUri);

				if (discardComments)
					{
					XmlDocument docWithNoComments = DiscardComments(normDocument);

					Type TransformChain_Type = typeof(TransformChain);
					MethodInfo Reference_TransformToOctetStream = null;
					MethodInfo[] References = TransformChain_Type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
					foreach (MethodInfo mi in References)
						{
						if (mi.Name == "TransformToOctetStream")
							{
							ParameterInfo[] Params;
							Params = mi.GetParameters();

							if (Params.Length == 3)
								{
								if (Params[0].ParameterType == typeof(XmlDocument))
									{
									Reference_TransformToOctetStream = mi;
									break;
									}
								}
							}
						}

					MemoryStream ss = (MemoryStream) Reference_TransformToOctetStream.Invoke(reference.TransformChain, new object[] { docWithNoComments, resolver, baseUri });
					hashInputStream = ss;
					}
				}

			MemoryStream ms2 = hashInputStream as MemoryStream;
			byte[] bb = ms2.ToArray();

			hashval = ExternalHasher(bb);

			return hashval;
			}

		#region Выдрано из исходников микрософт

		internal static XmlDocument DiscardComments(XmlDocument document)
			{
			XmlNodeList nodeList = document.SelectNodes("//comment()");
			if (nodeList != null)
				{
				foreach (XmlNode node1 in nodeList)
					{
					node1.ParentNode.RemoveChild(node1);
					}
				}
			return document;
			}

		internal static void AddNamespaces(XmlElement elem, List<XmlNode> namespaces)
			{
			if (namespaces != null)
				{
				foreach (XmlNode attrib in namespaces)
					{
					string name = ((attrib.Prefix.Length > 0) ? attrib.Prefix + ":" + attrib.LocalName : attrib.LocalName);

					// Skip the attribute if one with the same qualified name already exists
					if (elem.HasAttribute(name) || (name.Equals("xmlns") && elem.Prefix.Length == 0))
						continue;
					XmlAttribute nsattrib = (XmlAttribute) elem.OwnerDocument.CreateAttribute(name);
					nsattrib.Value = attrib.Value;
					elem.SetAttributeNode(nsattrib);
					}
				}
			}

		internal static XmlDocument PreProcessElementInput(XmlElement elem, XmlResolver xmlResolver, string baseUri)
			{
			if (elem == null)
				throw new ArgumentNullException("elem");

			XmlDocument doc = new XmlDocument();
			doc.PreserveWhitespace = true;

			// Normalize the document
			using (TextReader stringReader = new StringReader(elem.OuterXml))
				{
				XmlReaderSettings settings = new XmlReaderSettings();
				settings.XmlResolver = xmlResolver;
				settings.DtdProcessing = DtdProcessing.Parse;
				settings.MaxCharactersFromEntities = 10000000; // lo
				settings.MaxCharactersInDocument = 0; // unlimited
				XmlReader reader = XmlReader.Create(stringReader, settings, baseUri);
				doc.Load(reader);
				}
			return doc;
			}

		// This method gets the attributes that should be propagated
		internal static List<XmlNode> GetPropagatedAttributes(XmlElement elem)
			{
			if (elem == null)
				return null;

			List<XmlNode> namespaces = new List<XmlNode>();
			XmlNode ancestorNode = elem;

			if (ancestorNode == null)
				return null;

			bool bDefNamespaceToAdd = true;

			while (ancestorNode != null)
				{
				XmlElement ancestorElement = ancestorNode as XmlElement;
				if (ancestorElement == null)
					{
					ancestorNode = ancestorNode.ParentNode;
					continue;
					}
				if (!IsCommittedNamespace(ancestorElement, ancestorElement.Prefix, ancestorElement.NamespaceURI))
					{
					// Add the namespace attribute to the collection if needed
					if (!IsRedundantNamespace(ancestorElement, ancestorElement.Prefix, ancestorElement.NamespaceURI))
						{
						string name = ((ancestorElement.Prefix.Length > 0) ? "xmlns:" + ancestorElement.Prefix : "xmlns");
						XmlAttribute nsattrib = elem.OwnerDocument.CreateAttribute(name);
						nsattrib.Value = ancestorElement.NamespaceURI;
						namespaces.Add(nsattrib);
						}
					}
				if (ancestorElement.HasAttributes)
					{
					XmlAttributeCollection attribs = ancestorElement.Attributes;
					foreach (XmlAttribute attrib in attribs)
						{
						// Add a default namespace if necessary
						if (bDefNamespaceToAdd && attrib.LocalName == "xmlns")
							{
							XmlAttribute nsattrib = elem.OwnerDocument.CreateAttribute("xmlns");
							nsattrib.Value = attrib.Value;
							namespaces.Add(nsattrib);
							bDefNamespaceToAdd = false;
							continue;
							}

						// retain the declarations of type 'xml:*' as well
						if (attrib.Prefix == "xmlns" || attrib.Prefix == "xml")
							{
							namespaces.Add(attrib);
							continue;
							}
						if (attrib.NamespaceURI.Length > 0)
							{
							if (!IsCommittedNamespace(ancestorElement, attrib.Prefix, attrib.NamespaceURI))
								{
								// Add the namespace attribute to the collection if needed
								if (!IsRedundantNamespace(ancestorElement, attrib.Prefix, attrib.NamespaceURI))
									{
									string name = ((attrib.Prefix.Length > 0) ? "xmlns:" + attrib.Prefix : "xmlns");
									XmlAttribute nsattrib = elem.OwnerDocument.CreateAttribute(name);
									nsattrib.Value = attrib.NamespaceURI;
									namespaces.Add(nsattrib);
									}
								}
							}
						}
					}
				ancestorNode = ancestorNode.ParentNode;
				}

			return namespaces;
			}

		// A helper function that determines if a namespace node is a committed attribute
		internal static bool IsCommittedNamespace(XmlElement element, string prefix, string value)
			{
			if (element == null)
				throw new ArgumentNullException("element");

			string name = ((prefix.Length > 0) ? "xmlns:" + prefix : "xmlns");
			if (element.HasAttribute(name) && element.GetAttribute(name) == value)
				return true;
			return false;
			}

		internal static bool IsRedundantNamespace(XmlElement element, string prefix, string value)
			{
			if (element == null)
				throw new ArgumentNullException("element");

			XmlNode ancestorNode = ((XmlNode) element).ParentNode;
			while (ancestorNode != null)
				{
				XmlElement ancestorElement = ancestorNode as XmlElement;
				if (ancestorElement != null)
					if (HasNamespace(ancestorElement, prefix, value))
						return true;
				ancestorNode = ancestorNode.ParentNode;
				}

			return false;
			}

		private static bool HasNamespace(XmlElement element, string prefix, string value)
			{
			if (IsCommittedNamespace(element, prefix, value))
				return true;
			if (element.Prefix == prefix && element.NamespaceURI == value)
				return true;
			return false;
			}

		internal static string GetIdFromLocalUri(string uri, out bool discardComments)
			{
			string idref = uri.Substring(1);

			// initialize the return value
			discardComments = true;

			// Deal with XPointer of type #xpointer(id("ID")). Other XPointer support isn't handled
			// here and is anyway optional
			if (idref.StartsWith("xpointer(id(", StringComparison.Ordinal))
				{
				int startId = idref.IndexOf("id(", StringComparison.Ordinal);
				int endId = idref.IndexOf(")", StringComparison.Ordinal);
				if (endId < 0 || endId < startId + 3)
					{
					throw new InvalidOperationException("Cryptography_Xml_InvalidReference");//CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidReference"));
					}
				idref = idref.Substring(startId + 3, endId - startId - 3);
				idref = idref.Replace("\'", "");
				idref = idref.Replace("\"", "");
				discardComments = false;
				}
			return idref;
			}

		#endregion Выдрано из исходников микрософт

		/// <summary>
		/// Copy of System.Security.Cryptography.Xml.SignedXml.GetC14NDigest() which will add a
		/// namespace prefix to all XmlDsig nodes
		/// </summary>
		private byte[] GetC14NDigest(Func<byte[], byte[]> ExternalHasher, string prefix)
			{
			byte[] SignedInfoHash;

			Type SignedXml_Type = typeof(SignedXml);
			Type SignedInfo_Type = typeof(SignedInfo);

			FieldInfo SignedXml__digestedSignedInfo = SignedXml_Type.GetField("_digestedSignedInfo", BindingFlags.NonPublic | BindingFlags.Instance);

			//string securityUrl = (this.m_containingDocument == null) ? null : this.m_containingDocument.BaseURI;
			FieldInfo SignedXml_m_containingDocument = SignedXml_Type.GetField("m_containingDocument", BindingFlags.NonPublic | BindingFlags.Instance);
			XmlDocument m_containingDocument = (XmlDocument) SignedXml_m_containingDocument.GetValue(this);
			string securityUrl = (m_containingDocument == null) ? null : m_containingDocument.BaseURI;

			//XmlResolver xmlResolver = this.m_bResolverSet ? this.m_xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), securityUrl);
			FieldInfo SignedXml_m_bResolverSet = SignedXml_Type.GetField("m_bResolverSet", BindingFlags.NonPublic | BindingFlags.Instance);
			bool m_bResolverSet = (bool) SignedXml_m_bResolverSet.GetValue(this);
			FieldInfo SignedXml_m_xmlResolver = SignedXml_Type.GetField("m_xmlResolver", BindingFlags.NonPublic | BindingFlags.Instance);
			XmlResolver m_xmlResolver = (XmlResolver) SignedXml_m_xmlResolver.GetValue(this);
			XmlResolver xmlResolver = m_bResolverSet ? m_xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), securityUrl);

			//XmlDocument document = Utils.PreProcessElementInput(this.SignedInfo.GetXml(), xmlResolver, securityUrl);
			Assembly System_Security_Assembly = Assembly.Load("System.Security, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
			Type Utils_Type = System_Security_Assembly.GetType("System.Security.Cryptography.Xml.Utils");
			MethodInfo Utils_PreProcessElementInput = Utils_Type.GetMethod("PreProcessElementInput", BindingFlags.NonPublic | BindingFlags.Static);
			XmlElement xml = this.SignedInfo.GetXml();
			SetPrefix(prefix, xml); // <---
			XmlDocument document = (XmlDocument) Utils_PreProcessElementInput.Invoke(null, new object[] { xml, xmlResolver, securityUrl });

			//CanonicalXmlNodeList namespaces = (this.m_context == null) ? null : Utils.GetPropagatedAttributes(this.m_context);
			//FieldInfo SignedXml_m_context = SignedXml_Type.GetField("m_context", BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo Utils_GetPropagatedAttributes = Utils_Type.GetMethod("GetPropagatedAttributes", BindingFlags.NonPublic | BindingFlags.Static);

			var xmlDocumentCloned = new XmlDocument();
			xmlDocumentCloned.LoadXml(m_containingDocument.OuterXml);

			var signedDataContainer = GetIdElement(xmlDocumentCloned, "signed-data-container");
			signedDataContainer.InsertBefore(xmlDocumentCloned.ImportNode(document.DocumentElement, true), signedDataContainer.FirstChild);

			object namespaces = Utils_GetPropagatedAttributes.Invoke(null, new object[] { signedDataContainer.FirstChild });

			// GetPropagatedAttributes(signedDataContainer.FirstChild);

			//object m_context = SignedXml_m_context.GetValue(this);
			//object namespaces = (m_context == null) ? null : Utils_GetPropagatedAttributes.Invoke(null, new object[] { m_context });
			//

			// Utils.AddNamespaces(document.DocumentElement, namespaces);
			Type CanonicalXmlNodeList_Type = System_Security_Assembly.GetType("System.Security.Cryptography.Xml.CanonicalXmlNodeList");
			MethodInfo Utils_AddNamespaces = Utils_Type.GetMethod("AddNamespaces", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(XmlElement), CanonicalXmlNodeList_Type }, null);
			Utils_AddNamespaces.Invoke(null, new object[] { document.DocumentElement, namespaces });

			// AddNamespaces(document.DocumentElement, namespaces);

			//Transform canonicalizationMethodObject = this.SignedInfo.CanonicalizationMethodObject;
			System.Security.Cryptography.Xml.Transform canonicalizationMethodObject = this.SignedInfo.CanonicalizationMethodObject;


			canonicalizationMethodObject.Resolver = xmlResolver;

			//canonicalizationMethodObject.BaseURI = securityUrl;
			Type Transform_Type = typeof(System.Security.Cryptography.Xml.Transform);
			PropertyInfo Transform_BaseURI = Transform_Type.GetProperty("BaseURI", BindingFlags.NonPublic | BindingFlags.Instance);
			Transform_BaseURI.SetValue(canonicalizationMethodObject, securityUrl, null);



			canonicalizationMethodObject.LoadInput(document);
			MemoryStream outms = (MemoryStream) canonicalizationMethodObject.GetOutput();
			byte[] output = outms.ToArray();

			SignedInfoHash = ExternalHasher(output);

			// SignedInfoDigest = canonicalizationMethodObject.GetDigestedOutput(hash);

			//this._digestedSignedInfo = canonicalizationMethodObject.GetDigestedOutput(hash);
			//SignedXml__digestedSignedInfo.SetValue(this, SignedInfoHash);

			return SignedInfoHash;
			}


		}
	}
