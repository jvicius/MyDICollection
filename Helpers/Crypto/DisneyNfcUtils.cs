using System.Security.Cryptography;
using System.Text;

namespace MyDICollection.Helpers.Crypto;

/// <summary>
/// Read-only Disney Infinity NFC interoperability helpers used by MyDICollection.
///
/// Protocol behavior and interoperability details were implemented in C# with
/// reference to publicly available community research, including historical
/// Proxmark3/RfidResearchGroup work.
///
/// This project does not include, link against, or redistribute Proxmark3
/// source code. See THIRD_PARTY_NOTICES.md for attribution and licensing details.
/// </summary>
public static class DisneyNfcUtils
{
    private const int DisneyUidLength = 7;
    private const int NfcBlockSize = 16;
    private const int IdentificationBlockOffset = 0x10;
    private const int PayloadLength = 12;

    // Protocol/interoperability material used to derive the per-tag MIFARE key.
    // These values are not application secrets.
    private static readonly byte[] AuthenticationContext =
    {
        0x0A, 0x14, 0xFD, 0x05,
        0x07, 0xFF, 0x4B, 0xCD,
        0x02, 0x6B, 0xA8, 0x3F,
        0x0A, 0x3B, 0x89, 0xA9
    };

    // ASCII: "(c) Disney 2013".
    private static readonly byte[] ProtocolLabel =
    {
        0x28, 0x63, 0x29, 0x20,
        0x44, 0x69, 0x73, 0x6E,
        0x65, 0x79, 0x20, 0x32,
        0x30, 0x31, 0x33
    };

    // Protocol/interoperability material used to derive the per-tag data key.
    // This value is not the final AES key; the AES key is derived per tag UID.
    private static readonly byte[] DataKeyContext =
        Convert.FromHexString("AF62D2EC0491968CC52A1A7165F865FE");

    /// <summary>
    /// Identification information extracted from a physical Disney Infinity tag.
    /// </summary>
    public sealed class DisneyFigureInfo
    {
        public required byte[] Uid { get; init; }

        public required string UidHex { get; init; }

        public required uint ModelNumber { get; init; }

        public required string InfCode { get; init; }

        public required byte[] MifareKey { get; init; }

        public required byte[] AesKey { get; init; }

        public required byte[] EncryptedBlock1 { get; init; }

        public required byte[] DecryptedBlock1 { get; init; }

        public required uint StoredChecksum { get; init; }

        public required uint CalculatedChecksum { get; init; }

        public bool IsChecksumValid =>
            StoredChecksum == CalculatedChecksum;
    }

    /// <summary>
    /// Derives the six-byte MIFARE authentication key for a Disney Infinity tag.
    /// The same derived value can be used as Key A or Key B.
    /// </summary>
    public static byte[] CalculateMifareKey(byte[] uid)
    {
        ValidateUid(uid);

        byte[] input = new byte[
            AuthenticationContext.Length +
            uid.Length +
            ProtocolLabel.Length];

        int offset = 0;

        Buffer.BlockCopy(
            AuthenticationContext,
            0,
            input,
            offset,
            AuthenticationContext.Length);

        offset += AuthenticationContext.Length;

        Buffer.BlockCopy(
            uid,
            0,
            input,
            offset,
            uid.Length);

        offset += uid.Length;

        Buffer.BlockCopy(
            ProtocolLabel,
            0,
            input,
            offset,
            ProtocolLabel.Length);

        using SHA1 sha1 = SHA1.Create();
        byte[] hash = sha1.ComputeHash(input);

        return
        [
            hash[3],
            hash[2],
            hash[1],
            hash[0],
            hash[7],
            hash[6]
        ];
    }

    /// <summary>
    /// Derives the per-tag AES-128 key used to decode identification data.
    /// </summary>
    public static byte[] CreateDisneyAesKey(byte[] uid)
    {
        ValidateUid(uid);

        byte[] input = new byte[
            DataKeyContext.Length +
            ProtocolLabel.Length +
            uid.Length];

        int offset = 0;

        Buffer.BlockCopy(
            DataKeyContext,
            0,
            input,
            offset,
            DataKeyContext.Length);

        offset += DataKeyContext.Length;

        Buffer.BlockCopy(
            ProtocolLabel,
            0,
            input,
            offset,
            ProtocolLabel.Length);

        offset += ProtocolLabel.Length;

        Buffer.BlockCopy(
            uid,
            0,
            input,
            offset,
            uid.Length);

        using SHA1 sha1 = SHA1.Create();
        byte[] hash = sha1.ComputeHash(input);

        byte[] key = new byte[16];
        Buffer.BlockCopy(hash, 0, key, 0, key.Length);

        // The protocol represents the first 16 SHA-1 bytes as four
        // 32-bit words with reversed byte order.
        for (int offset32 = 0; offset32 < key.Length; offset32 += 4)
        {
            Array.Reverse(key, offset32, 4);
        }

        return key;
    }

