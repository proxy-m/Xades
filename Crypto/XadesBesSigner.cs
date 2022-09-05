using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using Microsoft.Xades;
using Org.BouncyCastle.X509;
using Crypto.CryptoProviders;

using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace Crypto
	{
	/// <summary>
	/// Вспомогательный класс для представления информации о ключе для Xades
	/// </summary>
	public class XadesInfo
		{
		/// <summary>
		/// Отпечаток пальца сертификата
		/// </summary>
		public string Thumbprint
			{
			get; set;
			}

		/// <summary>
		/// Необработанные данные всего сертификата X.509v3
		/// </summary>
		public string RawPK
			{
			get; set;
			}

		/// <summary>
		/// Время подписи в UTC
		/// </summary>
		public DateTime SigningDateTimeUTC
			{
			get; set;
			}

		/// <summary>
		/// Смещение в минутах локального времени от времени в UTC
		/// </summary>
		public int TimeZoneOffsetMinutes
			{
			get; set;
			}
		}

	/// <summary>
	/// Вспомогательный класс ключ/значение
	/// </summary>
	public class ReplacementPair
		{
		/// <summary>
		/// Ключ
		/// </summary>
		public string Key
			{
			get; set;
			}

		/// <summary>
		/// Значение
		/// </summary>
		public string Value
			{
			get; set;
			}
		}

	/// <summary>
	/// Вспомогательный класс для подписи исходящего сообщения
	/// </summary>
	public static class XadesBesSigner
		{
		/// <summary>
		/// Сохранять пробелы в XML документе
		/// </summary>
		private const bool PRESERVE_WHITESPACE = true;

		/// <summary>
		/// Маркер подписи. Вроде как (не уверен) используется внутри библиотеки Xades
		/// </summary>
		public const string XADES_SIGNED_DATA_CONTAINER = "signed-data-container";

		/// <summary>
		/// Иденфикатор алгоритма
		/// </summary>
		private const string XmlDsigGost3411UrlObsolete = "http://www.w3.org/2001/04/xmldsig-more#gostr3411";

		private const string XmlDsigGost3410UrlObsolete = "http://www.w3.org/2001/04/xmldsig-more#gostr34102001-gostr3411";

		/// <summary>
		/// Функция возвращает запрос подписанный XADES-BES
		/// </summary>
		/// <param name="request">Кусок запроса который нужно подписать</param>
		/// <param name="SigningCertificate">Сертификат которым подписывается запрос</param>
		/// <returns>запрос подписанный XADES-BES</returns>
		public static string GetSignedRequestXades(string request, X509Certificate2 SigningCertificate)
			{
			XmlDocument originalDoc = new XmlDocument();
			originalDoc.PreserveWhitespace = PRESERVE_WHITESPACE;
			originalDoc.LoadXml(request);


			string signatureid = String.Format(CultureInfo.CurrentCulture, "xmldsig-{0}", Guid.NewGuid().ToString().ToLower());
			XadesSignedXml signedXml = GetXadesSignedXml(SigningCertificate, originalDoc, signatureid);


			string Base64EncodedCertificate;
			Base64EncodedCertificate = GetBase64EncodedCertificate(SigningCertificate);
			KeyInfo keyInfo = GetKeyInfo(Base64EncodedCertificate);
			signedXml.KeyInfo = keyInfo;

			XadesInfo xadesInfo = GetXadesInfo(SigningCertificate);

			ICryptoProvider CryptoProvider = GetCryptoProvider();

			XadesObject xadesObject = GetXadesObject(xadesInfo, signatureid, CryptoProvider, SigningCertificate);
			string tmpQualifyingProperties = xadesObject.GetXml().InnerXml; // QualifyingProperties

			signedXml.AddXadesObject(xadesObject);


			// ComputeSignature() -> GetSignedInfoHash -> GetC14NDigest(hash, "ds") -> GetDigestedOutput 
			signedXml.ComputeSignature(CryptoProvider.ComputeHash, CryptoProvider.ComputeSignature, CryptoProvider.ReverseSignature);

			InjectSignatureToOriginalDoc(signedXml, originalDoc);

			return originalDoc.OuterXml;
			}

		/// <summary>
		/// Получить криптопровайдера
		/// </summary>
		/// <returns></returns>
		static ICryptoProvider GetCryptoProvider()
			{
			switch (GisBusted.GisGlobals.ProviderModel)
				{
				case GisBusted.ProviderModel.dotNetFramework:
						{
						ICryptoProvider iCryptoProvider = new NetFrameworkProvider(GisBusted.GisGlobals.SigningCertificate);
						return iCryptoProvider;
						}
				case GisBusted.ProviderModel.CryptoApi:
						{
						CryptoProviderFactory Factory = CryptoProviderFactory.GetInstance();
						ICryptoProvider iCryptoProvider = Factory.GetCryptoProvider(GisBusted.GisGlobals.SigningCertificate);
						return iCryptoProvider;
						}
				default:
						{
						throw new NotImplementedException();
						}
				}
			}

		/// <summary>
		/// Возвращает какую-то информацию
		/// </summary>
		/// <param name="certificate">Сертификат</param>
		/// <returns>Информацию о ключе для Xades</returns>
		public static XadesInfo GetXadesInfo(X509Certificate2 certificate)
			{
			XadesInfo xadesInfo = new XadesInfo();
			string Base64EncodedCertificate;
			Base64EncodedCertificate = GetBase64EncodedCertificate(certificate);

			xadesInfo.RawPK = Base64EncodedCertificate;
			xadesInfo.SigningDateTimeUTC = DateTime.UtcNow;
			TimeSpan delta = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
			xadesInfo.TimeZoneOffsetMinutes = Convert.ToInt32(delta.TotalMinutes);
			return xadesInfo;
			}

		/// <summary>
		/// Получить сертификат закодированный в виде Base64
		/// </summary>
		/// <param name="certificate">Сертификат</param>
		/// <returns>сертификат закодированный в виде Base64</returns>
		private static string GetBase64EncodedCertificate(X509Certificate2 certificate)
			{
			string Base64EncodedCertificate;
			byte[] RawCertData = certificate.GetRawCertData();
			Base64EncodedCertificate = Convert.ToBase64String(RawCertData);

			return Base64EncodedCertificate;
			}

		/// <summary>
		/// Возвращает цифровую подпись XML
		/// </summary>
		/// <param name="Base64EncodedCertificate">Сертификат закодированный в Base64</param>
		/// <returns></returns>
		public static KeyInfo GetKeyInfo(string Base64EncodedCertificate)
			{
			KeyInfo keyInfo = new KeyInfo();

			XmlDocument doc = new XmlDocument();
			XmlElement keyInfoElement = (XmlElement) doc.AppendChild(doc.CreateElement("ds", "KeyInfo", "http://www.w3.org/2000/09/xmldsig#"));
			XmlElement x509DataElement = doc.CreateElement("ds", "X509Data", "http://www.w3.org/2000/09/xmldsig#");
			XmlNode x509DataNode = keyInfoElement.AppendChild(x509DataElement);

			XmlElement x509CertificateElement = doc.CreateElement("ds", "X509Certificate", "http://www.w3.org/2000/09/xmldsig#");
			x509CertificateElement.InnerText = Base64EncodedCertificate;
			x509DataNode.AppendChild(x509CertificateElement);

			KeyInfoNode kid = new KeyInfoNode(x509DataElement);
			keyInfo.AddClause(kid);

			return keyInfo;
			}

		/// <summary>
		/// Вставить подпись до подписываемого элемента?
		/// </summary>
		/// <param name="signedXml"></param>
		/// <param name="originalDoc"></param>
		public static void InjectSignatureToOriginalDoc(XadesSignedXml signedXml, XmlDocument originalDoc)
			{
			XmlElement xmlSig = signedXml.GetXml();
			XmlElement signedDataContainer = signedXml.GetIdElement(originalDoc, XADES_SIGNED_DATA_CONTAINER);
			signedDataContainer.InsertBefore(originalDoc.ImportNode(xmlSig, true), signedDataContainer.FirstChild);
			}

		/// <summary>
		/// Разбор строки по запятым
		/// </summary>
		/// <param name="strToSplit">Строка для разбора</param>
		/// <returns>Набор разобранный по запятым</returns>
		public static List<string> SplitString(string strToSplit)
			{
			List<string> Parts = new List<string>();

			int StartIndex = 0;
			for (;;)
				{
				int index1 = strToSplit.IndexOf(',', StartIndex);

				if ((index1 == -1)) // запятых больше нет
					{
					if (strToSplit.Length > 0)
						{
						Parts.Add(strToSplit);
						}
					break;
					}

				int index2 = strToSplit.IndexOf("\\,", StartIndex, StringComparison.Ordinal);

				if ((index2 == -1) || (index1 < index2))
					{
					string sub = strToSplit.Substring(0, index1);
					strToSplit = strToSplit.Substring(index1 + 1, strToSplit.Length - index1 - 1);
					Parts.Add(sub);
					StartIndex = 0;
					continue;
					}

				if ((index2 > 0) && (index2 < index1))
					{
					StartIndex = index1 + 1;
					continue;
					}
				}

			return Parts;
			}

		/// <summary>
		/// Исправить строку X509IssuerName для рукожопых писателей из Ланита
		/// </summary>
		/// <param name="X509IssuerName">Исходная строка из сертификата</param>
		/// <returns>Исправленная строка, чтобы ее понимал сервер ГИС ЖКХ</returns>
		public static string IssuerNamePatcher(string X509IssuerName)
			{
			// return "CN=Ланит - отличная компания";

			// string[] Parts = X509IssuerName.Split(','); // не всегда работает как хотелось бы...
			List<string> Parts = SplitString(X509IssuerName);

			List<ReplacementPair> Pairs = new List<ReplacementPair>();

			foreach (string Part in Parts)
				{
				string[] LRParts = Part.Split('=');
				ReplacementPair rp = new ReplacementPair();
				rp.Key = LRParts[0];
				rp.Value = LRParts[1];
				Pairs.Add(rp);
				}

			// замена
			foreach (ReplacementPair Pair in Pairs)
				{
				// Title
				if (Pair.Key.Equals("T", StringComparison.OrdinalIgnoreCase) || Pair.Key.Equals("Title", StringComparison.OrdinalIgnoreCase))
					{
					Pair.Key = "2.5.4.12";
					continue;
					}

				// GivenName
				if (Pair.Key.Equals("G", StringComparison.OrdinalIgnoreCase) || Pair.Key.Equals("GivenName", StringComparison.OrdinalIgnoreCase))
					{
					Pair.Key = "2.5.4.42";
					continue;
					}

				// SurName
				if (Pair.Key.Equals("SN", StringComparison.OrdinalIgnoreCase) || Pair.Key.Equals("SurName", StringComparison.OrdinalIgnoreCase))
					{
					Pair.Key = "2.5.4.4";
					continue;
					}

				// OrgUnit
				if (Pair.Key.Equals("OU", StringComparison.OrdinalIgnoreCase) || Pair.Key.Equals("OrgUnit", StringComparison.OrdinalIgnoreCase))
					{
					Pair.Key = "2.5.4.11";
					continue;
					}

				// Unstructured-Name
				if (Pair.Key.Equals("Unstructured-Name", StringComparison.OrdinalIgnoreCase) || Pair.Key.Equals("UnstructuredName", StringComparison.OrdinalIgnoreCase))
					{
					Pair.Key = "1.2.840.113549.1.9.2";
					continue;
					}

				// http://www.alvestrand.no/objectid/1.2.840.113549.1.9.1.html Email Address attribute
				// for use in signatures - 1.2.840.113549.1.9.1
				if (Pair.Key.Equals("E", StringComparison.OrdinalIgnoreCase) || Pair.Key.Equals("UnstructuredName", StringComparison.OrdinalIgnoreCase))
					{
					Pair.Key = "1.2.840.113549.1.9.1";
					continue;
					}

				} // end foreach

			// Собираем обратно в строку
			StringBuilder sb = new StringBuilder();
			int nCount = Pairs.Count;

			for (int i = 0; i < nCount; i++)
				{
				sb.Append(Pairs[i].Key);
				sb.Append("=");
				sb.Append(Pairs[i].Value);
				if (i != (nCount - 1))
					{
					sb.Append(", ");
					}
				}

			string Result = sb.ToString();
			return Result;
			}


		/// <summary>
		/// Создать XadesObject
		/// </summary>
		/// <param name="xadesInfo"></param>
		/// <param name="signatureid"></param>
		/// <returns></returns>
		internal static XadesObject GetXadesObject(XadesInfo xadesInfo, string signatureid, ICryptoProvider CryptoProvider, X509Certificate2 SigningCertificate)
			{
			XadesObject xadesObject = new XadesObject();
			xadesObject.QualifyingProperties.Target = String.Format(CultureInfo.InvariantCulture, "#{0}", signatureid);
			xadesObject.QualifyingProperties.SignedProperties.Id = String.Format("{0}-signedprops", signatureid);
			SignedSignatureProperties signedSignatureProperties = xadesObject.QualifyingProperties.SignedProperties.SignedSignatureProperties;

			X509CertificateParser x509CertificateParser = new Org.BouncyCastle.X509.X509CertificateParser();
			X509Certificate bouncyCert = x509CertificateParser.ReadCertificate(Convert.FromBase64String(xadesInfo.RawPK));

			// string sss = bouncyCert.IssuerDN.ToString(false, Org.BouncyCastle.Asn1.X509.X509Name.RFC1779Symbols);

			string X509IssuerDN; // = GetOidRepresentation(bouncyCert.IssuerDN.ToString());


			X509IssuerDN = bouncyCert.IssuerDN.ToString(false, Org.BouncyCastle.Asn1.X509.X509Name.RFC1779Symbols);

			// Исправляем криворукость Ланита
			X509IssuerDN = IssuerNamePatcher(X509IssuerDN);

			Cert cert = new Cert
				{
				IssuerSerial =
				{
					X509IssuerName = X509IssuerDN,
					X509SerialNumber = bouncyCert.SerialNumber.ToString()
				}
				};

			cert.CertDigest.DigestMethod.Algorithm = XmlDsigGost3411UrlObsolete;

			byte[] rawCertData = SigningCertificate.RawData; // Convert.FromBase64String(xadesInfo.RawPK);

			// оригинальный вариант
			// HashAlgorithm.Create("GOST3411"); byte[] hashValue = pkHash.ComputeHash(rawCertData);
			// cert.CertDigest.DigestValue = hashValue;

			// Расчет с помощью BouncyCastle
			// byte[] Hash3411 = Gost3411Hash(rawCertData);
			// cert.CertDigest.DigestValue = Hash3411;

			// Расчет с помощью криптопровайдера
			byte[] Hash3411 = CryptoProvider.ComputeHash(rawCertData);
			cert.CertDigest.DigestValue = Hash3411;

			signedSignatureProperties.SigningCertificate.CertCollection.Add(cert);

			signedSignatureProperties.SigningTime = xadesInfo.SigningDateTimeUTC.AddMinutes(xadesInfo.TimeZoneOffsetMinutes);
			return xadesObject;
			}

		/// <summary>
		/// Расчитать хэш по алгоритму ГОСТ 3411
		/// </summary>
		/// <param name="MessageBytes">входные байты</param>
		/// <returns>хэш по алгоритму ГОСТ 3411</returns>
		//static public byte[] Gost3411Hash(byte[] MessageBytes)
		//	{
		//	Org.BouncyCastle.Crypto.Digests.Gost3411Digest digEng = new Org.BouncyCastle.Crypto.Digests.Gost3411Digest();

		//	digEng.BlockUpdate(MessageBytes, 0, MessageBytes.Length);

		//	byte[] digest = new byte[digEng.GetDigestSize()];
		//	digEng.DoFinal(digest, 0);
		//	return digest;
		//	}

		/// <summary>
		/// Получить подписанный XML?
		/// </summary>
		/// <param name="certificate">Сертификат</param>
		/// <param name="originalDoc">Исходный документ</param>
		/// <param name="signatureid">Идентификатор сигнатуры</param>
		/// <returns></returns>
		public static XadesSignedXml GetXadesSignedXml(X509Certificate2 certificate, XmlDocument originalDoc, string signatureid)
			{
			// вариант 2
			XadesSignedXml signedXml = new XadesSignedXml(originalDoc);
			// signedXml.SigningKey = certificate.PrivateKey;  
			signedXml.Signature.Id = signatureid;
			signedXml.SignatureValueId = String.Format(CultureInfo.InvariantCulture, "{0}-sigvalue", signatureid);

			Reference reference = new Reference
				{
				Uri = "#" + XADES_SIGNED_DATA_CONTAINER,
				DigestMethod = XmlDsigGost3411UrlObsolete,
				Id = String.Format(CultureInfo.CurrentCulture, "{0}-ref0", signatureid)
				};

			reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
			reference.AddTransform(new XmlDsigExcC14NTransform());
			signedXml.AddReference(reference);

			signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigCanonicalizationUrl;
			signedXml.SignedInfo.SignatureMethod = XmlDsigGost3410UrlObsolete;

			return signedXml;
			}



		}
	}
