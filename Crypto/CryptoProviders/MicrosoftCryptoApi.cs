// Copyright (c) Microsoft Corporation. All rights reserved. CryptoApi.cs

using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace Crypto.CryptoProviders.MicrosoftCryptoApi
	{
	using _FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

	public static class CAPI
		{
		// PInvoke dll's.

		public const String ADVAPI32 = "advapi32.dll";
		public const String CRYPT32 = "crypt32.dll";
		public const String CRYPTUI = "cryptui.dll";
		public const String KERNEL32 = "kernel32.dll";

		// Constants

		public const uint LMEM_FIXED = 0x0000;
		public const uint LMEM_ZEROINIT = 0x0040;
		public const uint LPTR = (LMEM_FIXED | LMEM_ZEROINIT);

		public const int S_OK = 0;
		public const int S_FALSE = 1;

		public const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
		public const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;

		public const uint VER_PLATFORM_WIN32s = 0;
		public const uint VER_PLATFORM_WIN32_WINDOWS = 1;
		public const uint VER_PLATFORM_WIN32_NT = 2;
		public const uint VER_PLATFORM_WINCE = 3;

		// ASN.
		public const uint ASN_TAG_NULL = 0x05;

		public const uint ASN_TAG_OBJID = 0x06;

		// cert query object types.
		public const uint CERT_QUERY_OBJECT_FILE = 1;

		public const uint CERT_QUERY_OBJECT_BLOB = 2;

		// cert query content types.
		public const uint CERT_QUERY_CONTENT_CERT = 1;

		public const uint CERT_QUERY_CONTENT_CTL = 2;
		public const uint CERT_QUERY_CONTENT_CRL = 3;
		public const uint CERT_QUERY_CONTENT_SERIALIZED_STORE = 4;
		public const uint CERT_QUERY_CONTENT_SERIALIZED_CERT = 5;
		public const uint CERT_QUERY_CONTENT_SERIALIZED_CTL = 6;
		public const uint CERT_QUERY_CONTENT_SERIALIZED_CRL = 7;
		public const uint CERT_QUERY_CONTENT_PKCS7_SIGNED = 8;
		public const uint CERT_QUERY_CONTENT_PKCS7_UNSIGNED = 9;
		public const uint CERT_QUERY_CONTENT_PKCS7_SIGNED_EMBED = 10;
		public const uint CERT_QUERY_CONTENT_PKCS10 = 11;
		public const uint CERT_QUERY_CONTENT_PFX = 12;
		public const uint CERT_QUERY_CONTENT_CERT_PAIR = 13;

		// cert query content flags.
		public const uint CERT_QUERY_CONTENT_FLAG_CERT = (1 << (int) CERT_QUERY_CONTENT_CERT);

		public const uint CERT_QUERY_CONTENT_FLAG_CTL = (1 << (int) CERT_QUERY_CONTENT_CTL);
		public const uint CERT_QUERY_CONTENT_FLAG_CRL = (1 << (int) CERT_QUERY_CONTENT_CRL);
		public const uint CERT_QUERY_CONTENT_FLAG_SERIALIZED_STORE = (1 << (int) CERT_QUERY_CONTENT_SERIALIZED_STORE);
		public const uint CERT_QUERY_CONTENT_FLAG_SERIALIZED_CERT = (1 << (int) CERT_QUERY_CONTENT_SERIALIZED_CERT);
		public const uint CERT_QUERY_CONTENT_FLAG_SERIALIZED_CTL = (1 << (int) CERT_QUERY_CONTENT_SERIALIZED_CTL);
		public const uint CERT_QUERY_CONTENT_FLAG_SERIALIZED_CRL = (1 << (int) CERT_QUERY_CONTENT_SERIALIZED_CRL);
		public const uint CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED = (1 << (int) CERT_QUERY_CONTENT_PKCS7_SIGNED);
		public const uint CERT_QUERY_CONTENT_FLAG_PKCS7_UNSIGNED = (1 << (int) CERT_QUERY_CONTENT_PKCS7_UNSIGNED);
		public const uint CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED_EMBED = (1 << (int) CERT_QUERY_CONTENT_PKCS7_SIGNED_EMBED);
		public const uint CERT_QUERY_CONTENT_FLAG_PKCS10 = (1 << (int) CERT_QUERY_CONTENT_PKCS10);
		public const uint CERT_QUERY_CONTENT_FLAG_PFX = (1 << (int) CERT_QUERY_CONTENT_PFX);
		public const uint CERT_QUERY_CONTENT_FLAG_CERT_PAIR = (1 << (int) CERT_QUERY_CONTENT_CERT_PAIR);

		public const uint CERT_QUERY_CONTENT_FLAG_ALL =
									   (CERT_QUERY_CONTENT_FLAG_CERT |
										CERT_QUERY_CONTENT_FLAG_CTL |
										CERT_QUERY_CONTENT_FLAG_CRL |
										CERT_QUERY_CONTENT_FLAG_SERIALIZED_STORE |
										CERT_QUERY_CONTENT_FLAG_SERIALIZED_CERT |
										CERT_QUERY_CONTENT_FLAG_SERIALIZED_CTL |
										CERT_QUERY_CONTENT_FLAG_SERIALIZED_CRL |
										CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED |
										CERT_QUERY_CONTENT_FLAG_PKCS7_UNSIGNED |
										CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED_EMBED |
										CERT_QUERY_CONTENT_FLAG_PKCS10 |
										CERT_QUERY_CONTENT_FLAG_PFX |
										CERT_QUERY_CONTENT_FLAG_CERT_PAIR);

		public const uint CERT_QUERY_FORMAT_BINARY = 1;
		public const uint CERT_QUERY_FORMAT_BASE64_ENCODED = 2;
		public const uint CERT_QUERY_FORMAT_ASN_ASCII_HEX_ENCODED = 3;

		public const uint CERT_QUERY_FORMAT_FLAG_BINARY = (1 << (int) CERT_QUERY_FORMAT_BINARY);
		public const uint CERT_QUERY_FORMAT_FLAG_BASE64_ENCODED = (1 << (int) CERT_QUERY_FORMAT_BASE64_ENCODED);
		public const uint CERT_QUERY_FORMAT_FLAG_ASN_ASCII_HEX_ENCODED = (1 << (int) CERT_QUERY_FORMAT_ASN_ASCII_HEX_ENCODED);

		public const uint CERT_QUERY_FORMAT_FLAG_ALL =
									   (CERT_QUERY_FORMAT_FLAG_BINARY |
										CERT_QUERY_FORMAT_FLAG_BASE64_ENCODED |
										CERT_QUERY_FORMAT_FLAG_ASN_ASCII_HEX_ENCODED);

		// CryptProtectData and CryptUnprotectData.
		public const uint CRYPTPROTECT_UI_FORBIDDEN = 0x1;

		public const uint CRYPTPROTECT_LOCAL_MACHINE = 0x4;

		//public const uint CRYPTPROTECT_CRED_[....] = 0x8;
		public const uint CRYPTPROTECT_AUDIT = 0x10;

		public const uint CRYPTPROTECT_NO_RECOVERY = 0x20;
		public const uint CRYPTPROTECT_VERIFY_PROTECTION = 0x40;

		// CryptProtectMemory and CryptUnprotectMemory.
		public const uint CRYPTPROTECTMEMORY_BLOCK_SIZE = 16;

		public const uint CRYPTPROTECTMEMORY_SAME_PROCESS = 0x00;
		public const uint CRYPTPROTECTMEMORY_CROSS_PROCESS = 0x01;
		public const uint CRYPTPROTECTMEMORY_SAME_LOGON = 0x02;

		// OID key type.
		public const uint CRYPT_OID_INFO_OID_KEY = 1;

		public const uint CRYPT_OID_INFO_NAME_KEY = 2;
		public const uint CRYPT_OID_INFO_ALGID_KEY = 3;
		public const uint CRYPT_OID_INFO_SIGN_KEY = 4;

		// OID group Id's.
		public const uint CRYPT_HASH_ALG_OID_GROUP_ID = 1;

		public const uint CRYPT_ENCRYPT_ALG_OID_GROUP_ID = 2;
		public const uint CRYPT_PUBKEY_ALG_OID_GROUP_ID = 3;
		public const uint CRYPT_SIGN_ALG_OID_GROUP_ID = 4;
		public const uint CRYPT_RDN_ATTR_OID_GROUP_ID = 5;
		public const uint CRYPT_EXT_OR_ATTR_OID_GROUP_ID = 6;
		public const uint CRYPT_ENHKEY_USAGE_OID_GROUP_ID = 7;
		public const uint CRYPT_POLICY_OID_GROUP_ID = 8;
		public const uint CRYPT_TEMPLATE_OID_GROUP_ID = 9;
		public const uint CRYPT_LAST_OID_GROUP_ID = 9;

		public const uint CRYPT_FIRST_ALG_OID_GROUP_ID = CRYPT_HASH_ALG_OID_GROUP_ID;
		public const uint CRYPT_LAST_ALG_OID_GROUP_ID = CRYPT_SIGN_ALG_OID_GROUP_ID;

		// cert encoding flags.
		public const uint CRYPT_ASN_ENCODING = 0x00000001;

		public const uint CRYPT_NDR_ENCODING = 0x00000002;
		public const uint X509_ASN_ENCODING = 0x00000001;
		public const uint X509_NDR_ENCODING = 0x00000002;
		public const uint PKCS_7_ASN_ENCODING = 0x00010000;
		public const uint PKCS_7_NDR_ENCODING = 0x00020000;
		public const uint PKCS_7_OR_X509_ASN_ENCODING = (PKCS_7_ASN_ENCODING | X509_ASN_ENCODING);

		// cert store provider
		public const uint CERT_STORE_PROV_MSG = 1;

		public const uint CERT_STORE_PROV_MEMORY = 2;
		public const uint CERT_STORE_PROV_FILE = 3;
		public const uint CERT_STORE_PROV_REG = 4;
		public const uint CERT_STORE_PROV_PKCS7 = 5;
		public const uint CERT_STORE_PROV_SERIALIZED = 6;
		public const uint CERT_STORE_PROV_FILENAME_A = 7;
		public const uint CERT_STORE_PROV_FILENAME_W = 8;
		public const uint CERT_STORE_PROV_FILENAME = CERT_STORE_PROV_FILENAME_W;
		public const uint CERT_STORE_PROV_SYSTEM_A = 9;
		public const uint CERT_STORE_PROV_SYSTEM_W = 10;
		public const uint CERT_STORE_PROV_SYSTEM = CERT_STORE_PROV_SYSTEM_W;
		public const uint CERT_STORE_PROV_COLLECTION = 11;
		public const uint CERT_STORE_PROV_SYSTEM_REGISTRY_A = 12;
		public const uint CERT_STORE_PROV_SYSTEM_REGISTRY_W = 13;
		public const uint CERT_STORE_PROV_SYSTEM_REGISTRY = CERT_STORE_PROV_SYSTEM_REGISTRY_W;
		public const uint CERT_STORE_PROV_PHYSICAL_W = 14;
		public const uint CERT_STORE_PROV_PHYSICAL = CERT_STORE_PROV_PHYSICAL_W;
		public const uint CERT_STORE_PROV_SMART_CARD_W = 15;
		public const uint CERT_STORE_PROV_SMART_CARD = CERT_STORE_PROV_SMART_CARD_W;
		public const uint CERT_STORE_PROV_LDAP_W = 16;
		public const uint CERT_STORE_PROV_LDAP = CERT_STORE_PROV_LDAP_W;

		// cert store flags
		public const uint CERT_STORE_NO_CRYPT_RELEASE_FLAG = 0x00000001;

		public const uint CERT_STORE_SET_LOCALIZED_NAME_FLAG = 0x00000002;
		public const uint CERT_STORE_DEFER_CLOSE_UNTIL_LAST_FREE_FLAG = 0x00000004;
		public const uint CERT_STORE_DELETE_FLAG = 0x00000010;
		public const uint CERT_STORE_SHARE_STORE_FLAG = 0x00000040;
		public const uint CERT_STORE_SHARE_CONTEXT_FLAG = 0x00000080;
		public const uint CERT_STORE_MANIFOLD_FLAG = 0x00000100;
		public const uint CERT_STORE_ENUM_ARCHIVED_FLAG = 0x00000200;
		public const uint CERT_STORE_UPDATE_KEYID_FLAG = 0x00000400;
		public const uint CERT_STORE_BACKUP_RESTORE_FLAG = 0x00000800;
		public const uint CERT_STORE_READONLY_FLAG = 0x00008000;
		public const uint CERT_STORE_OPEN_EXISTING_FLAG = 0x00004000;
		public const uint CERT_STORE_CREATE_NEW_FLAG = 0x00002000;
		public const uint CERT_STORE_MAXIMUM_ALLOWED_FLAG = 0x00001000;

		// cert store location
		public const uint CERT_SYSTEM_STORE_UNPROTECTED_FLAG = 0x40000000;

		public const uint CERT_SYSTEM_STORE_LOCATION_MASK = 0x00FF0000;
		public const uint CERT_SYSTEM_STORE_LOCATION_SHIFT = 16;

		public const uint CERT_SYSTEM_STORE_CURRENT_USER_ID = 1;
		public const uint CERT_SYSTEM_STORE_LOCAL_MACHINE_ID = 2;
		public const uint CERT_SYSTEM_STORE_CURRENT_SERVICE_ID = 4;
		public const uint CERT_SYSTEM_STORE_SERVICES_ID = 5;
		public const uint CERT_SYSTEM_STORE_USERS_ID = 6;
		public const uint CERT_SYSTEM_STORE_CURRENT_USER_GROUP_POLICY_ID = 7;
		public const uint CERT_SYSTEM_STORE_LOCAL_MACHINE_GROUP_POLICY_ID = 8;
		public const uint CERT_SYSTEM_STORE_LOCAL_MACHINE_ENTERPRISE_ID = 9;

		public const uint CERT_SYSTEM_STORE_CURRENT_USER = ((int) CERT_SYSTEM_STORE_CURRENT_USER_ID << (int) CERT_SYSTEM_STORE_LOCATION_SHIFT);
		public const uint CERT_SYSTEM_STORE_LOCAL_MACHINE = ((int) CERT_SYSTEM_STORE_LOCAL_MACHINE_ID << (int) CERT_SYSTEM_STORE_LOCATION_SHIFT);
		public const uint CERT_SYSTEM_STORE_CURRENT_SERVICE = ((int) CERT_SYSTEM_STORE_CURRENT_SERVICE_ID << (int) CERT_SYSTEM_STORE_LOCATION_SHIFT);
		public const uint CERT_SYSTEM_STORE_SERVICES = ((int) CERT_SYSTEM_STORE_SERVICES_ID << (int) CERT_SYSTEM_STORE_LOCATION_SHIFT);
		public const uint CERT_SYSTEM_STORE_USERS = ((int) CERT_SYSTEM_STORE_USERS_ID << (int) CERT_SYSTEM_STORE_LOCATION_SHIFT);
		public const uint CERT_SYSTEM_STORE_CURRENT_USER_GROUP_POLICY = ((int) CERT_SYSTEM_STORE_CURRENT_USER_GROUP_POLICY_ID << (int) CERT_SYSTEM_STORE_LOCATION_SHIFT);
		public const uint CERT_SYSTEM_STORE_LOCAL_MACHINE_GROUP_POLICY = ((int) CERT_SYSTEM_STORE_LOCAL_MACHINE_GROUP_POLICY_ID << (int) CERT_SYSTEM_STORE_LOCATION_SHIFT);
		public const uint CERT_SYSTEM_STORE_LOCAL_MACHINE_ENTERPRISE = ((int) CERT_SYSTEM_STORE_LOCAL_MACHINE_ENTERPRISE_ID << (int) CERT_SYSTEM_STORE_LOCATION_SHIFT);

		// cert name types.
		public const uint CERT_NAME_EMAIL_TYPE = 1;

		public const uint CERT_NAME_RDN_TYPE = 2;
		public const uint CERT_NAME_ATTR_TYPE = 3;
		public const uint CERT_NAME_SIMPLE_DISPLAY_TYPE = 4;
		public const uint CERT_NAME_FRIENDLY_DISPLAY_TYPE = 5;
		public const uint CERT_NAME_DNS_TYPE = 6;
		public const uint CERT_NAME_URL_TYPE = 7;
		public const uint CERT_NAME_UPN_TYPE = 8;

		// cert name flags.
		public const uint CERT_SIMPLE_NAME_STR = 1;

		public const uint CERT_OID_NAME_STR = 2;
		public const uint CERT_X500_NAME_STR = 3;

		public const uint CERT_NAME_STR_SEMICOLON_FLAG = 0x40000000;
		public const uint CERT_NAME_STR_NO_PLUS_FLAG = 0x20000000;
		public const uint CERT_NAME_STR_NO_QUOTING_FLAG = 0x10000000;
		public const uint CERT_NAME_STR_CRLF_FLAG = 0x08000000;
		public const uint CERT_NAME_STR_COMMA_FLAG = 0x04000000;
		public const uint CERT_NAME_STR_REVERSE_FLAG = 0x02000000;

		public const uint CERT_NAME_ISSUER_FLAG = 0x1;
		public const uint CERT_NAME_STR_DISABLE_IE4_UTF8_FLAG = 0x00010000;
		public const uint CERT_NAME_STR_ENABLE_T61_UNICODE_FLAG = 0x00020000;
		public const uint CERT_NAME_STR_ENABLE_UTF8_UNICODE_FLAG = 0x00040000;
		public const uint CERT_NAME_STR_FORCE_UTF8_DIR_STR_FLAG = 0x00080000;

		// cert context property Id's.
		public const uint CERT_KEY_PROV_HANDLE_PROP_ID = 1;

		public const uint CERT_KEY_PROV_INFO_PROP_ID = 2;
		public const uint CERT_SHA1_HASH_PROP_ID = 3;
		public const uint CERT_MD5_HASH_PROP_ID = 4;
		public const uint CERT_HASH_PROP_ID = CERT_SHA1_HASH_PROP_ID;
		public const uint CERT_KEY_CONTEXT_PROP_ID = 5;
		public const uint CERT_KEY_SPEC_PROP_ID = 6;
		public const uint CERT_IE30_RESERVED_PROP_ID = 7;
		public const uint CERT_PUBKEY_HASH_RESERVED_PROP_ID = 8;
		public const uint CERT_ENHKEY_USAGE_PROP_ID = 9;
		public const uint CERT_CTL_USAGE_PROP_ID = CERT_ENHKEY_USAGE_PROP_ID;
		public const uint CERT_NEXT_UPDATE_LOCATION_PROP_ID = 10;
		public const uint CERT_FRIENDLY_NAME_PROP_ID = 11;
		public const uint CERT_PVK_FILE_PROP_ID = 12;
		public const uint CERT_DESCRIPTION_PROP_ID = 13;
		public const uint CERT_ACCESS_STATE_PROP_ID = 14;
		public const uint CERT_SIGNATURE_HASH_PROP_ID = 15;
		public const uint CERT_SMART_CARD_DATA_PROP_ID = 16;
		public const uint CERT_EFS_PROP_ID = 17;
		public const uint CERT_FORTEZZA_DATA_PROP_ID = 18;
		public const uint CERT_ARCHIVED_PROP_ID = 19;
		public const uint CERT_KEY_IDENTIFIER_PROP_ID = 20;
		public const uint CERT_AUTO_ENROLL_PROP_ID = 21;
		public const uint CERT_PUBKEY_ALG_PARA_PROP_ID = 22;
		public const uint CERT_CROSS_CERT_DIST_POINTS_PROP_ID = 23;
		public const uint CERT_ISSUER_PUBLIC_KEY_MD5_HASH_PROP_ID = 24;
		public const uint CERT_SUBJECT_PUBLIC_KEY_MD5_HASH_PROP_ID = 25;
		public const uint CERT_ENROLLMENT_PROP_ID = 26;
		public const uint CERT_DATE_STAMP_PROP_ID = 27;
		public const uint CERT_ISSUER_SERIAL_NUMBER_MD5_HASH_PROP_ID = 28;
		public const uint CERT_SUBJECT_NAME_MD5_HASH_PROP_ID = 29;
		public const uint CERT_EXTENDED_ERROR_INFO_PROP_ID = 30;
		public const uint CERT_RENEWAL_PROP_ID = 64;
		public const uint CERT_ARCHIVED_KEY_HASH_PROP_ID = 65;
		public const uint CERT_FIRST_RESERVED_PROP_ID = 66;

		// This value shall be defined in wincrypt.h so we avoid conflicts
		public const uint CERT_DELETE_KEYSET_PROP_ID = 101;

		// cert info flags.
		public const uint CERT_INFO_VERSION_FLAG = 1;

		public const uint CERT_INFO_SERIAL_NUMBER_FLAG = 2;
		public const uint CERT_INFO_SIGNATURE_ALGORITHM_FLAG = 3;
		public const uint CERT_INFO_ISSUER_FLAG = 4;
		public const uint CERT_INFO_NOT_BEFORE_FLAG = 5;
		public const uint CERT_INFO_NOT_AFTER_FLAG = 6;
		public const uint CERT_INFO_SUBJECT_FLAG = 7;
		public const uint CERT_INFO_SUBJECT_PUBLIC_KEY_INFO_FLAG = 8;
		public const uint CERT_INFO_ISSUER_UNIQUE_ID_FLAG = 9;
		public const uint CERT_INFO_SUBJECT_UNIQUE_ID_FLAG = 10;
		public const uint CERT_INFO_EXTENSION_FLAG = 11;

		// cert compare flags.
		public const uint CERT_COMPARE_MASK = 0xFFFF;

		public const uint CERT_COMPARE_SHIFT = 16;
		public const uint CERT_COMPARE_ANY = 0;
		public const uint CERT_COMPARE_SHA1_HASH = 1;
		public const uint CERT_COMPARE_NAME = 2;
		public const uint CERT_COMPARE_ATTR = 3;
		public const uint CERT_COMPARE_MD5_HASH = 4;
		public const uint CERT_COMPARE_PROPERTY = 5;
		public const uint CERT_COMPARE_PUBLIC_KEY = 6;
		public const uint CERT_COMPARE_HASH = CERT_COMPARE_SHA1_HASH;
		public const uint CERT_COMPARE_NAME_STR_A = 7;
		public const uint CERT_COMPARE_NAME_STR_W = 8;
		public const uint CERT_COMPARE_KEY_SPEC = 9;
		public const uint CERT_COMPARE_ENHKEY_USAGE = 10;
		public const uint CERT_COMPARE_CTL_USAGE = CERT_COMPARE_ENHKEY_USAGE;
		public const uint CERT_COMPARE_SUBJECT_CERT = 11;
		public const uint CERT_COMPARE_ISSUER_OF = 12;
		public const uint CERT_COMPARE_EXISTING = 13;
		public const uint CERT_COMPARE_SIGNATURE_HASH = 14;
		public const uint CERT_COMPARE_KEY_IDENTIFIER = 15;
		public const uint CERT_COMPARE_CERT_ID = 16;
		public const uint CERT_COMPARE_CROSS_CERT_DIST_POINTS = 17;
		public const uint CERT_COMPARE_PUBKEY_MD5_HASH = 18;

		// cert find flags.
		public const uint CERT_FIND_ANY = ((int) CERT_COMPARE_ANY << (int) CERT_COMPARE_SHIFT);

		public const uint CERT_FIND_SHA1_HASH = ((int) CERT_COMPARE_SHA1_HASH << (int) CERT_COMPARE_SHIFT);
		public const uint CERT_FIND_MD5_HASH = ((int) CERT_COMPARE_MD5_HASH << (int) CERT_COMPARE_SHIFT);
		public const uint CERT_FIND_SIGNATURE_HASH = ((int) CERT_COMPARE_SIGNATURE_HASH << (int) CERT_COMPARE_SHIFT);
		public const uint CERT_FIND_KEY_IDENTIFIER = ((int) CERT_COMPARE_KEY_IDENTIFIER << (int) CERT_COMPARE_SHIFT);
		public const uint CERT_FIND_HASH = CERT_FIND_SHA1_HASH;
		public const uint CERT_FIND_PROPERTY = ((int) CERT_COMPARE_PROPERTY << (int) CERT_COMPARE_SHIFT);
		public const uint CERT_FIND_PUBLIC_KEY = ((int) CERT_COMPARE_PUBLIC_KEY << (int) CERT_COMPARE_SHIFT);
		public const uint CERT_FIND_SUBJECT_NAME = ((int) CERT_COMPARE_NAME << (int) CERT_COMPARE_SHIFT | (int) CERT_INFO_SUBJECT_FLAG);
		public const uint CERT_FIND_SUBJECT_ATTR = ((int) CERT_COMPARE_ATTR << (int) CERT_COMPARE_SHIFT | (int) CERT_INFO_SUBJECT_FLAG);
		public const uint CERT_FIND_ISSUER_NAME = ((int) CERT_COMPARE_NAME << (int) CERT_COMPARE_SHIFT | (int) CERT_INFO_ISSUER_FLAG);
		public const uint CERT_FIND_ISSUER_ATTR = ((int) CERT_COMPARE_ATTR << (int) CERT_COMPARE_SHIFT | (int) CERT_INFO_ISSUER_FLAG);
		public const uint CERT_FIND_SUBJECT_STR_A = ((int) CERT_COMPARE_NAME_STR_A << (int) CERT_COMPARE_SHIFT | (int) CERT_INFO_SUBJECT_FLAG);
		public const uint CERT_FIND_SUBJECT_STR_W = ((int) CERT_COMPARE_NAME_STR_W << (int) CERT_COMPARE_SHIFT | (int) CERT_INFO_SUBJECT_FLAG);
		public const uint CERT_FIND_SUBJECT_STR = CERT_FIND_SUBJECT_STR_W;
		public const uint CERT_FIND_ISSUER_STR_A = ((int) CERT_COMPARE_NAME_STR_A << (int) CERT_COMPARE_SHIFT | (int) CERT_INFO_ISSUER_FLAG);
		public const uint CERT_FIND_ISSUER_STR_W = ((int) CERT_COMPARE_NAME_STR_W << (int) CERT_COMPARE_SHIFT | (int) CERT_INFO_ISSUER_FLAG);
		public const uint CERT_FIND_ISSUER_STR = CERT_FIND_ISSUER_STR_W;
		public const uint CERT_FIND_KEY_SPEC = ((int) CERT_COMPARE_KEY_SPEC << (int) CERT_COMPARE_SHIFT);
		public const uint CERT_FIND_ENHKEY_USAGE = ((int) CERT_COMPARE_ENHKEY_USAGE << (int) CERT_COMPARE_SHIFT);
		public const uint CERT_FIND_CTL_USAGE = CERT_FIND_ENHKEY_USAGE;
		public const uint CERT_FIND_SUBJECT_CERT = ((int) CERT_COMPARE_SUBJECT_CERT << (int) CERT_COMPARE_SHIFT);
		public const uint CERT_FIND_ISSUER_OF = ((int) CERT_COMPARE_ISSUER_OF << (int) CERT_COMPARE_SHIFT);
		public const uint CERT_FIND_EXISTING = ((int) CERT_COMPARE_EXISTING << (int) CERT_COMPARE_SHIFT);
		public const uint CERT_FIND_CERT_ID = ((int) CERT_COMPARE_CERT_ID << (int) CERT_COMPARE_SHIFT);
		public const uint CERT_FIND_CROSS_CERT_DIST_POINTS = ((int) CERT_COMPARE_CROSS_CERT_DIST_POINTS << (int) CERT_COMPARE_SHIFT);
		public const uint CERT_FIND_PUBKEY_MD5_HASH = ((int) CERT_COMPARE_PUBKEY_MD5_HASH << (int) CERT_COMPARE_SHIFT);

		// cert key usage flags.
		public const uint CERT_ENCIPHER_ONLY_KEY_USAGE = 0x0001;

		public const uint CERT_CRL_SIGN_KEY_USAGE = 0x0002;
		public const uint CERT_KEY_CERT_SIGN_KEY_USAGE = 0x0004;
		public const uint CERT_KEY_AGREEMENT_KEY_USAGE = 0x0008;
		public const uint CERT_DATA_ENCIPHERMENT_KEY_USAGE = 0x0010;
		public const uint CERT_KEY_ENCIPHERMENT_KEY_USAGE = 0x0020;
		public const uint CERT_NON_REPUDIATION_KEY_USAGE = 0x0040;
		public const uint CERT_DIGITAL_SIGNATURE_KEY_USAGE = 0x0080;
		public const uint CERT_DECIPHER_ONLY_KEY_USAGE = 0x8000;

		// Add certificate/CRL, encoded, context or element disposition values.
		public const uint CERT_STORE_ADD_NEW = 1;

		public const uint CERT_STORE_ADD_USE_EXISTING = 2;
		public const uint CERT_STORE_ADD_REPLACE_EXISTING = 3;
		public const uint CERT_STORE_ADD_ALWAYS = 4;
		public const uint CERT_STORE_ADD_REPLACE_EXISTING_INHERIT_PROPERTIES = 5;
		public const uint CERT_STORE_ADD_NEWER = 6;
		public const uint CERT_STORE_ADD_NEWER_INHERIT_PROPERTIES = 7;

		// store save as type.
		public const uint CERT_STORE_SAVE_AS_STORE = 1;

		public const uint CERT_STORE_SAVE_AS_PKCS7 = 2;

		// store save to type.
		public const uint CERT_STORE_SAVE_TO_FILE = 1;

		public const uint CERT_STORE_SAVE_TO_MEMORY = 2;
		public const uint CERT_STORE_SAVE_TO_FILENAME_A = 3;
		public const uint CERT_STORE_SAVE_TO_FILENAME_W = 4;
		public const uint CERT_STORE_SAVE_TO_FILENAME = CERT_STORE_SAVE_TO_FILENAME_W;

		// flags for CERT_BASIC_CONSTRAINTS_INFO.SubjectType
		public const uint CERT_CA_SUBJECT_FLAG = 0x80;

		public const uint CERT_END_ENTITY_SUBJECT_FLAG = 0x40;

		// Predefined primitive data structures that can be encoded / decoded.
		public const uint RSA_CSP_PUBLICKEYBLOB = 19;

		public const uint X509_MULTI_BYTE_UINT = 38;
		public const uint X509_DSS_PUBLICKEY = X509_MULTI_BYTE_UINT;
		public const uint X509_DSS_PARAMETERS = 39;
		public const uint X509_DSS_SIGNATURE = 40;

		// Object Identifiers short hand.
		public const uint X509_EXTENSIONS = 5;

		public const uint X509_NAME_VALUE = 6;
		public const uint X509_NAME = 7;
		public const uint X509_AUTHORITY_KEY_ID = 9;
		public const uint X509_KEY_USAGE_RESTRICTION = 11;
		public const uint X509_BASIC_CONSTRAINTS = 13;
		public const uint X509_KEY_USAGE = 14;
		public const uint X509_BASIC_CONSTRAINTS2 = 15;
		public const uint X509_CERT_POLICIES = 16;
		public const uint PKCS_UTC_TIME = 17;
		public const uint PKCS_ATTRIBUTE = 22;
		public const uint X509_UNICODE_NAME_VALUE = 24;
		public const uint X509_OCTET_STRING = 25;
		public const uint X509_BITS = 26;
		public const uint X509_ANY_STRING = X509_NAME_VALUE;
		public const uint X509_UNICODE_ANY_STRING = X509_UNICODE_NAME_VALUE;
		public const uint X509_ENHANCED_KEY_USAGE = 36;
		public const uint PKCS_RC2_CBC_PARAMETERS = 41;
		public const uint X509_CERTIFICATE_TEMPLATE = 64;
		public const uint PKCS7_SIGNER_INFO = 500;
		public const uint CMS_SIGNER_INFO = 501;

		public const string szOID_AUTHORITY_KEY_IDENTIFIER = "2.5.29.1";
		public const string szOID_KEY_USAGE_RESTRICTION = "2.5.29.4";
		public const string szOID_KEY_USAGE = "2.5.29.15";
		public const string szOID_KEYID_RDN = "1.3.6.1.4.1.311.10.7.1";
		public const string szOID_RDN_DUMMY_SIGNER = "1.3.6.1.4.1.311.21.9";

		// Predefined verify chain policies
		public const uint CERT_CHAIN_POLICY_BASE = 1;

		public const uint CERT_CHAIN_POLICY_AUTHENTICODE = 2;
		public const uint CERT_CHAIN_POLICY_AUTHENTICODE_TS = 3;
		public const uint CERT_CHAIN_POLICY_SSL = 4;
		public const uint CERT_CHAIN_POLICY_BASIC_CONSTRAINTS = 5;
		public const uint CERT_CHAIN_POLICY_NT_AUTH = 6;
		public const uint CERT_CHAIN_POLICY_MICROSOFT_ROOT = 7;

		// Default usage match type is AND with value zero
		public const uint USAGE_MATCH_TYPE_AND = 0x00000000;

		public const uint USAGE_MATCH_TYPE_OR = 0x00000001;

		// Common chain policy flags.
		public const uint CERT_CHAIN_REVOCATION_CHECK_END_CERT = 0x10000000;

		public const uint CERT_CHAIN_REVOCATION_CHECK_CHAIN = 0x20000000;
		public const uint CERT_CHAIN_REVOCATION_CHECK_CHAIN_EXCLUDE_ROOT = 0x40000000;
		public const uint CERT_CHAIN_REVOCATION_CHECK_CACHE_ONLY = 0x80000000;
		public const uint CERT_CHAIN_REVOCATION_ACCUMULATIVE_TIMEOUT = 0x08000000;

		// These can be applied to certificates and chains
		public const uint CERT_TRUST_NO_ERROR = 0x00000000;

		public const uint CERT_TRUST_IS_NOT_TIME_VALID = 0x00000001;
		public const uint CERT_TRUST_IS_NOT_TIME_NESTED = 0x00000002;
		public const uint CERT_TRUST_IS_REVOKED = 0x00000004;
		public const uint CERT_TRUST_IS_NOT_SIGNATURE_VALID = 0x00000008;
		public const uint CERT_TRUST_IS_NOT_VALID_FOR_USAGE = 0x00000010;
		public const uint CERT_TRUST_IS_UNTRUSTED_ROOT = 0x00000020;
		public const uint CERT_TRUST_REVOCATION_STATUS_UNKNOWN = 0x00000040;
		public const uint CERT_TRUST_IS_CYCLIC = 0x00000080;

		public const uint CERT_TRUST_INVALID_EXTENSION = 0x00000100;
		public const uint CERT_TRUST_INVALID_POLICY_CONSTRAINTS = 0x00000200;
		public const uint CERT_TRUST_INVALID_BASIC_CONSTRAINTS = 0x00000400;
		public const uint CERT_TRUST_INVALID_NAME_CONSTRAINTS = 0x00000800;
		public const uint CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT = 0x00001000;
		public const uint CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT = 0x00002000;
		public const uint CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT = 0x00004000;
		public const uint CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT = 0x00008000;

		public const uint CERT_TRUST_IS_OFFLINE_REVOCATION = 0x01000000;
		public const uint CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY = 0x02000000;

		// These can be applied to chains only
		public const uint CERT_TRUST_IS_PARTIAL_CHAIN = 0x00010000;

		public const uint CERT_TRUST_CTL_IS_NOT_TIME_VALID = 0x00020000;
		public const uint CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID = 0x00040000;
		public const uint CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE = 0x00080000;

		// Common chain policy flags
		public const uint CERT_CHAIN_POLICY_IGNORE_NOT_TIME_VALID_FLAG = 0x00000001;

		public const uint CERT_CHAIN_POLICY_IGNORE_CTL_NOT_TIME_VALID_FLAG = 0x00000002;
		public const uint CERT_CHAIN_POLICY_IGNORE_NOT_TIME_NESTED_FLAG = 0x00000004;
		public const uint CERT_CHAIN_POLICY_IGNORE_INVALID_BASIC_CONSTRAINTS_FLAG = 0x00000008;

		public const uint CERT_CHAIN_POLICY_ALLOW_UNKNOWN_CA_FLAG = 0x00000010;
		public const uint CERT_CHAIN_POLICY_IGNORE_WRONG_USAGE_FLAG = 0x00000020;
		public const uint CERT_CHAIN_POLICY_IGNORE_INVALID_NAME_FLAG = 0x00000040;
		public const uint CERT_CHAIN_POLICY_IGNORE_INVALID_POLICY_FLAG = 0x00000080;

		public const uint CERT_CHAIN_POLICY_IGNORE_END_REV_UNKNOWN_FLAG = 0x00000100;
		public const uint CERT_CHAIN_POLICY_IGNORE_CTL_SIGNER_REV_UNKNOWN_FLAG = 0x00000200;
		public const uint CERT_CHAIN_POLICY_IGNORE_CA_REV_UNKNOWN_FLAG = 0x00000400;
		public const uint CERT_CHAIN_POLICY_IGNORE_ROOT_REV_UNKNOWN_FLAG = 0x00000800;

		public const uint CERT_CHAIN_POLICY_IGNORE_ALL_REV_UNKNOWN_FLAGS = (
												CERT_CHAIN_POLICY_IGNORE_END_REV_UNKNOWN_FLAG |
												CERT_CHAIN_POLICY_IGNORE_CTL_SIGNER_REV_UNKNOWN_FLAG |
												CERT_CHAIN_POLICY_IGNORE_CA_REV_UNKNOWN_FLAG |
												CERT_CHAIN_POLICY_IGNORE_ROOT_REV_UNKNOWN_FLAG);

		// The following are info status bits

		// These can be applied to certificates only
		public const uint CERT_TRUST_HAS_EXACT_MATCH_ISSUER = 0x00000001;

		public const uint CERT_TRUST_HAS_KEY_MATCH_ISSUER = 0x00000002;
		public const uint CERT_TRUST_HAS_NAME_MATCH_ISSUER = 0x00000004;
		public const uint CERT_TRUST_IS_SELF_SIGNED = 0x00000008;

		// These can be applied to certificates and chains
		public const uint CERT_TRUST_HAS_PREFERRED_ISSUER = 0x00000100;

		public const uint CERT_TRUST_HAS_ISSUANCE_CHAIN_POLICY = 0x00000200;
		public const uint CERT_TRUST_HAS_VALID_NAME_CONSTRAINTS = 0x00000400;

		// These can be applied to chains only
		public const uint CERT_TRUST_IS_COMPLEX_CHAIN = 0x00010000;

		// Signature value that only contains the hash octets. The parameters for this algorithm must
		// be present and must be encoded as NULL.
		public const string szOID_PKIX_NO_SIGNATURE = "1.3.6.1.5.5.7.6.2";

		// Consistent key usage bits: DIGITAL_SIGNATURE, KEY_ENCIPHERMENT or KEY_AGREEMENT
		public const string szOID_PKIX_KP_SERVER_AUTH = "1.3.6.1.5.5.7.3.1";

		// Consistent key usage bits: DIGITAL_SIGNATURE
		public const string szOID_PKIX_KP_CLIENT_AUTH = "1.3.6.1.5.5.7.3.2";

		// Consistent key usage bits: DIGITAL_SIGNATURE
		public const string szOID_PKIX_KP_CODE_SIGNING = "1.3.6.1.5.5.7.3.3";

		// Consistent key usage bits: DIGITAL_SIGNATURE, NON_REPUDIATION and/or (KEY_ENCIPHERMENT or KEY_AGREEMENT)
		public const string szOID_PKIX_KP_EMAIL_PROTECTION = "1.3.6.1.5.5.7.3.4";

		public const string SPC_INDIVIDUAL_SP_KEY_PURPOSE_OBJID = "1.3.6.1.4.1.311.2.1.21";
		public const string SPC_COMMERCIAL_SP_KEY_PURPOSE_OBJID = "1.3.6.1.4.1.311.2.1.22";

		// CertGetCertificateChain chain engine handles.
		public const uint HCCE_CURRENT_USER = 0x0;

		public const uint HCCE_LOCAL_MACHINE = 0x1;

		// PKCS.
		public const string szOID_PKCS_1 = "1.2.840.113549.1.1";

		public const string szOID_PKCS_2 = "1.2.840.113549.1.2";
		public const string szOID_PKCS_3 = "1.2.840.113549.1.3";
		public const string szOID_PKCS_4 = "1.2.840.113549.1.4";
		public const string szOID_PKCS_5 = "1.2.840.113549.1.5";
		public const string szOID_PKCS_6 = "1.2.840.113549.1.6";
		public const string szOID_PKCS_7 = "1.2.840.113549.1.7";
		public const string szOID_PKCS_8 = "1.2.840.113549.1.8";
		public const string szOID_PKCS_9 = "1.2.840.113549.1.9";
		public const string szOID_PKCS_10 = "1.2.840.113549.1.10";
		public const string szOID_PKCS_12 = "1.2.840.113549.1.12";

		// PKCS7 Content Types.
		public const string szOID_RSA_data = "1.2.840.113549.1.7.1";

		public const string szOID_RSA_signedData = "1.2.840.113549.1.7.2";
		public const string szOID_RSA_envelopedData = "1.2.840.113549.1.7.3";
		public const string szOID_RSA_signEnvData = "1.2.840.113549.1.7.4";
		public const string szOID_RSA_digestedData = "1.2.840.113549.1.7.5";
		public const string szOID_RSA_hashedData = "1.2.840.113549.1.7.5";
		public const string szOID_RSA_encryptedData = "1.2.840.113549.1.7.6";

		// PKCS9 Attributes.
		public const string szOID_RSA_emailAddr = "1.2.840.113549.1.9.1";

		public const string szOID_RSA_unstructName = "1.2.840.113549.1.9.2";
		public const string szOID_RSA_contentType = "1.2.840.113549.1.9.3";
		public const string szOID_RSA_messageDigest = "1.2.840.113549.1.9.4";
		public const string szOID_RSA_signingTime = "1.2.840.113549.1.9.5";
		public const string szOID_RSA_counterSign = "1.2.840.113549.1.9.6";
		public const string szOID_RSA_challengePwd = "1.2.840.113549.1.9.7";
		public const string szOID_RSA_unstructAddr = "1.2.840.113549.1.9.8";
		public const string szOID_RSA_extCertAttrs = "1.2.840.113549.1.9.9";
		public const string szOID_RSA_SMIMECapabilities = "1.2.840.113549.1.9.15";

		public const string szOID_CAPICOM = "1.3.6.1.4.1.311.88";     // Reserved for CAPICOM.
		public const string szOID_CAPICOM_version = "1.3.6.1.4.1.311.88.1";   // CAPICOM version
		public const string szOID_CAPICOM_attribute = "1.3.6.1.4.1.311.88.2";   // CAPICOM attribute
		public const string szOID_CAPICOM_documentName = "1.3.6.1.4.1.311.88.2.1"; // Document type attribute
		public const string szOID_CAPICOM_documentDescription = "1.3.6.1.4.1.311.88.2.2"; // Document description attribute
		public const string szOID_CAPICOM_encryptedData = "1.3.6.1.4.1.311.88.3";   // CAPICOM encrypted data message.
		public const string szOID_CAPICOM_encryptedContent = "1.3.6.1.4.1.311.88.3.1"; // CAPICOM content of encrypted data.

		// Digest Algorithms
		public const string szOID_OIWSEC_sha1 = "1.3.14.3.2.26";

		public const string szOID_RSA_MD5 = "1.2.840.113549.2.5";
		public const string szOID_OIWSEC_SHA256 = "2.16.840.1.101.3.4.1";
		public const string szOID_OIWSEC_SHA384 = "2.16.840.1.101.3.4.2";
		public const string szOID_OIWSEC_SHA512 = "2.16.840.1.101.3.4.3";

		// Encryption Algorithms
		public const string szOID_RSA_RC2CBC = "1.2.840.113549.3.2";

		public const string szOID_RSA_RC4 = "1.2.840.113549.3.4";
		public const string szOID_RSA_DES_EDE3_CBC = "1.2.840.113549.3.7";
		public const string szOID_OIWSEC_desCBC = "1.3.14.3.2.7";

		// Key encryption algorithms
		public const string szOID_RSA_SMIMEalg = "1.2.840.113549.1.9.16.3";

		public const string szOID_RSA_SMIMEalgESDH = "1.2.840.113549.1.9.16.3.5";
		public const string szOID_RSA_SMIMEalgCMS3DESwrap = "1.2.840.113549.1.9.16.3.6";
		public const string szOID_RSA_SMIMEalgCMSRC2wrap = "1.2.840.113549.1.9.16.3.7";

		// DSA signing algorithms
		public const string szOID_X957_DSA = "1.2.840.10040.4.1";

		public const string szOID_X957_sha1DSA = "1.2.840.10040.4.3";

		// RSA signing algorithms
		public const string szOID_OIWSEC_sha1RSASign = "1.3.14.3.2.29";

		// Alt Name Types.
		public const uint CERT_ALT_NAME_OTHER_NAME = 1;

		public const uint CERT_ALT_NAME_RFC822_NAME = 2;
		public const uint CERT_ALT_NAME_DNS_NAME = 3;
		public const uint CERT_ALT_NAME_X400_ADDRESS = 4;
		public const uint CERT_ALT_NAME_DIRECTORY_NAME = 5;
		public const uint CERT_ALT_NAME_EDI_PARTY_NAME = 6;
		public const uint CERT_ALT_NAME_URL = 7;
		public const uint CERT_ALT_NAME_IP_ADDRESS = 8;
		public const uint CERT_ALT_NAME_REGISTERED_ID = 9;

		// CERT_RDN Attribute Value Types
		public const uint CERT_RDN_ANY_TYPE = 0;

		public const uint CERT_RDN_ENCODED_BLOB = 1;
		public const uint CERT_RDN_OCTET_STRING = 2;
		public const uint CERT_RDN_NUMERIC_STRING = 3;
		public const uint CERT_RDN_PRINTABLE_STRING = 4;
		public const uint CERT_RDN_TELETEX_STRING = 5;
		public const uint CERT_RDN_T61_STRING = 5;
		public const uint CERT_RDN_VIDEOTEX_STRING = 6;
		public const uint CERT_RDN_IA5_STRING = 7;
		public const uint CERT_RDN_GRAPHIC_STRING = 8;
		public const uint CERT_RDN_VISIBLE_STRING = 9;
		public const uint CERT_RDN_ISO646_STRING = 9;
		public const uint CERT_RDN_GENERAL_STRING = 10;
		public const uint CERT_RDN_UNIVERSAL_STRING = 11;
		public const uint CERT_RDN_INT4_STRING = 11;
		public const uint CERT_RDN_BMP_STRING = 12;
		public const uint CERT_RDN_UNICODE_STRING = 12;
		public const uint CERT_RDN_UTF8_STRING = 13;
		public const uint CERT_RDN_TYPE_MASK = 0x000000FF;
		public const uint CERT_RDN_FLAGS_MASK = 0xFF000000;

		// Certificate Store control types
		public const uint CERT_STORE_CTRL_RESYNC = 1;

		public const uint CERT_STORE_CTRL_NOTIFY_CHANGE = 2;
		public const uint CERT_STORE_CTRL_COMMIT = 3;
		public const uint CERT_STORE_CTRL_AUTO_RESYNC = 4;
		public const uint CERT_STORE_CTRL_CANCEL_NOTIFY = 5;

		// Certificate Identifier
		public const uint CERT_ID_ISSUER_SERIAL_NUMBER = 1;

		public const uint CERT_ID_KEY_IDENTIFIER = 2;
		public const uint CERT_ID_SHA1_HASH = 3;

		// MS provider names.
		public const string MS_ENHANCED_PROV = "Microsoft Enhanced Cryptographic Provider v1.0";

		public const string MS_STRONG_PROV = "Microsoft Strong Cryptographic Provider";
		public const string MS_DEF_PROV = "Microsoft Base Cryptographic Provider v1.0";
		public const string MS_DEF_DSS_DH_PROV = "Microsoft Base DSS and Diffie-Hellman Cryptographic Provider";
		public const string MS_ENH_DSS_DH_PROV = "Microsoft Enhanced DSS and Diffie-Hellman Cryptographic Provider";

		// HashOnly Signature
		public const string DummySignerCommonName = "CN=Dummy Signer";

		// CSP types.
		public const uint PROV_RSA_FULL = 1;

		public const uint PROV_DSS_DH = 13;

		// Algorithm types
		public const uint ALG_TYPE_ANY = (0);

		public const uint ALG_TYPE_DSS = (1 << 9);
		public const uint ALG_TYPE_RSA = (2 << 9);
		public const uint ALG_TYPE_BLOCK = (3 << 9);
		public const uint ALG_TYPE_STREAM = (4 << 9);
		public const uint ALG_TYPE_DH = (5 << 9);
		public const uint ALG_TYPE_SECURECHANNEL = (6 << 9);

		// Algorithm classes
		public const uint ALG_CLASS_ANY = (0);

		public const uint ALG_CLASS_SIGNATURE = (1 << 13);
		public const uint ALG_CLASS_MSG_ENCRYPT = (2 << 13);
		public const uint ALG_CLASS_DATA_ENCRYPT = (3 << 13);
		public const uint ALG_CLASS_HASH = (4 << 13);
		public const uint ALG_CLASS_KEY_EXCHANGE = (5 << 13);
		public const uint ALG_CLASS_ALL = (7 << 13);

		public const uint ALG_SID_ANY = (0);

		// Some RSA sub-ids
		public const uint ALG_SID_RSA_ANY = 0;

		public const uint ALG_SID_RSA_PKCS = 1;
		public const uint ALG_SID_RSA_MSATWORK = 2;
		public const uint ALG_SID_RSA_ENTRUST = 3;
		public const uint ALG_SID_RSA_PGP = 4;

		// Some DSS sub-ids
		public const uint ALG_SID_DSS_ANY = 0;

		public const uint ALG_SID_DSS_PKCS = 1;
		public const uint ALG_SID_DSS_DMS = 2;

		// Block cipher sub ids DES sub_ids
		public const uint ALG_SID_DES = 1;

		public const uint ALG_SID_3DES = 3;
		public const uint ALG_SID_DESX = 4;
		public const uint ALG_SID_IDEA = 5;
		public const uint ALG_SID_CAST = 6;
		public const uint ALG_SID_SAFERSK64 = 7;
		public const uint ALG_SID_SAFERSK128 = 8;
		public const uint ALG_SID_3DES_112 = 9;
		public const uint ALG_SID_CYLINK_MEK = 12;
		public const uint ALG_SID_RC5 = 13;
		public const uint ALG_SID_AES_128 = 14;
		public const uint ALG_SID_AES_192 = 15;
		public const uint ALG_SID_AES_256 = 16;
		public const uint ALG_SID_AES = 17;

		// Fortezza sub-ids
		public const uint ALG_SID_SKIPJACK = 10;

		public const uint ALG_SID_TEK = 11;

		// RC2 sub-ids
		public const uint ALG_SID_RC2 = 2;

		// Stream cipher sub-ids
		public const uint ALG_SID_RC4 = 1;

		public const uint ALG_SID_SEAL = 2;

		// Diffie-Hellman sub-ids
		public const uint ALG_SID_DH_SANDF = 1;

		public const uint ALG_SID_DH_EPHEM = 2;
		public const uint ALG_SID_AGREED_KEY_ANY = 3;
		public const uint ALG_SID_KEA = 4;

		// Hash sub ids
		public const uint ALG_SID_MD2 = 1;

		public const uint ALG_SID_MD4 = 2;
		public const uint ALG_SID_MD5 = 3;
		public const uint ALG_SID_SHA = 4;
		public const uint ALG_SID_SHA1 = 4;
		public const uint ALG_SID_MAC = 5;
		public const uint ALG_SID_RIPEMD = 6;
		public const uint ALG_SID_RIPEMD160 = 7;
		public const uint ALG_SID_SSL3SHAMD5 = 8;
		public const uint ALG_SID_HMAC = 9;
		public const uint ALG_SID_TLS1PRF = 10;
		public const uint ALG_SID_HASH_REPLACE_OWF = 11;

		// secure channel sub ids
		public const uint ALG_SID_SSL3_MASTER = 1;

		public const uint ALG_SID_SCHANNEL_MASTER_HASH = 2;
		public const uint ALG_SID_SCHANNEL_MAC_KEY = 3;
		public const uint ALG_SID_PCT1_MASTER = 4;
		public const uint ALG_SID_SSL2_MASTER = 5;
		public const uint ALG_SID_TLS1_MASTER = 6;
		public const uint ALG_SID_SCHANNEL_ENC_KEY = 7;

		public const uint AT_KEYEXCHANGE = 1;
		public const uint AT_SIGNATURE = 2;

		// algorithm identifier definitions
		public const uint CALG_MD2 = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_MD2);

		public const uint CALG_MD4 = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_MD4);
		public const uint CALG_MD5 = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_MD5);
		public const uint CALG_SHA = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_SHA);
		public const uint CALG_SHA1 = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_SHA1);
		public const uint CALG_MAC = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_MAC);
		public const uint CALG_RSA_SIGN = (ALG_CLASS_SIGNATURE | ALG_TYPE_RSA | ALG_SID_RSA_ANY);
		public const uint CALG_DSS_SIGN = (ALG_CLASS_SIGNATURE | ALG_TYPE_DSS | ALG_SID_DSS_ANY);
		public const uint CALG_NO_SIGN = (ALG_CLASS_SIGNATURE | ALG_TYPE_ANY | ALG_SID_ANY);
		public const uint CALG_RSA_KEYX = (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_RSA | ALG_SID_RSA_ANY);
		public const uint CALG_DES = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_DES);
		public const uint CALG_3DES_112 = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_3DES_112);
		public const uint CALG_3DES = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_3DES);
		public const uint CALG_DESX = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_DESX);
		public const uint CALG_RC2 = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_RC2);
		public const uint CALG_RC4 = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_STREAM | ALG_SID_RC4);
		public const uint CALG_SEAL = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_STREAM | ALG_SID_SEAL);
		public const uint CALG_DH_SF = (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_DH | ALG_SID_DH_SANDF);
		public const uint CALG_DH_EPHEM = (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_DH | ALG_SID_DH_EPHEM);
		public const uint CALG_AGREEDKEY_ANY = (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_DH | ALG_SID_AGREED_KEY_ANY);
		public const uint CALG_KEA_KEYX = (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_DH | ALG_SID_KEA);
		public const uint CALG_HUGHES_MD5 = (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_ANY | ALG_SID_MD5);
		public const uint CALG_SKIPJACK = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_SKIPJACK);
		public const uint CALG_TEK = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_TEK);
		public const uint CALG_CYLINK_MEK = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_CYLINK_MEK);
		public const uint CALG_SSL3_SHAMD5 = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_SSL3SHAMD5);
		public const uint CALG_SSL3_MASTER = (ALG_CLASS_MSG_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_SSL3_MASTER);
		public const uint CALG_SCHANNEL_MASTER_HASH = (ALG_CLASS_MSG_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_SCHANNEL_MASTER_HASH);
		public const uint CALG_SCHANNEL_MAC_KEY = (ALG_CLASS_MSG_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_SCHANNEL_MAC_KEY);
		public const uint CALG_SCHANNEL_ENC_KEY = (ALG_CLASS_MSG_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_SCHANNEL_ENC_KEY);
		public const uint CALG_PCT1_MASTER = (ALG_CLASS_MSG_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_PCT1_MASTER);
		public const uint CALG_SSL2_MASTER = (ALG_CLASS_MSG_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_SSL2_MASTER);
		public const uint CALG_TLS1_MASTER = (ALG_CLASS_MSG_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_TLS1_MASTER);
		public const uint CALG_RC5 = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_RC5);
		public const uint CALG_HMAC = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_HMAC);
		public const uint CALG_TLS1PRF = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_TLS1PRF);
		public const uint CALG_HASH_REPLACE_OWF = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_HASH_REPLACE_OWF);
		public const uint CALG_AES_128 = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_AES_128);
		public const uint CALG_AES_192 = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_AES_192);
		public const uint CALG_AES_256 = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_AES_256);
		public const uint CALG_AES = (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_AES);

		public enum HashParameters
			{
			HP_ALGID = 0x0001,   // Hash algorithm
			HP_HASHVAL = 0x0002,   // Hash value
			HP_HASHSIZE = 0x0004,    // Hash value size
			HP_HMAC_INFO = 0x0005,  // information for creating an HMAC
			HP_TLS1PRF_LABEL = 0x0006,  // label for TLS1 PRF
			HP_TLS1PRF_SEED = 0x0007  // seed for TLS1 PRF
			}

		// Максимальная длина названия контейнера
		public const int MAX_CONTAINER_NAME_LEN = 260;

		// CryptGetProvParam flags
		public const uint CRYPT_FIRST = 1;

		public const uint CRYPT_NEXT = 2;
		public const uint PP_ENUMALGS_EX = 22;

		// CryptGetProvParam dwParam flags
		public const uint PP_ADMIN_PIN = 31;            // Returns the administrator personal identification number (PIN) in the pbData parameter as a LPSTR.

		public const uint PP_CERTCHAIN = 9;         // Returns the certificate chain associated with the hProv handle. The returned certificate chain is X509_ASN_ENCODING encoded
		public const uint PP_CONTAINER = 6; // The name of the current key container as a null-terminated CHAR string. This string is exactly the same as the one passed in the pszContainer parameter of the CryptAcquireContext function to specify the key container to use. The pszContainer parameter can be read to determine the name of the default key container.
		public const uint PP_ENUMALGS = 1; // A PROV_ENUMALGS structure that contains information about one algorithm supported by the CSP being queried
		public const uint PP_ENUMCONTAINERS = 2; //
		public const uint PP_ENUMEX_SIGNING_PROT = 40; //
		public const uint PP_IMPTYPE = 3; //
		public const uint PP_NAME = 4; // The name of the CSP in the form of a null-terminated CHAR string. This string is identical to the one passed in the pszProvider parameter of the CryptAcquireContext function to specify that the current CSP be used.

		public const uint PP_PROVTYPE = 16; //
		public const uint PP_VERSION = 5; //

		// dwFlags definitions for CryptAcquireContext
		public const uint CRYPT_VERIFYCONTEXT = 0xF0000000;

		public const uint CRYPT_NEWKEYSET = 0x00000008;
		public const uint CRYPT_DELETEKEYSET = 0x00000010;
		public const uint CRYPT_MACHINE_KEYSET = 0x00000020;
		public const uint CRYPT_SILENT = 0x00000040;
		public const uint CRYPT_USER_KEYSET = 0x00001000;

		// dwFlag definitions for CryptGenKey
		public const uint CRYPT_EXPORTABLE = 0x00000001;

		public const uint CRYPT_USER_PROTECTED = 0x00000002;
		public const uint CRYPT_CREATE_SALT = 0x00000004;
		public const uint CRYPT_UPDATE_KEY = 0x00000008;
		public const uint CRYPT_NO_SALT = 0x00000010;
		public const uint CRYPT_PREGEN = 0x00000040;
		public const uint CRYPT_RECIPIENT = 0x00000010;
		public const uint CRYPT_INITIATOR = 0x00000040;
		public const uint CRYPT_ONLINE = 0x00000080;
		public const uint CRYPT_SF = 0x00000100;
		public const uint CRYPT_CREATE_IV = 0x00000200;
		public const uint CRYPT_KEK = 0x00000400;
		public const uint CRYPT_DATA_KEY = 0x00000800;
		public const uint CRYPT_VOLATILE = 0x00001000;
		public const uint CRYPT_SGCKEY = 0x00002000;
		public const uint CRYPT_ARCHIVABLE = 0x00004000;

		public const byte CUR_BLOB_VERSION = 2;

		// Exported key blob definitions
		public const byte SIMPLEBLOB = 0x1;

		public const byte PUBLICKEYBLOB = 0x6;
		public const byte PRIVATEKEYBLOB = 0x7;
		public const byte PLAINTEXTKEYBLOB = 0x8;
		public const byte OPAQUEKEYBLOB = 0x9;
		public const byte PUBLICKEYBLOBEX = 0xA;
		public const byte SYMMETRICWRAPKEYBLOB = 0xB;

		// Magic constants
		public const uint DSS_MAGIC = 0x31535344;

		public const uint DSS_PRIVATE_MAGIC = 0x32535344;
		public const uint DSS_PUB_MAGIC_VER3 = 0x33535344;
		public const uint DSS_PRIV_MAGIC_VER3 = 0x34535344;
		public const uint RSA_PUB_MAGIC = 0x31415352;
		public const uint RSA_PRIV_MAGIC = 0x32415352;

		// CryptAcquireCertificatePrivateKey dwFlags
		public const uint CRYPT_ACQUIRE_CACHE_FLAG = 0x00000001;

		public const uint CRYPT_ACQUIRE_USE_PROV_INFO_FLAG = 0x00000002;
		public const uint CRYPT_ACQUIRE_COMPARE_KEY_FLAG = 0x00000004;
		public const uint CRYPT_ACQUIRE_SILENT_FLAG = 0x00000040;

		// CryptMsgOpenToDecode dwFlags
		public const uint CMSG_BARE_CONTENT_FLAG = 0x00000001;

		public const uint CMSG_LENGTH_ONLY_FLAG = 0x00000002;
		public const uint CMSG_DETACHED_FLAG = 0x00000004;
		public const uint CMSG_AUTHENTICATED_ATTRIBUTES_FLAG = 0x00000008;
		public const uint CMSG_CONTENTS_OCTETS_FLAG = 0x00000010;
		public const uint CMSG_MAX_LENGTH_FLAG = 0x00000020;

		// Get parameter types and their corresponding data structure definitions.
		public const uint CMSG_TYPE_PARAM = 1;

		public const uint CMSG_CONTENT_PARAM = 2;
		public const uint CMSG_BARE_CONTENT_PARAM = 3;
		public const uint CMSG_INNER_CONTENT_TYPE_PARAM = 4;
		public const uint CMSG_SIGNER_COUNT_PARAM = 5;
		public const uint CMSG_SIGNER_INFO_PARAM = 6;
		public const uint CMSG_SIGNER_CERT_INFO_PARAM = 7;
		public const uint CMSG_SIGNER_HASH_ALGORITHM_PARAM = 8;
		public const uint CMSG_SIGNER_AUTH_ATTR_PARAM = 9;
		public const uint CMSG_SIGNER_UNAUTH_ATTR_PARAM = 10;
		public const uint CMSG_CERT_COUNT_PARAM = 11;
		public const uint CMSG_CERT_PARAM = 12;
		public const uint CMSG_CRL_COUNT_PARAM = 13;
		public const uint CMSG_CRL_PARAM = 14;
		public const uint CMSG_ENVELOPE_ALGORITHM_PARAM = 15;
		public const uint CMSG_RECIPIENT_COUNT_PARAM = 17;
		public const uint CMSG_RECIPIENT_INDEX_PARAM = 18;
		public const uint CMSG_RECIPIENT_INFO_PARAM = 19;
		public const uint CMSG_HASH_ALGORITHM_PARAM = 20;
		public const uint CMSG_HASH_DATA_PARAM = 21;
		public const uint CMSG_COMPUTED_HASH_PARAM = 22;
		public const uint CMSG_ENCRYPT_PARAM = 26;
		public const uint CMSG_ENCRYPTED_DIGEST = 27;
		public const uint CMSG_ENCODED_SIGNER = 28;
		public const uint CMSG_ENCODED_MESSAGE = 29;
		public const uint CMSG_VERSION_PARAM = 30;
		public const uint CMSG_ATTR_CERT_COUNT_PARAM = 31;
		public const uint CMSG_ATTR_CERT_PARAM = 32;
		public const uint CMSG_CMS_RECIPIENT_COUNT_PARAM = 33;
		public const uint CMSG_CMS_RECIPIENT_INDEX_PARAM = 34;
		public const uint CMSG_CMS_RECIPIENT_ENCRYPTED_KEY_INDEX_PARAM = 35;
		public const uint CMSG_CMS_RECIPIENT_INFO_PARAM = 36;
		public const uint CMSG_UNPROTECTED_ATTR_PARAM = 37;
		public const uint CMSG_SIGNER_CERT_ID_PARAM = 38;
		public const uint CMSG_CMS_SIGNER_INFO_PARAM = 39;

		// Message control types.
		public const uint CMSG_CTRL_VERIFY_SIGNATURE = 1;

		public const uint CMSG_CTRL_DECRYPT = 2;
		public const uint CMSG_CTRL_VERIFY_HASH = 5;
		public const uint CMSG_CTRL_ADD_SIGNER = 6;
		public const uint CMSG_CTRL_DEL_SIGNER = 7;
		public const uint CMSG_CTRL_ADD_SIGNER_UNAUTH_ATTR = 8;
		public const uint CMSG_CTRL_DEL_SIGNER_UNAUTH_ATTR = 9;
		public const uint CMSG_CTRL_ADD_CERT = 10;
		public const uint CMSG_CTRL_DEL_CERT = 11;
		public const uint CMSG_CTRL_ADD_CRL = 12;
		public const uint CMSG_CTRL_DEL_CRL = 13;
		public const uint CMSG_CTRL_ADD_ATTR_CERT = 14;
		public const uint CMSG_CTRL_DEL_ATTR_CERT = 15;
		public const uint CMSG_CTRL_KEY_TRANS_DECRYPT = 16;
		public const uint CMSG_CTRL_KEY_AGREE_DECRYPT = 17;
		public const uint CMSG_CTRL_MAIL_LIST_DECRYPT = 18;
		public const uint CMSG_CTRL_VERIFY_SIGNATURE_EX = 19;
		public const uint CMSG_CTRL_ADD_CMS_SIGNER_INFO = 20;

		// Signer Types
		public const uint CMSG_VERIFY_SIGNER_PUBKEY = 1; // pvSigner: PCERT_PUBLIC_KEY_INFO

		public const uint CMSG_VERIFY_SIGNER_CERT = 2; // pvSigner: PCCERT_CONTEXT
		public const uint CMSG_VERIFY_SIGNER_CHAIN = 3; // pvSigner: PCCERT_CHAIN_CONTEXT
		public const uint CMSG_VERIFY_SIGNER_NULL = 4; // pvSigner: NULL

		// Message types.
		public const uint CMSG_DATA = 1;

		public const uint CMSG_SIGNED = 2;
		public const uint CMSG_ENVELOPED = 3;
		public const uint CMSG_SIGNED_AND_ENVELOPED = 4;
		public const uint CMSG_HASHED = 5;
		public const uint CMSG_ENCRYPTED = 6;

		// Recipient types
		public const uint CMSG_KEY_TRANS_RECIPIENT = 1;

		public const uint CMSG_KEY_AGREE_RECIPIENT = 2;
		public const uint CMSG_MAIL_LIST_RECIPIENT = 3;

		// Key agree type
		public const uint CMSG_KEY_AGREE_ORIGINATOR_CERT = 1;

		public const uint CMSG_KEY_AGREE_ORIGINATOR_PUBLIC_KEY = 2;

		// Key agree choices
		public const uint CMSG_KEY_AGREE_EPHEMERAL_KEY_CHOICE = 1;

		public const uint CMSG_KEY_AGREE_STATIC_KEY_CHOICE = 2;

		// dwVersion numbers for the KeyTrans, KeyAgree and MailList recipients
		public const uint CMSG_ENVELOPED_RECIPIENT_V0 = 0;

		public const uint CMSG_ENVELOPED_RECIPIENT_V2 = 2;
		public const uint CMSG_ENVELOPED_RECIPIENT_V3 = 3;
		public const uint CMSG_ENVELOPED_RECIPIENT_V4 = 4;
		public const uint CMSG_KEY_TRANS_PKCS_1_5_VERSION = CMSG_ENVELOPED_RECIPIENT_V0;
		public const uint CMSG_KEY_TRANS_CMS_VERSION = CMSG_ENVELOPED_RECIPIENT_V2;
		public const uint CMSG_KEY_AGREE_VERSION = CMSG_ENVELOPED_RECIPIENT_V3;
		public const uint CMSG_MAIL_LIST_VERSION = CMSG_ENVELOPED_RECIPIENT_V4;

		// RC2 encryption algorithm version (key length).
		public const uint CRYPT_RC2_40BIT_VERSION = 160;

		public const uint CRYPT_RC2_56BIT_VERSION = 52;
		public const uint CRYPT_RC2_64BIT_VERSION = 120;
		public const uint CRYPT_RC2_128BIT_VERSION = 58;

		// Error codes.
		public const int E_NOTIMPL = unchecked((int) 0x80000001); // Not implemented.

		public const int E_FILENOTFOUND = unchecked((int) 0x80070002); // File not found
		public const int E_OUTOFMEMORY = unchecked((int) 0x8007000E); // Ran out of memory.
		public const int NTE_NO_KEY = unchecked((int) 0x8009000D); // Key does not exist.
		public const int NTE_BAD_PUBLIC_KEY = unchecked((int) 0x80090015); // Provider's public key is invalid.
		public const int NTE_BAD_KEYSET = unchecked((int) 0x80090016); // Keyset does not exist
		public const int CRYPT_E_MSG_ERROR = unchecked((int) 0x80091001); // An error occurred while performing an operation on a cryptographic message.
		public const int CRYPT_E_UNKNOWN_ALGO = unchecked((int) 0x80091002); // Unknown cryptographic algorithm.
		public const int CRYPT_E_INVALID_MSG_TYPE = unchecked((int) 0x80091004); // Invalid cryptographic message type.
		public const int CRYPT_E_RECIPIENT_NOT_FOUND = unchecked((int) 0x8009100B); // The enveloped-data message does not contain the specified recipient.
		public const int CRYPT_E_ISSUER_SERIALNUMBER = unchecked((int) 0x8009100D); // Invalid issuer and/or serial number.
		public const int CRYPT_E_SIGNER_NOT_FOUND = unchecked((int) 0x8009100E); // Cannot find the original signer.
		public const int CRYPT_E_ATTRIBUTES_MISSING = unchecked((int) 0x8009100F); // The cryptographic message does not contain all of the requested attributes.
		public const int CRYPT_E_BAD_ENCODE = unchecked((int) 0x80092002); // An error occurred during encode or decode operation.
		public const int CRYPT_E_NOT_FOUND = unchecked((int) 0x80092004); // Cannot find object or property.
		public const int CRYPT_E_NO_MATCH = unchecked((int) 0x80092009); // Cannot find the requested object.
		public const int CRYPT_E_NO_SIGNER = unchecked((int) 0x8009200E); // The signed cryptographic message does not have a signer for the specified signer index.
		public const int CRYPT_E_REVOKED = unchecked((int) 0x80092010); // The certificate is revoked.
		public const int CRYPT_E_NO_REVOCATION_CHECK = unchecked((int) 0x80092012); // The revocation function was unable to check revocation for the certificate.
		public const int CRYPT_E_REVOCATION_OFFLINE = unchecked((int) 0x80092013); // The revocation function was unable to check revocation

																				   // because the
																				   // revocation
																				   // server was offline.
		public const int CRYPT_E_ASN1_BADTAG = unchecked((int) 0x8009310B); // ASN1 bad tag value met.

		public const int TRUST_E_CERT_SIGNATURE = unchecked((int) 0x80096004); // The signature of the certificate can not be verified.
		public const int TRUST_E_BASIC_CONSTRAINTS = unchecked((int) 0x80096019); // A certificate's basic constraint extension has not been observed.
		public const int CERT_E_EXPIRED = unchecked((int) 0x800B0101); // A required certificate is not within its validity period when verifying against

																	   // the current system clock or
																	   // the timestamp in the signed file.
		public const int CERT_E_VALIDITYPERIODNESTING = unchecked((int) 0x800B0102); // The validity periods of the certification chain do not nest correctly.

		public const int CERT_E_UNTRUSTEDROOT = unchecked((int) 0x800B0109); // A certificate chain processed, but terminated in a root

																			 // certificate which is
																			 // not trusted by the
																			 // trust provider.
		public const int CERT_E_CHAINING = unchecked((int) 0x800B010A); // An public certificate chaining error has occurred.

		public const int TRUST_E_FAIL = unchecked((int) 0x800B010B); // Generic trust failure.
		public const int CERT_E_REVOKED = unchecked((int) 0x800B010C); // A certificate was explicitly revoked by its issuer.
		public const int CERT_E_UNTRUSTEDTESTROOT = unchecked((int) 0x800B010D); // The certification path terminates with the test root which

																				 // is not trusted
																				 // with the current
																				 // policy settings.
		public const int CERT_E_REVOCATION_FAILURE = unchecked((int) 0x800B010E); // The revocation process could not continue - the certificate(s) could not be checked.

		public const int CERT_E_WRONG_USAGE = unchecked((int) 0x800B0110); // The certificate is not valid for the requested usage.
		public const int CERT_E_INVALID_POLICY = unchecked((int) 0x800B0113); // The certificate has invalid policy.
		public const int CERT_E_INVALID_NAME = unchecked((int) 0x800B0114); // The certificate has an invalid name. The name is not included

																			// in the permitted list
																			// or is explicitly excluded.

		public const int ERROR_SUCCESS = 0;                           // The operation completed successfully.
		public const int ERROR_FILE_NOT_FOUND = 2;                           // File not found
		public const int ERROR_CALL_NOT_IMPLEMENTED = 120;                         // This function is not supported on this system.
		public const int ERROR_CANCELLED = 1223;                        // The operation was canceled by the user.

		// Structures

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct BLOBHEADER
			{
			public byte bType;
			public byte bVersion;
			public short reserved;
			public uint aiKeyAlg;
			};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_ALT_NAME_INFO
			{
			public uint cAltEntry;
			public IntPtr rgAltEntry; // PCERT_ALT_NAME_ENTRY
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_BASIC_CONSTRAINTS_INFO
			{
			public CRYPT_BIT_BLOB SubjectType;
			public bool fPathLenConstraint;
			public uint dwPathLenConstraint;
			public uint cSubtreesConstraint;
			public IntPtr rgSubtreesConstraint; // PCERT_NAME_BLOB
			};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_BASIC_CONSTRAINTS2_INFO
			{
			public int fCA;
			public int fPathLenConstraint;
			public uint dwPathLenConstraint;
			};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_CHAIN_PARA
			{
			public uint cbSize;
			public CERT_USAGE_MATCH RequestedUsage;
			public CERT_USAGE_MATCH RequestedIssuancePolicy;
			public uint dwUrlRetrievalTimeout; // milliseconds
			public bool fCheckRevocationFreshnessTime;
			public uint dwRevocationFreshnessTime;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_CHAIN_POLICY_PARA
			{
			public CERT_CHAIN_POLICY_PARA(int size)
				{
				cbSize = (uint) size;
				dwFlags = 0;
				pvExtraPolicyPara = IntPtr.Zero;
				}

			public uint cbSize;
			public uint dwFlags;
			public IntPtr pvExtraPolicyPara;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_CHAIN_POLICY_STATUS
			{
			public CERT_CHAIN_POLICY_STATUS(int size)
				{
				cbSize = (uint) size;
				dwError = 0;
				lChainIndex = IntPtr.Zero;
				lElementIndex = IntPtr.Zero;
				pvExtraPolicyStatus = IntPtr.Zero;
				}

			public uint cbSize;
			public uint dwError;
			public IntPtr lChainIndex;
			public IntPtr lElementIndex;
			public IntPtr pvExtraPolicyStatus;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_CONTEXT
			{
			public uint dwCertEncodingType;
			public IntPtr pbCertEncoded;
			public uint cbCertEncoded;
			public IntPtr pCertInfo;
			public IntPtr hCertStore;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_DSS_PARAMETERS
			{
			public CRYPTOAPI_BLOB p;
			public CRYPTOAPI_BLOB q;
			public CRYPTOAPI_BLOB g;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_ENHKEY_USAGE
			{
			public uint cUsageIdentifier;
			public IntPtr rgpszUsageIdentifier; // LPSTR*
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_EXTENSION
			{
			[MarshalAs(UnmanagedType.LPStr)]
			public string pszObjId;

			public bool fCritical;
			public CRYPTOAPI_BLOB Value;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_ID
			{
			public uint dwIdChoice;
			public CERT_ID_UNION Value;
			}

		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct CERT_ID_UNION
			{
			[FieldOffset(0)]
			public CERT_ISSUER_SERIAL_NUMBER IssuerSerialNumber; // CERT_ID_ISSUER_SERIAL_NUMBER

			[FieldOffset(0)]
			public CRYPTOAPI_BLOB KeyId;                         // CERT_ID_KEY_IDENTIFIER

			[FieldOffset(0)]
			public CRYPTOAPI_BLOB HashId;                        // CERT_ID_SHA1_HASH
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_ISSUER_SERIAL_NUMBER
			{
			public CRYPTOAPI_BLOB Issuer;
			public CRYPTOAPI_BLOB SerialNumber;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_INFO
			{
			public uint dwVersion;
			public CRYPTOAPI_BLOB SerialNumber;
			public CRYPT_ALGORITHM_IDENTIFIER SignatureAlgorithm;
			public CRYPTOAPI_BLOB Issuer;
			public _FILETIME NotBefore;
			public _FILETIME NotAfter;
			public CRYPTOAPI_BLOB Subject;
			public CERT_PUBLIC_KEY_INFO SubjectPublicKeyInfo;
			public CRYPT_BIT_BLOB IssuerUniqueId;
			public CRYPT_BIT_BLOB SubjectUniqueId;
			public uint cExtension;
			public IntPtr rgExtension; // PCERT_EXTENSION
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_KEY_USAGE_RESTRICTION_INFO
			{
			public uint cCertPolicyId;
			public IntPtr rgCertPolicyId;     // LPSTR*
			public CRYPT_BIT_BLOB RestrictedKeyUsage;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_NAME_INFO
			{
			public uint cRDN;
			public IntPtr rgRDN; // PCERT_RDN
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_NAME_VALUE
			{
			public uint dwValueType;
			public CRYPTOAPI_BLOB Value;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_OTHER_NAME
			{
			[MarshalAs(UnmanagedType.LPStr)]
			public string pszObjId;

			public CRYPTOAPI_BLOB Value;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_POLICY_ID
			{
			public uint cCertPolicyElementId;
			public IntPtr rgpszCertPolicyElementId; // LPSTR*
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_POLICIES_INFO
			{
			public uint cPolicyInfo;
			public IntPtr rgPolicyInfo;      // PCERT_POLICY_INFO
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_POLICY_INFO
			{
			[MarshalAs(UnmanagedType.LPStr)]
			public string pszPolicyIdentifier;

			public uint cPolicyQualifier;
			public IntPtr rgPolicyQualifier; // PCERT_POLICY_QUALIFIER_INFO
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_POLICY_QUALIFIER_INFO
			{
			[MarshalAs(UnmanagedType.LPStr)]
			public string pszPolicyQualifierId;

			private CRYPTOAPI_BLOB Qualifier;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_PUBLIC_KEY_INFO
			{
			public CRYPT_ALGORITHM_IDENTIFIER Algorithm;
			public CRYPT_BIT_BLOB PublicKey;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_PUBLIC_KEY_INFO2
			{
			public CRYPT_ALGORITHM_IDENTIFIER2 Algorithm;
			public CRYPT_BIT_BLOB PublicKey;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_RDN
			{
			public uint cRDNAttr;
			public IntPtr rgRDNAttr; // PCERT_RDN_ATTR
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_RDN_ATTR
			{
			[MarshalAs(UnmanagedType.LPStr)]
			public string pszObjId;

			public uint dwValueType;
			public CRYPTOAPI_BLOB Value;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_TEMPLATE_EXT
			{
			[MarshalAs(UnmanagedType.LPStr)]
			public string pszObjId;

			public uint dwMajorVersion;
			private bool fMinorVersion;
			private uint dwMinorVersion;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_TRUST_STATUS
			{
			public uint dwErrorStatus;
			public uint dwInfoStatus;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CERT_USAGE_MATCH
			{
			public uint dwType;
			public CERT_ENHKEY_USAGE Usage;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_CMS_RECIPIENT_INFO
			{
			public uint dwRecipientChoice;

			// union { PCMSG_KEY_TRANS_RECIPIENT_INFO pKeyTrans; // CMSG_KEY_TRANS_RECIPIENT
			// PCMSG_KEY_AGREE_RECIPIENT_INFO pKeyAgree; // CMSG_KEY_AGREE_RECIPIENT
			// PCMSG_MAIL_LIST_RECIPIENT_INFO pMailList; // CMSG_MAIL_LIST_RECIPIENT }
			public IntPtr pRecipientInfo;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_CMS_SIGNER_INFO
			{
			public uint dwVersion;
			public CERT_ID SignerId;
			public CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;
			public CRYPT_ALGORITHM_IDENTIFIER HashEncryptionAlgorithm;
			public CRYPTOAPI_BLOB EncryptedHash;
			public CRYPT_ATTRIBUTES AuthAttrs;
			public CRYPT_ATTRIBUTES UnauthAttrs;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_CTRL_ADD_SIGNER_UNAUTH_ATTR_PARA
			{
			public CMSG_CTRL_ADD_SIGNER_UNAUTH_ATTR_PARA(int size)
				{
				cbSize = (uint) size;
				dwSignerIndex = 0;
				blob = new CRYPTOAPI_BLOB();
				}

			public uint cbSize;
			public uint dwSignerIndex;
			public CRYPTOAPI_BLOB blob;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_CTRL_DECRYPT_PARA
			{
			public CMSG_CTRL_DECRYPT_PARA(int size)
				{
				cbSize = (uint) size;
				hCryptProv = IntPtr.Zero;
				dwKeySpec = 0;
				dwRecipientIndex = 0;
				}

			public uint cbSize;
			public IntPtr hCryptProv;
			public uint dwKeySpec;
			public uint dwRecipientIndex;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_CTRL_DEL_SIGNER_UNAUTH_ATTR_PARA
			{
			public CMSG_CTRL_DEL_SIGNER_UNAUTH_ATTR_PARA(int size)
				{
				cbSize = (uint) size;
				dwSignerIndex = 0;
				dwUnauthAttrIndex = 0;
				}

			public uint cbSize;
			public uint dwSignerIndex;
			public uint dwUnauthAttrIndex;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_CTRL_KEY_TRANS_DECRYPT_PARA
			{
			public uint cbSize;

			[SecurityCritical]
			public SafeCryptProvHandle hCryptProv;

			public uint dwKeySpec;
			public IntPtr pKeyTrans; // PCMSG_KEY_TRANS_RECIPIENT_INFO
			public uint dwRecipientIndex;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_KEY_AGREE_RECIPIENT_ENCODE_INFO
			{
			public uint cbSize;
			public CRYPT_ALGORITHM_IDENTIFIER KeyEncryptionAlgorithm;
			public IntPtr pvKeyEncryptionAuxInfo;
			public CRYPT_ALGORITHM_IDENTIFIER KeyWrapAlgorithm;
			public IntPtr pvKeyWrapAuxInfo;

			// The following hCryptProv and dwKeySpec must be specified for the CMSG_KEY_AGREE_STATIC_KEY_CHOICE.
			//
			// For CMSG_KEY_AGREE_EPHEMERAL_KEY_CHOICE, dwKeySpec isn't applicable and hCryptProv is optional.
			public IntPtr hCryptProv; // HCRYPTPROV

			public uint dwKeySpec;
			public uint dwKeyChoice;

			// union { PCRYPT_ALGORITHM_IDENTIFIER // for CMSG_KEY_AGREE_EPHEMERAL_KEY_CHOICE
			// PCERT_ID // for CMSG_KEY_AGREE_STATIC_KEY_CHOICE }
			public IntPtr pEphemeralAlgorithmOrSenderId;  // PCRYPT_ALGORITHM_IDENTIFIER or PCERT_ID

			public CRYPTOAPI_BLOB UserKeyingMaterial; // OPTIONAL
			public uint cRecipientEncryptedKeys;
			public IntPtr rgpRecipientEncryptedKeys;      // PCMSG_RECIPIENT_ENCRYPTED_KEY_ENCODE_INFO *
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_KEY_TRANS_RECIPIENT_ENCODE_INFO
			{
			public uint cbSize;
			public CRYPT_ALGORITHM_IDENTIFIER KeyEncryptionAlgorithm;
			public IntPtr pvKeyEncryptionAuxInfo;
			public IntPtr hCryptProv; // HCRYPTPROV
			public CRYPT_BIT_BLOB RecipientPublicKey;
			public CERT_ID RecipientId;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_RC2_AUX_INFO
			{
			public CMSG_RC2_AUX_INFO(int size)
				{
				cbSize = (uint) size;
				dwBitLen = 0;
				}

			public uint cbSize;
			public uint dwBitLen;
			}

		//public struct CMSG_RECIPIENT_ENCODE_INFO
		//	{
		//	public uint dwRecipientChoice;
		//	// union {
		//	//  PCMSG_KEY_TRANS_RECIPIENT_ENCODE_INFO pKeyTrans; // CMSG_KEY_TRANS_RECIPIENT
		//	//  PCMSG_KEY_AGREE_RECIPIENT_ENCODE_INFO pKeyAgree; // CMSG_KEY_AGREE_RECIPIENT
		//	//  PCMSG_MAIL_LIST_RECIPIENT_ENCODE_INFO pMailList; // CMSG_MAIL_LIST_RECIPIENT
		//	// }
		//	//
		//	public IntPtr pRecipientInfo;
		//	}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_RECIPIENT_ENCRYPTED_KEY_ENCODE_INFO
			{
			public uint cbSize;
			public CRYPT_BIT_BLOB RecipientPublicKey;
			public CERT_ID RecipientId;

			// Following fields are optional and only applicable to KEY_IDENTIFIER CERT_IDs.
			public _FILETIME Date;

			public IntPtr pOtherAttr; // PCRYPT_ATTRIBUTE_TYPE_VALUE
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_ENVELOPED_ENCODE_INFO
			{
			public CMSG_ENVELOPED_ENCODE_INFO(int size)
				{
				cbSize = (uint) size;
				hCryptProv = IntPtr.Zero;
				ContentEncryptionAlgorithm = new CRYPT_ALGORITHM_IDENTIFIER();
				pvEncryptionAuxInfo = IntPtr.Zero;
				cRecipients = 0;
				rgpRecipients = IntPtr.Zero;
				rgCmsRecipients = IntPtr.Zero;
				cCertEncoded = 0;
				rgCertEncoded = IntPtr.Zero;
				cCrlEncoded = 0;
				rgCrlEncoded = IntPtr.Zero;
				cAttrCertEncoded = 0;
				rgAttrCertEncoded = IntPtr.Zero;
				cUnprotectedAttr = 0;
				rgUnprotectedAttr = IntPtr.Zero;
				}

			public uint cbSize;
			public IntPtr hCryptProv;
			public CRYPT_ALGORITHM_IDENTIFIER ContentEncryptionAlgorithm;
			public IntPtr pvEncryptionAuxInfo;
			public uint cRecipients;
			public IntPtr rgpRecipients; // PCERT_INFO

										 // If rgCmsRecipients != NULL, then, the above rgpRecipients
										 // must be NULL.
			public IntPtr rgCmsRecipients; // PCMSG_RECIPIENT_ENCODE_INFO

			public uint cCertEncoded;
			public IntPtr rgCertEncoded; // PCERT_BLOB
			public uint cCrlEncoded;
			public IntPtr rgCrlEncoded; // PCRL_BLOB
			public uint cAttrCertEncoded;
			public IntPtr rgAttrCertEncoded; // PCERT_BLOB
			public uint cUnprotectedAttr;
			public IntPtr rgUnprotectedAttr; // PCRYPT_ATTRIBUTE
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_CTRL_KEY_AGREE_DECRYPT_PARA
			{
			public CMSG_CTRL_KEY_AGREE_DECRYPT_PARA(int size)
				{
				cbSize = (uint) size;
				hCryptProv = IntPtr.Zero;
				dwKeySpec = 0;
				pKeyAgree = IntPtr.Zero;
				dwRecipientIndex = 0;
				dwRecipientEncryptedKeyIndex = 0;
				OriginatorPublicKey = new CRYPT_BIT_BLOB();
				}

			public uint cbSize;
			public IntPtr hCryptProv;
			public uint dwKeySpec;
			public IntPtr pKeyAgree; // PCMSG_KEY_AGREE_RECIPIENT_INFO
			public uint dwRecipientIndex;
			public uint dwRecipientEncryptedKeyIndex;
			public CRYPT_BIT_BLOB OriginatorPublicKey;
			}

		// CLR marshaller can't handle this kind of union (with managed type in structures within the
		// union (OriginatorPublicKeyInfo.Algorithm.pszObjId is declared as managed string type). So
		// break it up into different structures ourselves, and marshal accordingly.
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_KEY_AGREE_RECIPIENT_INFO
			{
			public uint dwVersion;
			public uint dwOriginatorChoice;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_KEY_AGREE_CERT_ID_RECIPIENT_INFO
			{
			public uint dwVersion;
			public uint dwOriginatorChoice;         // CMSG_KEY_AGREE_ORIGINATOR_CERT
			public CERT_ID OriginatorCertId;           // CERT_ID
			public IntPtr Padding;                    // Padded to the size of CERT_PUBLIC_KEY_INFO
			public CRYPTOAPI_BLOB UserKeyingMaterial;
			public CRYPT_ALGORITHM_IDENTIFIER KeyEncryptionAlgorithm;
			public uint cRecipientEncryptedKeys;
			public IntPtr rgpRecipientEncryptedKeys;  // PCMSG_RECIPIENT_ENCRYPTED_KEY_INFO*
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_KEY_AGREE_PUBLIC_KEY_RECIPIENT_INFO
			{
			public uint dwVersion;
			public uint dwOriginatorChoice;         // CMSG_KEY_AGREE_ORIGINATOR_PUBLIC_KEY
			public CERT_PUBLIC_KEY_INFO OriginatorPublicKeyInfo;    // CERT_PUBLIC_KEY_INFO
			public CRYPTOAPI_BLOB UserKeyingMaterial;
			public CRYPT_ALGORITHM_IDENTIFIER KeyEncryptionAlgorithm;
			public uint cRecipientEncryptedKeys;
			public IntPtr rgpRecipientEncryptedKeys; // PCMSG_RECIPIENT_ENCRYPTED_KEY_INFO *
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_RECIPIENT_ENCRYPTED_KEY_INFO
			{
			// Currently, only ISSUER_SERIAL_NUMBER or KEYID choices
			public CERT_ID RecipientId;

			public CRYPTOAPI_BLOB EncryptedKey;

			// The following optional fields are only applicable to KEYID choice
			public _FILETIME Date;

			public IntPtr pOtherAttr; //PCRYPT_ATTRIBUTE_TYPE_VALUE
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_CTRL_VERIFY_SIGNATURE_EX_PARA
			{
			public CMSG_CTRL_VERIFY_SIGNATURE_EX_PARA(int size)
				{
				cbSize = (uint) size;
				hCryptProv = IntPtr.Zero;
				dwSignerIndex = 0;
				dwSignerType = 0;
				pvSigner = IntPtr.Zero;
				}

			public uint cbSize;
			public IntPtr hCryptProv; // HCRYPTPROV
			public uint dwSignerIndex;
			public uint dwSignerType;
			public IntPtr pvSigner;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_KEY_TRANS_RECIPIENT_INFO
			{
			public uint dwVersion;

			// Currently, only ISSUER_SERIAL_NUMBER or KEYID choices
			public CERT_ID RecipientId;

			public CRYPT_ALGORITHM_IDENTIFIER KeyEncryptionAlgorithm;
			public CRYPTOAPI_BLOB EncryptedKey;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_SIGNED_ENCODE_INFO
			{
			public CMSG_SIGNED_ENCODE_INFO(int size)
				{
				cbSize = (uint) size;
				cSigners = 0;
				rgSigners = IntPtr.Zero;
				cCertEncoded = 0;
				rgCertEncoded = IntPtr.Zero;
				cCrlEncoded = 0;
				rgCrlEncoded = IntPtr.Zero;
				cAttrCertEncoded = 0;
				rgAttrCertEncoded = IntPtr.Zero;
				}

			public uint cbSize;
			public uint cSigners;
			public IntPtr rgSigners;
			public uint cCertEncoded;
			public IntPtr rgCertEncoded;
			public uint cCrlEncoded;
			public IntPtr rgCrlEncoded;
			public uint cAttrCertEncoded;
			public IntPtr rgAttrCertEncoded;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_SIGNER_ENCODE_INFO
			{
			[DllImport(KERNEL32, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			IntPtr LocalFree(IntPtr hMem);

			[DllImport(ADVAPI32, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CryptReleaseContext(
				[In]     IntPtr hProv,
				[In]     uint dwFlags);

			public CMSG_SIGNER_ENCODE_INFO(int size)
				{
				cbSize = (uint) size;
				pCertInfo = IntPtr.Zero;
				hCryptProv = IntPtr.Zero;
				dwKeySpec = 0;
				HashAlgorithm = new CRYPT_ALGORITHM_IDENTIFIER();
				pvHashAuxInfo = IntPtr.Zero;
				cAuthAttr = 0;
				rgAuthAttr = IntPtr.Zero;
				cUnauthAttr = 0;
				rgUnauthAttr = IntPtr.Zero;
				SignerId = new CERT_ID();
				HashEncryptionAlgorithm = new CRYPT_ALGORITHM_IDENTIFIER();
				pvHashEncryptionAuxInfo = IntPtr.Zero;
				}

			[SecuritySafeCritical]
			public void Dispose()
				{
				// Free hCryptProv
				if (hCryptProv != IntPtr.Zero)
					{
					CryptReleaseContext(hCryptProv, 0);
					}

				// Free SignerId.KeyId.pbData
				if (SignerId.Value.KeyId.pbData != IntPtr.Zero)
					{
					LocalFree(SignerId.Value.KeyId.pbData);
					}

				// Free rgAuthAttr and rgUnauthAttr
				if (rgAuthAttr != IntPtr.Zero)
					{
					LocalFree(rgAuthAttr);
					}
				if (rgUnauthAttr != IntPtr.Zero)
					{
					LocalFree(rgUnauthAttr);
					}
				}

			public uint cbSize;
			public IntPtr pCertInfo;
			public IntPtr hCryptProv;
			public uint dwKeySpec;
			public CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;
			public IntPtr pvHashAuxInfo;
			public uint cAuthAttr;
			public IntPtr rgAuthAttr;
			public uint cUnauthAttr;
			public IntPtr rgUnauthAttr;
			public CERT_ID SignerId;
			public CRYPT_ALGORITHM_IDENTIFIER HashEncryptionAlgorithm;
			public IntPtr pvHashEncryptionAuxInfo;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CMSG_SIGNER_INFO
			{
			public uint dwVersion;
			public CRYPTOAPI_BLOB Issuer;
			public CRYPTOAPI_BLOB SerialNumber;
			public CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;
			public CRYPT_ALGORITHM_IDENTIFIER HashEncryptionAlgorithm;
			public CRYPTOAPI_BLOB EncryptedHash;
			public CRYPT_ATTRIBUTES AuthAttrs;
			public CRYPT_ATTRIBUTES UnauthAttrs;
			}

		public delegate bool PFN_CMSG_STREAM_OUTPUT(IntPtr pvArg, IntPtr pbData, uint cbData, bool fFinal);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public class CMSG_STREAM_INFO
			{
			public CMSG_STREAM_INFO(uint cbContent, PFN_CMSG_STREAM_OUTPUT pfnStreamOutput, IntPtr pvArg)
				{
				this.cbContent = cbContent;
				this.pfnStreamOutput = pfnStreamOutput;
				this.pvArg = pvArg;
				}

			public uint cbContent;
			public PFN_CMSG_STREAM_OUTPUT pfnStreamOutput;
			public IntPtr pvArg;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CRYPT_ALGORITHM_IDENTIFIER
			{
			[MarshalAs(UnmanagedType.LPStr)]
			public string pszObjId;

			public CRYPTOAPI_BLOB Parameters;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CRYPT_ALGORITHM_IDENTIFIER2
			{
			public IntPtr pszObjId;
			public CRYPTOAPI_BLOB Parameters;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CRYPT_ATTRIBUTE
			{
			[MarshalAs(UnmanagedType.LPStr)]
			public string pszObjId;

			public uint cValue;
			public IntPtr rgValue;    // PCRYPT_ATTR_BLOB
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CRYPT_ATTRIBUTES
			{
			public uint cAttr;
			public IntPtr rgAttr;     // PCRYPT_ATTRIBUTE
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CRYPT_ATTRIBUTE_TYPE_VALUE
			{
			[MarshalAs(UnmanagedType.LPStr)]
			public string pszObjId;

			public CRYPTOAPI_BLOB Value;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CRYPT_BIT_BLOB
			{
			public uint cbData;
			public IntPtr pbData;
			public uint cUnusedBits;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CRYPT_KEY_PROV_INFO
			{
			public string pwszContainerName;
			public string pwszProvName;
			public uint dwProvType;
			public uint dwFlags;
			public uint cProvParam;
			public IntPtr rgProvParam;
			public uint dwKeySpec;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CRYPT_OID_INFO
			{
			public CRYPT_OID_INFO(int size)
				{
				cbSize = (uint) size;
				pszOID = null;
				pwszName = null;
				dwGroupId = 0;
				Algid = 0;
				ExtraInfo = new CRYPTOAPI_BLOB();
				}

			public uint cbSize;

			[MarshalAs(UnmanagedType.LPStr)]
			public string pszOID;

			public string pwszName;
			public uint dwGroupId;
			public uint Algid;
			public CRYPTOAPI_BLOB ExtraInfo;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CRYPT_RC2_CBC_PARAMETERS
			{
			public uint dwVersion;
			public bool fIV;            // set if has following IV

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public byte[] rgbIV;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CRYPTOAPI_BLOB
			{
			public uint cbData;
			public IntPtr pbData;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public class CRYPTUI_SELECTCERTIFICATE_STRUCTW
			{
			public uint dwSize;
			public IntPtr hwndParent;             // OPTIONAL
			public uint dwFlags;                // OPTIONAL
			public string szTitle;                // OPTIONAL
			public uint dwDontUseColumn;        // OPTIONAL
			public string szDisplayString;        // OPTIONAL
			public IntPtr pFilterCallback;        // OPTIONAL
			public IntPtr pDisplayCallback;       // OPTIONAL
			public IntPtr pvCallbackData;         // OPTIONAL
			public uint cDisplayStores;
			public IntPtr rghDisplayStores;
			public uint cStores;                // OPTIONAL
			public IntPtr rghStores;              // OPTIONAL
			public uint cPropSheetPages;        // OPTIONAL
			public IntPtr rgPropSheetPages;       // OPTIONAL
			public IntPtr hSelectedCertStore;     // OPTIONAL
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public class CRYPTUI_VIEWCERTIFICATE_STRUCTW
			{
			public uint dwSize;
			public IntPtr hwndParent;                         // OPTIONAL
			public uint dwFlags;                            // OPTIONAL
			public string szTitle;                            // OPTIONAL
			public IntPtr pCertContext;
			public IntPtr rgszPurposes;                       // OPTIONAL
			public uint cPurposes;                          // OPTIONAL
			public IntPtr pCryptProviderData;                 // OPTIONAL
			public bool fpCryptProviderDataTrustedUsage;    // OPTIONAL
			public uint idxSigner;                          // OPTIONAL
			public uint idxCert;                            // OPTIONAL
			public bool fCounterSigner;                     // OPTIONAL
			public uint idxCounterSigner;                   // OPTIONAL
			public uint cStores;                            // OPTIONAL
			public IntPtr rghStores;                          // OPTIONAL
			public uint cPropSheetPages;                    // OPTIONAL
			public IntPtr rgPropSheetPages;                   // OPTIONAL
			public uint nStartPage;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DSSPUBKEY
			{
			public uint magic;
			public uint bitlen;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct PROV_ENUMALGS_EX
			{
			public uint aiAlgid; // ALG_ID
			public uint dwDefaultLen;
			public uint dwMinLen;
			public uint dwMaxLen;
			public uint dwProtocols;
			public uint dwNameLen;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
			public byte[] szName;

			public uint dwLongNameLen;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
			public byte[] szLongName;
			}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct RSAPUBKEY
			{
			public uint magic;
			public uint bitlen;
			public uint pubexp;
			};

		///+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		///
		/// <summary>
		///
		/// Class   : CAPISafe.
		///
		/// Synopsis: CAPI wrapper class containing only static methods to wrap
		///           safe CAPI through P/Invoke.
		///
		///           All methods within this class will suppress unmanaged code
		///           permission and demand NO other permission, which means it is
		///           OK to be called by anyone.
		///
		///           !!! DANGER !!!!
		///
		///           Only in very specific situations you should place your method in
		///           this class. You MUST be absolutely sure that the method is
		///           safe to be called by anyone.
		///
		///           For example, CryptDecodeObject() would be fine to be in this
		///           class. On the other hand, CertOpenStore() definitely should
		///           NOT, especially when called with CERT_STORE_DELETE_FLAG.
		///
		/// </summary>
		///
		///--------------------------------------------------------------------------

		[SuppressUnmanagedCodeSecurityAttribute]
		[SecurityCritical]
		public static class CAPISafe
			{
			[DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			IntPtr GetProcAddress(
				[In] SafeLibraryHandle hModule,
				[In] [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

			[DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			SafeLocalAllocHandle LocalAlloc(
				[In] uint uFlags,
				[In] IntPtr sizetdwBytes);

			[DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false, EntryPoint = "LoadLibraryA")]
			[ResourceExposure(ResourceScope.Machine)]
			public static extern
			SafeLibraryHandle LoadLibrary(
				[In] [MarshalAs(UnmanagedType.LPStr)] string lpFileName);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			SafeCertContextHandle CertCreateCertificateContext(
				[In]     uint dwCertEncodingType,
				[In]     SafeLocalAllocHandle pbCertEncoded,
				[In]     uint cbCertEncoded);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			SafeCertContextHandle CertDuplicateCertificateContext(
				[In]     IntPtr pCertContext);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			bool CertFreeCertificateContext(
				[In]     IntPtr pCertContext);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CertGetCertificateChain(
				[In]     IntPtr hChainEngine,
				[In]     SafeCertContextHandle pCertContext,
				[In]     ref _FILETIME pTime,
				[In]     SafeCertStoreHandle hAdditionalStore,
				[In]     ref CERT_CHAIN_PARA pChainPara,
				[In]     uint dwFlags,
				[In]     IntPtr pvReserved,
				[In, Out] ref SafeCertChainHandle ppChainContext);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CertGetCertificateContextProperty(
				[In]     SafeCertContextHandle pCertContext,
				[In]     uint dwPropId,
				[In, Out] SafeLocalAllocHandle pvData,
				[In, Out] ref uint pcbData);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			uint CertGetPublicKeyLength(
				[In]    uint dwCertEncodingType,
				[In]    IntPtr pPublicKey); // PCERT_PUBLIC_KEY_INFO

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			uint CertNameToStrW(
				[In]     uint dwCertEncodingType,
				[In]     IntPtr pName,
				[In]     uint dwStrType,
				[In, Out] SafeLocalAllocHandle psz,
				[In]     uint csz);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CertVerifyCertificateChainPolicy(
				[In]     IntPtr pszPolicyOID,
				[In]     SafeCertChainHandle pChainContext,
				[In]     ref CERT_CHAIN_POLICY_PARA pPolicyPara,
				[In, Out] ref CERT_CHAIN_POLICY_STATUS pPolicyStatus);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CryptAcquireCertificatePrivateKey(
				[In]     SafeCertContextHandle pCert,
				[In]     uint dwFlags,
				[In]     IntPtr pvReserved,
				[In, Out] ref SafeCryptProvHandle phCryptProv,
				[In, Out] ref uint pdwKeySpec,
				[In, Out] ref bool pfCallerFreeProv);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			bool CryptDecodeObject(
				[In]     uint dwCertEncodingType,
				[In]     IntPtr lpszStructType,
				[In]     IntPtr pbEncoded,
				[In]     uint cbEncoded,
				[In]     uint dwFlags,
				[In, Out] SafeLocalAllocHandle pvStructInfo,
				[In, Out] IntPtr pcbStructInfo);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			bool CryptDecodeObject(
				[In]     uint dwCertEncodingType,
				[In]     IntPtr lpszStructType,
				[In]     byte[] pbEncoded,
				[In]     uint cbEncoded,
				[In]     uint dwFlags,
				[In, Out] SafeLocalAllocHandle pvStructInfo,
				[In, Out] IntPtr pcbStructInfo);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			bool CryptEncodeObject(
				[In]     uint dwCertEncodingType,
				[In]     IntPtr lpszStructType,
				[In]     IntPtr pvStructInfo,
				[In, Out] SafeLocalAllocHandle pbEncoded,
				[In, Out] IntPtr pcbEncoded);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			bool CryptEncodeObject(
				[In]     uint dwCertEncodingType,
				[In]     [MarshalAs(UnmanagedType.LPStr)] string lpszStructType,
				[In]     IntPtr pvStructInfo,
				[In, Out] SafeLocalAllocHandle pbEncoded,
				[In, Out] IntPtr pcbEncoded);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			IntPtr CryptFindOIDInfo(
				[In]     uint dwKeyType,
				[In]     IntPtr pvKey,
				[In]     uint dwGroupId);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			IntPtr CryptFindOIDInfo(
				[In]     uint dwKeyType,
				[In]     SafeLocalAllocHandle pvKey,
				[In]     uint dwGroupId);

			[DllImport(ADVAPI32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CryptGetProvParam(
				[In] SafeCryptProvHandle hProv,
				[In] uint dwParam,
				[In] IntPtr pbData,
				[In] IntPtr pdwDataLen,
				[In] uint dwFlags);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CryptMsgGetParam(
				[In]      SafeCryptMsgHandle hCryptMsg,
				[In]      uint dwParamType,
				[In]      uint dwIndex,
				[In, Out] IntPtr pvData,
				[In, Out] IntPtr pcbData);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CryptMsgGetParam(
				[In]      SafeCryptMsgHandle hCryptMsg,
				[In]      uint dwParamType,
				[In]      uint dwIndex,
				[In, Out] SafeLocalAllocHandle pvData,
				[In, Out] IntPtr pcbData);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			SafeCryptMsgHandle CryptMsgOpenToDecode(
				[In]      uint dwMsgEncodingType,
				[In]      uint dwFlags,
				[In]      uint dwMsgType,
				[In]      IntPtr hCryptProv,
				[In]      IntPtr pRecipientInfo,
				[In]      IntPtr pStreamInfo);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CryptMsgUpdate(
				[In]      SafeCryptMsgHandle hCryptMsg,
				[In]      byte[] pbData,
				[In]      uint cbData,
				[In]      bool fFinal);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CryptMsgUpdate(
				[In]      SafeCryptMsgHandle hCryptMsg,
				[In]      IntPtr pbData,
				[In]      uint cbData,
				[In]      bool fFinal);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CryptMsgVerifyCountersignatureEncoded(
				[In]      IntPtr hCryptProv,
				[In]      uint dwEncodingType,
				[In]      IntPtr pbSignerInfo,
				[In]      uint cbSignerInfo,
				[In]      IntPtr pbSignerInfoCountersignature,
				[In]      uint cbSignerInfoCountersignature,
				[In]      IntPtr pciCountersigner);

			[DllImport(KERNEL32, SetLastError = true)]
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			IntPtr LocalFree(IntPtr handle);

			[DllImport(KERNEL32, SetLastError = true)]
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			void ZeroMemory(IntPtr handle, uint length);

			[DllImport(ADVAPI32, SetLastError = true)]
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			int LsaNtStatusToWinError(
				[In]    int status);
			}

		///+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		///
		/// <summary>
		///
		/// Class   : CAPIUnsafe.
		///
		/// Synopsis: CAPI wrapper class containing only static methods to wrap
		///           unsafe CAPI through P/Invoke.
		///
		///           All methods within this class will suppress unmanaged code
		///           permission but should in turn demand for other appropriate
		///           permission(s).
		///
		///           !!! WARNING !!!!
		///
		///           You should first consider whether the method should go into
		///           the Native class above.
		///
		///           If you are convinced that the method does belong in this
		///           class, you MUST:
		///
		///           (1) declare the method as "protected public" here.
		///
		///           (2) override the method in CAPI class below.
		///
		///           (3) demand appropriate permission(s) in your override.
		///
		///           (4) properly validate all parameters.
		///
		///           For example, if you decide to put any method that will create
		///           a file on disk, such as CertSaveStore() with the
		///           CERT_STORE_SAVE_TO_FILENAME flag, you then better demand
		///           FileIOPermission for Write, and make sure to also validate
		///           the filename (think about what will happen if the filename is
		///           C:\boot.ini?).
		///
		/// </summary>
		///
		///-------------------------------------------------------------------------

		[SuppressUnmanagedCodeSecurityAttribute]
		[SecurityCritical]
		public static class CAPIUnsafe
			{
			[DllImport(ADVAPI32, SetLastError = true)]
			[ResourceExposure(ResourceScope.Machine)]
			public extern static
			bool CryptReleaseContext(
				[In] ref SafeCryptProvHandle hCryptProv,
				[In]     uint dwFlags);

			[DllImport(ADVAPI32, SetLastError = true)]
			[ResourceExposure(ResourceScope.Machine)]
			public extern static
			bool CryptContextAddRef(
				[In] ref SafeCryptProvHandle hCryptProv,
				[In, Out] ref uint pdwReserved,
				[In]     uint dwFlags);

			[DllImport(ADVAPI32, SetLastError = true)]
			[ResourceExposure(ResourceScope.Machine)]
			public extern static
			bool CryptCreateHash(
			  [In] SafeCryptProvHandle hCryptProv,
			  [In]  uint Algid,
			  [In] SafeCryptKeyHandle hKey,
			  [In]  uint dwFlags,
			  [In, Out] ref SafeCryptHashHandle phHash
			);

			[DllImport(ADVAPI32, SetLastError = true)]
			[ResourceExposure(ResourceScope.Machine)]
			public extern static
			bool CryptDestroyHash(
				[In] SafeCryptHashHandle phHash);

			[DllImport(ADVAPI32, SetLastError = true)]
			public static extern bool CryptHashData(
				[In] SafeCryptHashHandle hHash,
				[In] byte[] pbData,
				[In] uint dataLen,
				[In] uint dwFlags);

			[DllImport(ADVAPI32, CharSet = CharSet.Auto, SetLastError = true, EntryPoint = "CryptSignHashA")]
			public static extern bool CryptSignHash(
				[In]      SafeCryptHashHandle hHash,
				[In]      uint dwKeySpec,
				[In]      IntPtr sDescription,
				[In]      uint dwFlags,
				[Out]     byte[] pbSignature,
				[In, Out] ref uint pdwSigLen);

			[DllImport(ADVAPI32, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern bool CryptGetHashParam(
				[In] SafeCryptHashHandle hHash,
				[In] HashParameters dwParam,
				[Out] byte[] pbData,
				[In, Out] ref uint pdwDataLen,
				uint dwFlags
				);

			[DllImport(ADVAPI32, SetLastError = true)]
			public static extern bool CryptSetHashParam(
				[In]      SafeCryptHashHandle hHash,
				[In]      HashParameters dwParam,
				[In]      byte[] pbData,
				[In]      uint dwFlags
				);

			[DllImport(ADVAPI32, SetLastError = true)]
			public static extern bool CryptGetUserKey(
			[In]     SafeCryptProvHandle hCryptProv,
			[In]     uint dwProvType,
			[In, Out] ref SafeCryptKeyHandle KeyHandle
				);

			[DllImport(ADVAPI32, SetLastError = true)]
			public static extern bool CryptVerifySignature(
				[In] SafeCryptHashHandle hHash,
				[In]      byte[] pbSignature,
				[In]      uint dwSigLen,
				[In]      SafeCryptKeyHandle hPubKey,
				[In]      IntPtr sDescription,
				[In]      uint dwFlags
				);

			[DllImport(ADVAPI32, CharSet = CharSet.Auto, BestFitMapping = false, EntryPoint = "CryptAcquireContextA")]
			[ResourceExposure(ResourceScope.Machine)]
			public extern static
			bool CryptAcquireContext(
				[In, Out] ref SafeCryptProvHandle hCryptProv,
				[In]     [MarshalAs(UnmanagedType.LPStr)] string pszContainer,
				[In]     [MarshalAs(UnmanagedType.LPStr)] string pszProvider,
				[In]     uint dwProvType,
				[In]     uint dwFlags);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CertAddCertificateContextToStore(
				[In]     SafeCertStoreHandle hCertStore,
				[In]     SafeCertContextHandle pCertContext,
				[In]     uint dwAddDisposition,
				[In, Out] SafeCertContextHandle ppStoreContext);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CertAddCertificateLinkToStore(
				[In]     SafeCertStoreHandle hCertStore,
				[In]     SafeCertContextHandle pCertContext,
				[In]     uint dwAddDisposition,
				[In, Out] SafeCertContextHandle ppStoreContext);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			IntPtr CertEnumCertificatesInStore(
				[In]     SafeCertStoreHandle hCertStore,
				[In]     IntPtr pPrevCertContext);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			SafeCertContextHandle CertFindCertificateInStore(
				[In]     SafeCertStoreHandle hCertStore,
				[In]     uint dwCertEncodingType,
				[In]     uint dwFindFlags,
				[In]     uint dwFindType,
				[In]     IntPtr pvFindPara,
				[In]     SafeCertContextHandle pPrevCertContext);

			[DllImport(CRYPT32, CharSet = CharSet.Unicode, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			SafeCertStoreHandle CertOpenStore(
				[In]     IntPtr lpszStoreProvider,
				[In]     uint dwMsgAndCertEncodingType,
				[In]     IntPtr hCryptProv,
				[In]     uint dwFlags,
				[In]     string pvPara); // we want this always as a Unicode string.

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			SafeCertContextHandle CertCreateSelfSignCertificate(
				[In]     SafeCryptProvHandle hProv,
				[In]     IntPtr pSubjectIssuerBlob,
				[In]     uint dwFlags,
				[In]     IntPtr pKeyProvInfo,
				[In]     IntPtr pSignatureAlgorithm,
				[In]     IntPtr pStartTime,
				[In]     IntPtr pEndTime,
				[In]     IntPtr pExtensions);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CryptMsgControl(
				[In]      SafeCryptMsgHandle hCryptMsg,
				[In]      uint dwFlags,
				[In]      uint dwCtrlType,
				[In]      IntPtr pvCtrlPara);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			bool CryptMsgCountersign(
				[In]      SafeCryptMsgHandle hCryptMsg,
				[In]      uint dwIndex,
				[In]      uint cCountersigners,
				[In]      IntPtr rgCountersigners);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			SafeCryptMsgHandle CryptMsgOpenToEncode(
				[In]      uint dwMsgEncodingType,
				[In]      uint dwFlags,
				[In]      uint dwMsgType,
				[In]      IntPtr pvMsgEncodeInfo,
				[In]      IntPtr pszInnerContentObjID,
				[In]      IntPtr pStreamInfo);

			[DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
			[ResourceExposure(ResourceScope.None)]
			public extern static
			SafeCryptMsgHandle CryptMsgOpenToEncode(
				[In]      uint dwMsgEncodingType,
				[In]      uint dwFlags,
				[In]      uint dwMsgType,
				[In]      IntPtr pvMsgEncodeInfo,
				[In]      [MarshalAs(UnmanagedType.LPStr)] string pszInnerContentObjID,
				[In]      IntPtr pStreamInfo);

			[DllImport(CRYPT32, CharSet = CharSet.Unicode, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			bool CryptProtectData(
				[In]     IntPtr pDataIn,
				[In]     string szDataDescr,
				[In]     IntPtr pOptionalEntropy,
				[In]     IntPtr pvReserved,
				[In]     IntPtr pPromptStruct,
				[In]     uint dwFlags,
				[In, Out] IntPtr pDataBlob);

			[DllImport(CRYPT32, CharSet = CharSet.Unicode, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			bool CryptUnprotectData(
				[In]     IntPtr pDataIn,
				[In]     IntPtr ppszDataDescr,
				[In]     IntPtr pOptionalEntropy,
				[In]     IntPtr pvReserved,
				[In]     IntPtr pPromptStruct,
				[In]     uint dwFlags,
				[In, Out] IntPtr pDataBlob);

			// RtlEncryptMemory and RtlDecryptMemory are declared in the public header file crypt.h.
			// They were also recently declared in the public header file ntsecapi.h (in the Platform
			// SDK as well as the current build of Server 2003). We use them instead of
			// CryptProtectMemory and CryptUnprotectMemory because they are available in both WinXP
			// and in Windows Server 2003.

			[DllImport(ADVAPI32, CharSet = CharSet.Unicode, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			int SystemFunction040(
				[In, Out] byte[] pDataIn,
				[In]     uint cbDataIn,   // multiple of RTL_ENCRYPT_MEMORY_SIZE
				[In]     uint dwFlags);

			[DllImport(ADVAPI32, CharSet = CharSet.Unicode, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			int SystemFunction041(
				[In, Out] byte[] pDataIn,
				[In]     uint cbDataIn,   // multiple of RTL_ENCRYPT_MEMORY_SIZE
				[In]     uint dwFlags);

			[DllImport(CRYPTUI, CharSet = CharSet.Unicode, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			SafeCertContextHandle CryptUIDlgSelectCertificateW(
				[In, Out, MarshalAs(UnmanagedType.LPStruct)] CRYPTUI_SELECTCERTIFICATE_STRUCTW csc);

			[DllImport(CRYPTUI, CharSet = CharSet.Unicode, SetLastError = true)]
			[ResourceExposure(ResourceScope.None)]
			public static extern
			bool CryptUIDlgViewCertificateW(
				[In, MarshalAs(UnmanagedType.LPStruct)] CRYPTUI_VIEWCERTIFICATE_STRUCTW ViewInfo,
				[In, Out] IntPtr pfPropertiesChanged);
			}

		[SecurityCritical]
		public static byte[] BlobToByteArray(IntPtr pBlob)
			{
			CAPI.CRYPTOAPI_BLOB blob = (CAPI.CRYPTOAPI_BLOB) Marshal.PtrToStructure(pBlob, typeof(CAPI.CRYPTOAPI_BLOB));
			if (blob.cbData == 0)
				return new byte[0];
			return BlobToByteArray(blob);
			}

		[SecurityCritical]
		public static byte[] BlobToByteArray(CAPI.CRYPTOAPI_BLOB blob)
			{
			if (blob.cbData == 0)
				return new byte[0];
			byte[] data = new byte[blob.cbData];
			Marshal.Copy(blob.pbData, data, 0, data.Length);
			return data;
			}

		//[SecurityCritical]
		//public static unsafe bool DecodeObject(IntPtr pszStructType,
		//				  IntPtr pbEncoded,
		//				  uint cbEncoded,
		//				  out SafeLocalAllocHandle decodedValue,
		//				  out uint cbDecodedValue)
		//	{
		//	// Initialize out parameters
		//	decodedValue = SafeLocalAllocHandle.InvalidHandle;
		//	cbDecodedValue = 0;

		// // Decode uint cbDecoded = 0; SafeLocalAllocHandle ptr =
		// SafeLocalAllocHandle.InvalidHandle; bool result =
		// CAPISafe.CryptDecodeObject(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, pszStructType,
		// pbEncoded, cbEncoded, 0, ptr, new IntPtr(&cbDecoded)); if (result == false) return false;

		// ptr = CAPI.LocalAlloc(CAPI.LMEM_FIXED, new IntPtr(cbDecoded)); result =
		// CAPISafe.CryptDecodeObject(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, pszStructType,
		// pbEncoded, cbEncoded, 0, ptr, new IntPtr(&cbDecoded)); if (result == false) return false;

		// // Return decoded values decodedValue = ptr; cbDecodedValue = cbDecoded;

		// return true; }

		//[SecurityCritical]
		//public static unsafe bool DecodeObject(IntPtr pszStructType,
		//				  byte[] pbEncoded,
		//				  out SafeLocalAllocHandle decodedValue,
		//				  out uint cbDecodedValue)
		//	{
		//	// Initialize out parameters
		//	decodedValue = SafeLocalAllocHandle.InvalidHandle;
		//	cbDecodedValue = 0;

		// // Decode uint cbDecoded = 0; SafeLocalAllocHandle pbDecoded = SafeLocalAllocHandle.InvalidHandle;

		// if (!CAPISafe.CryptDecodeObject(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, pszStructType,
		// pbEncoded, (uint) pbEncoded.Length, 0, pbDecoded, new IntPtr(&cbDecoded))) return false;

		// pbDecoded = CAPI.LocalAlloc(CAPI.LMEM_FIXED, new IntPtr(cbDecoded)); if
		// (!CAPISafe.CryptDecodeObject(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, pszStructType,
		// pbEncoded, (uint) pbEncoded.Length, 0, pbDecoded, new IntPtr(&cbDecoded))) return false;

		// // Return decoded values decodedValue = pbDecoded; cbDecodedValue = cbDecoded;

		// return true; }

		//[SecuritySafeCritical]
		//public static unsafe bool EncodeObject(IntPtr lpszStructType,
		//				  IntPtr pvStructInfo,
		//				  out byte[] encodedData)
		//	{
		//	// Initialize out parameter
		//	encodedData = new byte[0];

		// // Encode uint cbEncoded = 0; SafeLocalAllocHandle pbEncoded = SafeLocalAllocHandle.InvalidHandle;

		// if (!CAPISafe.CryptEncodeObject(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, lpszStructType,
		// pvStructInfo, pbEncoded, new IntPtr(&cbEncoded))) return false;

		// pbEncoded = CAPI.LocalAlloc(CAPI.LMEM_FIXED, new IntPtr(cbEncoded)); if
		// (!CAPISafe.CryptEncodeObject(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, lpszStructType,
		// pvStructInfo, pbEncoded, new IntPtr(&cbEncoded))) return false;

		// // Return encoded data encodedData = new byte[cbEncoded];
		// Marshal.Copy(pbEncoded.DangerousGetHandle(), encodedData, 0, (int) cbEncoded);

		// pbEncoded.Dispose(); return true; }

		//[SecurityCritical]
		//public static unsafe bool EncodeObject(string lpszStructType,
		//				  IntPtr pvStructInfo,
		//				  out byte[] encodedData)
		//	{
		//	// Initialize out parameter
		//	encodedData = new byte[0];

		// // Encode uint cbEncoded = 0; SafeLocalAllocHandle pbEncoded = SafeLocalAllocHandle.InvalidHandle;

		// if (!CAPISafe.CryptEncodeObject(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, lpszStructType,
		// pvStructInfo, pbEncoded, new IntPtr(&cbEncoded))) return false;

		// pbEncoded = CAPI.LocalAlloc(CAPI.LMEM_FIXED, new IntPtr(cbEncoded)); if
		// (!CAPISafe.CryptEncodeObject(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, lpszStructType,
		// pvStructInfo, pbEncoded, new IntPtr(&cbEncoded))) return false;

		// // Return encoded data encodedData = new byte[cbEncoded];
		// Marshal.Copy(pbEncoded.DangerousGetHandle(), encodedData, 0, (int) cbEncoded);

		// pbEncoded.Dispose(); return true; }

		// Determine if an error code may have been caused by trying to do a crypto operation while
		// the current user's profile is not yet loaded.
		public static bool ErrorMayBeCausedByUnloadedProfile(int errorCode)
			{
			// CAPI returns a file not found error if the user profile is not yet loaded
			return errorCode == E_FILENOTFOUND ||
				   errorCode == ERROR_FILE_NOT_FOUND;
			}

		[SecurityCritical]
		public static SafeLocalAllocHandle LocalAlloc(uint uFlags, IntPtr sizetdwBytes)
			{
			SafeLocalAllocHandle safeLocalAllocHandle = CAPISafe.LocalAlloc(uFlags, sizetdwBytes);
			if (safeLocalAllocHandle == null || safeLocalAllocHandle.IsInvalid)
				throw new OutOfMemoryException();

			return safeLocalAllocHandle;
			}

		[ResourceExposure(ResourceScope.Machine)]
		[ResourceConsumption(ResourceScope.Machine)]
		[SecurityCritical]
		public static bool CryptAcquireContext(
			[In, Out] ref SafeCryptProvHandle hCryptProv,
			[In]     [MarshalAs(UnmanagedType.LPStr)] string pwszContainer,
			[In]     [MarshalAs(UnmanagedType.LPStr)] string pwszProvider,
			[In]     uint dwProvType,
			[In]     uint dwFlags)
			{
			CspParameters parameters = new CspParameters();
			parameters.ProviderName = pwszProvider;
			parameters.KeyContainerName = pwszContainer;
			parameters.ProviderType = (int) dwProvType;
			parameters.KeyNumber = -1;
			parameters.Flags = (CspProviderFlags) ((dwFlags & CAPI.CRYPT_MACHINE_KEYSET) == CAPI.CRYPT_MACHINE_KEYSET ? CspProviderFlags.UseMachineKeyStore : 0);

			KeyContainerPermission kp = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
			KeyContainerPermissionAccessEntry entry = new KeyContainerPermissionAccessEntry(parameters, KeyContainerPermissionFlags.Open);
			kp.AccessEntries.Add(entry);
			kp.Demand();

			bool rc = CAPIUnsafe.CryptAcquireContext(ref hCryptProv,
													 pwszContainer,
													 pwszProvider,
													 dwProvType,
													 dwFlags);

			if (!rc && Marshal.GetLastWin32Error() == CAPI.NTE_BAD_KEYSET)
				{
				rc = CAPIUnsafe.CryptAcquireContext(ref hCryptProv,
													pwszContainer,
													pwszProvider,
													dwProvType,
													dwFlags | CAPI.CRYPT_NEWKEYSET);
				}

			return rc;
			}

		[ResourceExposure(ResourceScope.Machine)]
		[ResourceConsumption(ResourceScope.Machine)]
		[SecurityCritical]
		public static bool CryptAcquireContext(
			ref SafeCryptProvHandle hCryptProv,
			IntPtr pwszContainer,
			IntPtr pwszProvider,
			uint dwProvType,
			uint dwFlags)
			{
			string container = null;
			if (pwszContainer != IntPtr.Zero)
				{
				container = Marshal.PtrToStringUni(pwszContainer);
				}

			string provider = null;
			if (pwszProvider != IntPtr.Zero)
				{
				provider = Marshal.PtrToStringUni(pwszProvider);
				}

			return CryptAcquireContext(ref hCryptProv,
									   container,
									   provider,
									   dwProvType,
									   dwFlags);
			}

		[SecurityCritical]
		public static CRYPT_OID_INFO CryptFindOIDInfo(
			[In]    uint dwKeyType,
			[In]    IntPtr pvKey,
			[In]    uint dwGroupId)
			{
			if (pvKey == IntPtr.Zero)
				throw new ArgumentNullException("pvKey");

			CRYPT_OID_INFO pOIDInfo = new CRYPT_OID_INFO(Marshal.SizeOf(typeof(CRYPT_OID_INFO)));
			IntPtr pv = CAPISafe.CryptFindOIDInfo(dwKeyType,
												  pvKey,
												  dwGroupId);

			if (pv != IntPtr.Zero)
				pOIDInfo = (CRYPT_OID_INFO) Marshal.PtrToStructure(pv, typeof(CAPI.CRYPT_OID_INFO));

			return pOIDInfo;
			}

		[SecurityCritical]
		public static CRYPT_OID_INFO CryptFindOIDInfo(
			[In]    uint dwKeyType,
			[In]    SafeLocalAllocHandle pvKey,
			[In]    uint dwGroupId)
			{
			if (pvKey == null)
				throw new ArgumentNullException("pvKey");
			if (pvKey.IsInvalid)
				throw new CryptographicException("Cryptography_InvalidHandle - pvKey");

			CRYPT_OID_INFO pOIDInfo = new CRYPT_OID_INFO(Marshal.SizeOf(typeof(CRYPT_OID_INFO)));
			IntPtr pv = CAPISafe.CryptFindOIDInfo(dwKeyType,
												  pvKey,
												  dwGroupId);

			if (pv != IntPtr.Zero)
				pOIDInfo = (CRYPT_OID_INFO) Marshal.PtrToStructure(pv, typeof(CAPI.CRYPT_OID_INFO));

			return pOIDInfo;
			}

		[SecurityCritical]
		public static bool CryptMsgControl(
			[In]      SafeCryptMsgHandle hCryptMsg,
			[In]      uint dwFlags,
			[In]      uint dwCtrlType,
			[In]      IntPtr pvCtrlPara)
			{
			// We are supposed to demand Open and Sign permissions for the key container here for the
			// CMSG_CTRL_DECRYPT class of parameters. For now, the caller should demand, as to do the
			// demand here will be more complicated and expensive since the CSP parameters are not
			// readily available here.

			return CAPIUnsafe.CryptMsgControl(hCryptMsg,
											  dwFlags,
											  dwCtrlType,
											  pvCtrlPara);
			}

		[SecurityCritical]
		public static bool CryptMsgCountersign(
			[In]      SafeCryptMsgHandle hCryptMsg,
			[In]      uint dwIndex,
			[In]      uint cCountersigners,
			[In]      IntPtr rgCountersigners)
			{
			// We are supposed to demand Open and Sign permissions for the key container here. For
			// now, the caller should demand, as to do the demand here will be more complicated since
			// the CSP parameters are not readily available here.

			return CAPIUnsafe.CryptMsgCountersign(hCryptMsg,
												  dwIndex,
												  cCountersigners,
												  rgCountersigners);
			}

		[SecurityCritical]
		public static SafeCryptMsgHandle CryptMsgOpenToEncode(
			[In]      uint dwMsgEncodingType,
			[In]      uint dwFlags,
			[In]      uint dwMsgType,
			[In]      IntPtr pvMsgEncodeInfo,
			[In]      IntPtr pszInnerContentObjID,
			[In]      IntPtr pStreamInfo)
			{
			// We are supposed to demand Open and Sign permissions for the key container here. For
			// now, the caller should demand, as to do the demand here will be more complicated since
			// the CSP parameters are not readily available here.

			return CAPIUnsafe.CryptMsgOpenToEncode(dwMsgEncodingType,
												   dwFlags,
												   dwMsgType,
												   pvMsgEncodeInfo,
												   pszInnerContentObjID,
												   pStreamInfo);
			}

		[SecurityCritical]
		public static SafeCryptMsgHandle CryptMsgOpenToEncode(
			[In]      uint dwMsgEncodingType,
			[In]      uint dwFlags,
			[In]      uint dwMsgType,
			[In]      IntPtr pvMsgEncodeInfo,
			[In]      string pszInnerContentObjID,
			[In]      IntPtr pStreamInfo)
			{
			// We are supposed to demand Open and Sign permissions for the key container here. For
			// now, the caller should demand, as to do the demand here will be more complicated and
			// expensive since the CSP parameters are not readily available here.

			return CAPIUnsafe.CryptMsgOpenToEncode(dwMsgEncodingType,
												   dwFlags,
												   dwMsgType,
												   pvMsgEncodeInfo,
												   pszInnerContentObjID,
												   pStreamInfo);
			}

		[SecurityCritical]
		public static SafeCertContextHandle CertDuplicateCertificateContext(
			[In]     IntPtr pCertContext)
			{
			if (pCertContext == IntPtr.Zero)
				return SafeCertContextHandle.InvalidHandle;

			return CAPISafe.CertDuplicateCertificateContext(pCertContext);
			}

		[SecurityCritical]
		public static IntPtr CertEnumCertificatesInStore(
			[In]     SafeCertStoreHandle hCertStore,
			[In]     IntPtr pPrevCertContext)
			{
			if (hCertStore == null)
				throw new ArgumentNullException("hCertStore");
			if (hCertStore.IsInvalid)
				throw new CryptographicException("Cryptography_InvalidHandle - hCertStore");

			if (pPrevCertContext == IntPtr.Zero)
				{
				StorePermission sp = new StorePermission(StorePermissionFlags.EnumerateCertificates);
				sp.Demand();
				}

			IntPtr handle = CAPIUnsafe.CertEnumCertificatesInStore(hCertStore, pPrevCertContext);
			if (handle == IntPtr.Zero)
				{
				int dwErrorCode = Marshal.GetLastWin32Error();
				if (dwErrorCode != CRYPT_E_NOT_FOUND)
					{
					CAPISafe.CertFreeCertificateContext(handle);
					throw new CryptographicException(dwErrorCode);
					}
				}
			return handle;
			}

		// This override demands StorePermission.AddToStore.

		[SecurityCritical]
		public static bool CertAddCertificateContextToStore(
			[In]     SafeCertStoreHandle hCertStore,
			[In]     SafeCertContextHandle pCertContext,
			[In]     uint dwAddDisposition,
			[In, Out] SafeCertContextHandle ppStoreContext)
			{
			if (hCertStore == null)
				throw new ArgumentNullException("hCertStore");
			if (hCertStore.IsInvalid)
				throw new CryptographicException("Cryptography_InvalidHandle - hCertStore");

			if (pCertContext == null)
				throw new ArgumentNullException("pCertContext");
			if (pCertContext.IsInvalid)
				throw new CryptographicException("Cryptography_InvalidHandle - pCertContext");

			StorePermission sp = new StorePermission(StorePermissionFlags.AddToStore);
			sp.Demand();

			return CAPIUnsafe.CertAddCertificateContextToStore(hCertStore,
															   pCertContext,
															   dwAddDisposition,
															   ppStoreContext);
			}

		// This override demands StorePermission.AddToStore.
		[SecurityCritical]
		public static bool CertAddCertificateLinkToStore(
			[In]     SafeCertStoreHandle hCertStore,
			[In]     SafeCertContextHandle pCertContext,
			[In]     uint dwAddDisposition,
			[In, Out] SafeCertContextHandle ppStoreContext)
			{
			if (hCertStore == null)
				throw new ArgumentNullException("hCertStore");
			if (hCertStore.IsInvalid)
				throw new CryptographicException("Cryptography_InvalidHandle - hCertStore");

			if (pCertContext == null)
				throw new ArgumentNullException("pCertContext");
			if (pCertContext.IsInvalid)
				throw new CryptographicException("Cryptography_InvalidHandle - pCertContext");

			StorePermission sp = new StorePermission(StorePermissionFlags.AddToStore);
			sp.Demand();

			return CAPIUnsafe.CertAddCertificateLinkToStore(hCertStore,
															pCertContext,
															dwAddDisposition,
															ppStoreContext);
			}

		// This override demands StorePermission.OpenStore.

		[SecurityCritical]
		public static SafeCertStoreHandle CertOpenStore(
			[In]    IntPtr lpszStoreProvider,
			[In]    uint dwMsgAndCertEncodingType,
			[In]    IntPtr hCryptProv,
			[In]    uint dwFlags,
			[In]    string pvPara)
			{
			if (lpszStoreProvider != new IntPtr(CERT_STORE_PROV_MEMORY) && lpszStoreProvider != new IntPtr(CERT_STORE_PROV_SYSTEM))
				throw new ArgumentException("Argument_InvalidValue - lpszStoreProvider");

			if ((dwFlags & CERT_SYSTEM_STORE_LOCAL_MACHINE) == CERT_SYSTEM_STORE_LOCAL_MACHINE ||
				(dwFlags & CERT_SYSTEM_STORE_LOCAL_MACHINE_GROUP_POLICY) == CERT_SYSTEM_STORE_LOCAL_MACHINE_GROUP_POLICY ||
				(dwFlags & CERT_SYSTEM_STORE_LOCAL_MACHINE_ENTERPRISE) == CERT_SYSTEM_STORE_LOCAL_MACHINE_ENTERPRISE)
				{
				// We do not allow opening remote local machine stores if in semi-trusted environment.
				if (pvPara != null && pvPara.StartsWith(@"\\", StringComparison.Ordinal))
					new PermissionSet(PermissionState.Unrestricted).Demand();
				}

			if ((dwFlags & CERT_STORE_DELETE_FLAG) == CERT_STORE_DELETE_FLAG)
				{
				StorePermission sp = new StorePermission(StorePermissionFlags.DeleteStore);
				sp.Demand();
				}
			else
				{
				StorePermission sp = new StorePermission(StorePermissionFlags.OpenStore);
				sp.Demand();
				}

			if ((dwFlags & CERT_STORE_CREATE_NEW_FLAG) == CERT_STORE_CREATE_NEW_FLAG)
				{
				StorePermission sp = new StorePermission(StorePermissionFlags.CreateStore);
				sp.Demand();
				}

			if ((dwFlags & CERT_STORE_OPEN_EXISTING_FLAG) == 0)
				{
				StorePermission sp = new StorePermission(StorePermissionFlags.CreateStore);
				sp.Demand();
				}

			return CAPIUnsafe.CertOpenStore(lpszStoreProvider,
											dwMsgAndCertEncodingType,
											hCryptProv,
											dwFlags | CERT_STORE_DEFER_CLOSE_UNTIL_LAST_FREE_FLAG,
											pvPara);
			}

		[SecurityCritical]
		public static bool CryptProtectData(
			[In]     IntPtr pDataIn,
			[In]     string szDataDescr,
			[In]     IntPtr pOptionalEntropy,
			[In]     IntPtr pvReserved,
			[In]     IntPtr pPromptStruct,
			[In]     uint dwFlags,
			[In, Out] IntPtr pDataBlob)
			{
			// Semi-trusted callers are not allowed to use DPAPI since it uses the user/machine credentials.
			DataProtectionPermission dp = new DataProtectionPermission(DataProtectionPermissionFlags.ProtectData);
			dp.Demand();

			return CAPIUnsafe.CryptProtectData(pDataIn, szDataDescr, pOptionalEntropy, pvReserved, pPromptStruct, dwFlags, pDataBlob);
			}

		[SecurityCritical]
		public static bool CryptUnprotectData(
			[In]     IntPtr pDataIn,
			[In]     IntPtr ppszDataDescr,
			[In]     IntPtr pOptionalEntropy,
			[In]     IntPtr pvReserved,
			[In]     IntPtr pPromptStruct,
			[In]     uint dwFlags,
			[In, Out] IntPtr pDataBlob)
			{
			// Semi-trusted callers are not allowed to use DPAPI since it uses the user/machine credentials.
			DataProtectionPermission dp = new DataProtectionPermission(DataProtectionPermissionFlags.UnprotectData);
			dp.Demand();

			return CAPIUnsafe.CryptUnprotectData(pDataIn, ppszDataDescr, pOptionalEntropy, pvReserved, pPromptStruct, dwFlags, pDataBlob);
			}

		[SecurityCritical]
		public static int SystemFunction040(
			[In, Out] byte[] pDataIn,
			[In]     uint cbDataIn,   // multiple of RTL_ENCRYPT_MEMORY_SIZE
			[In]     uint dwFlags)
			{
			// Semi-trusted callers are not allowed to use DPAPI since it uses the user/machine credentials.
			DataProtectionPermission dp = new DataProtectionPermission(DataProtectionPermissionFlags.ProtectMemory);
			dp.Demand();

			return CAPIUnsafe.SystemFunction040(pDataIn, cbDataIn, dwFlags);
			}

		[SecurityCritical]
		public static int SystemFunction041(
			[In, Out] byte[] pDataIn,
			[In]     uint cbDataIn,   // multiple of RTL_ENCRYPT_MEMORY_SIZE
			[In]     uint dwFlags)
			{
			// Semi-trusted callers are not allowed to use DPAPI since it uses the user/machine credentials.
			DataProtectionPermission dp = new DataProtectionPermission(DataProtectionPermissionFlags.UnprotectMemory);
			dp.Demand();

			return CAPIUnsafe.SystemFunction041(pDataIn, cbDataIn, dwFlags);
			}

		[SecurityCritical]
		public static SafeCertContextHandle CryptUIDlgSelectCertificateW(
			[In, Out, MarshalAs(UnmanagedType.LPStruct)]
			CRYPTUI_SELECTCERTIFICATE_STRUCTW csc)
			{
			// UI only allowed in interactive session.
			if (!System.Environment.UserInteractive)
				throw new InvalidOperationException("Environment_NotInteractive");

			// we need to demand UI permission here.
			UIPermission uiPermission = new UIPermission(UIPermissionWindow.SafeTopLevelWindows);
			uiPermission.Demand();

			return CAPIUnsafe.CryptUIDlgSelectCertificateW(csc);
			}

		[SecurityCritical]
		public static bool CryptUIDlgViewCertificateW(
			[In, MarshalAs(UnmanagedType.LPStruct)] CRYPTUI_VIEWCERTIFICATE_STRUCTW ViewInfo,
			[In, Out] IntPtr pfPropertiesChanged)
			{
			// UI only allowed in interactive session.
			if (!System.Environment.UserInteractive)
				throw new InvalidOperationException("Environment_NotInteractive");

			// we need to demand UI permission here.
			UIPermission uiPermission = new UIPermission(UIPermissionWindow.SafeTopLevelWindows);
			uiPermission.Demand();

			return CAPIUnsafe.CryptUIDlgViewCertificateW(ViewInfo, pfPropertiesChanged);
			}

		[SecurityCritical]
		public static SafeCertContextHandle CertFindCertificateInStore(
			[In]    SafeCertStoreHandle hCertStore,
			[In]    uint dwCertEncodingType,
			[In]    uint dwFindFlags,
			[In]    uint dwFindType,
			[In]    IntPtr pvFindPara,
			[In]    SafeCertContextHandle pPrevCertContext)
			{
			if (hCertStore == null)
				throw new ArgumentNullException("hCertStore");
			if (hCertStore.IsInvalid)
				throw new CryptographicException("Cryptography_InvalidHandle - hCertStore");

			// Note we do not demand EnumerateCertificates flag of StorePermission since we only call
			// this method for a memory store and we don't want to incur the cost of a demand.
			// Ideally, we should be doing that.

			return CAPIUnsafe.CertFindCertificateInStore(hCertStore,
														 dwCertEncodingType,
														 dwFindFlags,
														 dwFindType,
														 pvFindPara,
														 pPrevCertContext);
			}
		}

	[SecurityCritical]
	public sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
		private SafeLibraryHandle() : base(true)
			{
			}

		[DllImport(CAPI.KERNEL32, SetLastError = true),
		 SuppressUnmanagedCodeSecurity,
		 ResourceExposure(ResourceScope.None),
		 ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FreeLibrary([In] IntPtr hModule);

		[SecurityCritical]
		protected override bool ReleaseHandle()
			{
			return FreeLibrary(handle);
			}
		}

	[SecurityCritical]
	public sealed class SafeLocalAllocHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
		private SafeLocalAllocHandle() : base(true)
			{
			}

		// 0 is an Invalid Handle
		public SafeLocalAllocHandle(IntPtr handle) : base(true)
			{
			SetHandle(handle);
			}

		public static SafeLocalAllocHandle InvalidHandle
			{
			get
				{
				return new SafeLocalAllocHandle(IntPtr.Zero);
				}
			}

		[DllImport(CAPI.KERNEL32, SetLastError = true),
		 SuppressUnmanagedCodeSecurity,
		 ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[ResourceExposure(ResourceScope.None)]
		private static extern IntPtr LocalFree(IntPtr handle);

		[SecurityCritical]
		override protected bool ReleaseHandle()
			{
			return LocalFree(handle) == IntPtr.Zero;
			}
		}

	[SecurityCritical]
	public sealed class SafeCryptProvHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
		private SafeCryptProvHandle() : base(true)
			{
			}

		// 0 is an Invalid Handle
		public SafeCryptProvHandle(IntPtr handle) : base(true)
			{
			SetHandle(handle);
			}

		public static SafeCryptProvHandle InvalidHandle
			{
			get
				{
				return new SafeCryptProvHandle(IntPtr.Zero);
				}
			}

		[DllImport(CAPI.ADVAPI32, SetLastError = true),
		 SuppressUnmanagedCodeSecurity,
		 ResourceExposure(ResourceScope.None),
		 ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		private static extern bool CryptReleaseContext(IntPtr hCryptProv, uint dwFlags);

		[SecurityCritical]
		override protected bool ReleaseHandle()
			{
			return CryptReleaseContext(handle, 0);
			}
		}

	[SecurityCritical]
	public sealed class SafeCryptKeyHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
		private SafeCryptKeyHandle() : base(true)
			{
			}

		// 0 is an Invalid Handle
		public SafeCryptKeyHandle(IntPtr handle) : base(true)
			{
			SetHandle(handle);
			}

		public static SafeCryptKeyHandle InvalidHandle
			{
			get
				{
				return new SafeCryptKeyHandle(IntPtr.Zero);
				}
			}

		[DllImport(CAPI.ADVAPI32, SetLastError = true),
			SuppressUnmanagedCodeSecurity,
			ResourceExposure(ResourceScope.None)]
		private extern static bool CryptDestroyKey(
			[In] IntPtr pKey);

		[SecurityCritical]
		override protected bool ReleaseHandle()
			{
			return CryptDestroyKey(handle);
			}
		}

	[SecurityCritical]
	public sealed class SafeCryptHashHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
		private SafeCryptHashHandle() : base(true)
			{
			}

		// 0 is an Invalid Handle
		public SafeCryptHashHandle(IntPtr handle) : base(true)
			{
			SetHandle(handle);
			}

		public static SafeCryptHashHandle InvalidHandle
			{
			get
				{
				return new SafeCryptHashHandle(IntPtr.Zero);
				}
			}

		[DllImport(CAPI.ADVAPI32, SetLastError = true),
			SuppressUnmanagedCodeSecurity,
			ResourceExposure(ResourceScope.None)]
		private extern static bool CryptDestroyHash(
			[In] IntPtr phHash);

		[SecurityCritical]
		override protected bool ReleaseHandle()
			{
			return CryptDestroyHash(handle);
			}
		}

	[SecurityCritical]
	public sealed class SafeCertContextHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
		private SafeCertContextHandle() : base(true)
			{
			}

		// 0 is an Invalid Handle
		public SafeCertContextHandle(IntPtr handle) : base(true)
			{
			SetHandle(handle);
			}

		public static SafeCertContextHandle InvalidHandle
			{
			get
				{
				return new SafeCertContextHandle(IntPtr.Zero);
				}
			}

		[DllImport(CAPI.CRYPT32, SetLastError = true),
		 SuppressUnmanagedCodeSecurity,
		 ResourceExposure(ResourceScope.None),
		 ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		private static extern bool CertFreeCertificateContext(IntPtr pCertContext);

		[SecurityCritical]
		override protected bool ReleaseHandle()
			{
			return CertFreeCertificateContext(handle);
			}
		}

	[SecurityCritical]
	public sealed class SafeCertStoreHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
		private SafeCertStoreHandle() : base(true)
			{
			}

		// 0 is an Invalid Handle
		public SafeCertStoreHandle(IntPtr handle) : base(true)
			{
			SetHandle(handle);
			}

		public static SafeCertStoreHandle InvalidHandle
			{
			get
				{
				return new SafeCertStoreHandle(IntPtr.Zero);
				}
			}

		[DllImport(CAPI.CRYPT32, SetLastError = true),
		 SuppressUnmanagedCodeSecurity,
		 ResourceExposure(ResourceScope.None),
		 ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		private static extern bool CertCloseStore(IntPtr hCertStore, uint dwFlags);

		[SecurityCritical]
		override protected bool ReleaseHandle()
			{
			return CertCloseStore(handle, 0);
			}
		}

	[SecurityCritical]
	public sealed class SafeCryptMsgHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
		private SafeCryptMsgHandle() : base(true)
			{
			}

		public SafeCryptMsgHandle(IntPtr handle) : base(true)
			{
			SetHandle(handle);
			}

		public static SafeCryptMsgHandle InvalidHandle
			{
			get
				{
				return new SafeCryptMsgHandle(IntPtr.Zero);
				}
			}

		[DllImport(CAPI.CRYPT32, SetLastError = true),
		 SuppressUnmanagedCodeSecurity,
		 ResourceExposure(ResourceScope.None),
		 ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		private static extern bool CryptMsgClose(IntPtr handle);

		[SecurityCritical]
		override protected bool ReleaseHandle()
			{
			return CryptMsgClose(handle);
			}
		}

	[SecurityCritical]
	public sealed class SafeCertChainHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
		private SafeCertChainHandle() : base(true)
			{
			}

		public SafeCertChainHandle(IntPtr handle) : base(true)
			{
			SetHandle(handle);
			}

		public static SafeCertChainHandle InvalidHandle
			{
			get
				{
				return new SafeCertChainHandle(IntPtr.Zero);
				}
			}

		[DllImport(CAPI.CRYPT32, SetLastError = true),
		 SuppressUnmanagedCodeSecurity,
		 ResourceExposure(ResourceScope.None),
		 ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		private static extern void CertFreeCertificateChain(IntPtr handle);

		[SecurityCritical]
		override protected bool ReleaseHandle()
			{
			CertFreeCertificateChain(handle);
			return true;
			}
		}
	} // end namespace

// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