    /// <summary>
    /// Decodes one 16-byte Disney Infinity data block using AES-128 ECB.
    /// </summary>
    public static byte[] DecryptBlock(
        byte[] encryptedBlock,
        byte[] uid)
    {
        ArgumentNullException.ThrowIfNull(encryptedBlock);

        if (encryptedBlock.Length != NfcBlockSize)
        {
            throw new ArgumentException(
                $"El bloque debe contener exactamente {NfcBlockSize} bytes.",
                nameof(encryptedBlock));
        }

        byte[] aesKey = CreateDisneyAesKey(uid);

        using Aes aes = Aes.Create();
        aes.Key = aesKey;
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;

        using ICryptoTransform decryptor = aes.CreateDecryptor();

        return decryptor.TransformFinalBlock(
            encryptedBlock,
            0,
            encryptedBlock.Length);
    }

    /// <summary>
    /// Attempts to identify a Disney Infinity figure from an already-read NFC dump.
    /// This method performs no NFC writes and does not modify tag data.
    /// </summary>
    public static DisneyFigureInfo? TryGetFigureInfo(
    byte[] fullDump,
    byte[] uid)
    {
        try
        {
            ValidateUid(uid);

            if (fullDump == null ||
                fullDump.Length <
                IdentificationBlockOffset + NfcBlockSize)
            {
                return null;
            }

            byte[] encryptedBlock1 =
                new byte[NfcBlockSize];

            Buffer.BlockCopy(
                fullDump,
                IdentificationBlockOffset,
                encryptedBlock1,
                0,
                encryptedBlock1.Length);

            return TryGetFigureInfoFromBlock1(
                encryptedBlock1,
                uid);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    /// <summary>
    /// Calculates the CRC used by the 12-byte identification payload.
    /// Polynomial: 0xEDB88320, initial value: 0, no final XOR.
    /// </summary>
    public static uint CalculateDisneyChecksum(
        ReadOnlySpan<byte> data)
    {
        uint crc = 0;

        foreach (byte value in data)
        {
            crc ^= value;

            for (int bit = 0; bit < 8; bit++)
            {
                crc = (crc & 1) != 0
                    ? (crc >> 1) ^ 0xEDB88320u
                    : crc >> 1;
            }
        }

        return crc;
    }

    public static string ToHex(byte[] data)
    {
        if (data == null || data.Length == 0)
            return string.Empty;

        return BitConverter
            .ToString(data)
            .Replace("-", " ");
    }

    public static string ToHexCompact(byte[] data)
    {
        if (data == null || data.Length == 0)
            return string.Empty;

        return Convert.ToHexString(data);
    }

    public static string ToAscii(byte[] data)
    {
        if (data == null || data.Length == 0)
            return string.Empty;

        StringBuilder sb = new(data.Length);

        foreach (byte value in data)
        {
            sb.Append(
                value is >= 32 and <= 126
                    ? (char)value
                    : '.');
        }

        return sb.ToString();
    }

    private static uint ReadUInt32BigEndian(
        byte[] data,
        int offset)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (offset < 0 || data.Length < offset + sizeof(uint))
        {
            throw new ArgumentOutOfRangeException(
                nameof(offset),
                "No hay suficientes bytes para leer un UInt32.");
        }

        return
            ((uint)data[offset] << 24) |
            ((uint)data[offset + 1] << 16) |
            ((uint)data[offset + 2] << 8) |
            data[offset + 3];
    }

    private static void ValidateUid(byte[] uid)
    {
        ArgumentNullException.ThrowIfNull(uid);

        if (uid.Length != DisneyUidLength)
        {
            throw new ArgumentException(
                $"Disney Infinity requiere un UID de {DisneyUidLength} bytes.",
                nameof(uid));
        }
    }

    /// <summary>
    /// Attempts to identify a Disney Infinity figure
    /// directly from encrypted block 1.
    /// </summary>
    public static DisneyFigureInfo? TryGetFigureInfoFromBlock1(
        byte[] encryptedBlock1,
        byte[] uid)
    {
        try
        {
            ValidateUid(uid);

            if (encryptedBlock1 == null ||
                encryptedBlock1.Length != NfcBlockSize)
            {
                return null;
            }

            byte[] decryptedBlock1 =
                DecryptBlock(
                    encryptedBlock1,
                    uid);

            uint modelNumber =
                ReadUInt32BigEndian(
                    decryptedBlock1,
                    0);

            uint storedChecksum =
                ReadUInt32BigEndian(
                    decryptedBlock1,
                    PayloadLength);

            uint calculatedChecksum =
                CalculateDisneyChecksum(
                    decryptedBlock1.AsSpan(
                        0,
                        PayloadLength));

            return new DisneyFigureInfo
            {
                Uid = (byte[])uid.Clone(),

                UidHex =
                    ToHexCompact(uid),

                ModelNumber =
                    modelNumber,

                InfCode =
                    $"INF-{modelNumber:D7}",

                MifareKey =
                    CalculateMifareKey(uid),

                AesKey =
                    CreateDisneyAesKey(uid),

                EncryptedBlock1 =
                    (byte[])encryptedBlock1.Clone(),

                DecryptedBlock1 =
                    decryptedBlock1,

                StoredChecksum =
                    storedChecksum,

                CalculatedChecksum =
                    calculatedChecksum
            };
        }
        catch (CryptographicException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
