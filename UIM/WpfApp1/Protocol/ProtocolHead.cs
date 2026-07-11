using System;
using System.IO;
using System.Text;

namespace WpfApp1.Protocol
{
    public class ProtocolHead
    {
        // ================================================================
        // КОНСТАНТЫ
        // ================================================================

        /// <summary>
        /// Магическое число для идентификации файла ("UIMP" в hex)
        /// </summary>
        private const uint DefaultFileNumber = 0x55494D50;

        /// <summary>
        /// Размер заголовка в байтах (4+8+32+32+16+4 = 96)
        /// </summary>
        public const int HeaderSize = 96;

        // ================================================================
        // ПОЛЯ
        // ================================================================

        /// <summary>
        /// Магическое число (реальное значение из файла)
        /// </summary>
        private uint _fileNumber;

        /// <summary>
        /// Версия ПО
        /// </summary>
        private uint _softwareVersion;

        /// <summary>
        /// Размер зашифрованных данных
        /// </summary>
        private long _dataSize;

        /// <summary>
        /// SHA-256 хеш данных (32 байта)
        /// </summary>
        private byte[] _dataHash;

        /// <summary>
        /// SHA-256 хеш расчетного ядра (32 байта)
        /// </summary>
        private byte[] _coreHash;

        /// <summary>
        /// Время создания файла (16 байт: "YYYYMMDDHHMMSS")
        /// </summary>
        private byte[] _timestamp;

        // ================================================================
        // КОНСТРУКТОР
        // ================================================================

        /// <summary>
        /// Создание нового заголовка со значениями по умолчанию
        /// </summary>
        public ProtocolHead()
        {
            _fileNumber = DefaultFileNumber;
            _softwareVersion = 1;
            _dataSize = 0;
            _dataHash = new byte[32];
            _coreHash = new byte[32];
            _timestamp = new byte[16];
        }

        // ================================================================
        // СВОЙСТВА
        // ================================================================

        /// <summary>
        /// Файловое число
        /// </summary>
        public uint FileNumber => _fileNumber;

        /// <summary>
        /// Версия ПО
        /// </summary>
        public uint SoftwareVersion
        {
            get => _softwareVersion;
            set => _softwareVersion = value;
        }

        /// <summary>
        /// Размер зашифрованных данных
        /// </summary>
        public long DataSize
        {
            get => _dataSize;
            set => _dataSize = value;
        }

        /// <summary>
        /// Хеш данных
        /// </summary>
        public byte[] DataHash => _dataHash;

        /// <summary>
        /// Хеш ядра
        /// </summary>
        public byte[] CoreHash => _coreHash;

        // ================================================================
        // МЕТОДЫ ЗАПИСИ/ЧТЕНИЯ
        // ================================================================

        /// <summary>
        /// Запись заголовка в бинарный поток
        /// </summary>
        public void Write(BinaryWriter writer)
        {
            writer.Write(_fileNumber);       // 4 байта
            writer.Write(_dataSize);         // 8 байт
            writer.Write(_dataHash);         // 32 байта
            writer.Write(_coreHash);         // 32 байта
            writer.Write(_timestamp);        // 16 байт
            writer.Write(_softwareVersion);  // 4 байта
            // Итого: 96 байт
        }

        /// <summary>
        /// Чтение заголовка из бинарного потока
        /// </summary>
        public void Read(BinaryReader reader)
        {
            _fileNumber = reader.ReadUInt32();       // 4 байта
            _dataSize = reader.ReadInt64();          // 8 байт
            _dataHash = reader.ReadBytes(32);        // 32 байта
            _coreHash = reader.ReadBytes(32);        // 32 байта
            _timestamp = reader.ReadBytes(16);       // 16 байт
            _softwareVersion = reader.ReadUInt32();  // 4 байта
            // Итого: 96 байт
        }

        // ================================================================
        // МЕТОДЫ ПРОВЕРКИ
        // ================================================================

        /// <summary>
        /// Проверка валидности заголовка (магическое число совпадает)
        /// </summary>
        public bool IsValid()
        {
            return _fileNumber == DefaultFileNumber;
        }

        /// <summary>
        /// Проверка хеша данных
        /// </summary>
        public bool VerifyDataHash(byte[] actualHash)
        {
            if (actualHash == null || actualHash.Length != 32)
                return false;

            for (int i = 0; i < 32; i++)
            {
                if (_dataHash[i] != actualHash[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Проверка хеша ядра
        /// </summary>
        public bool VerifyCoreHash(byte[] actualHash)
        {
            if (actualHash == null || actualHash.Length != 32)
                return false;

            for (int i = 0; i < 32; i++)
            {
                if (_coreHash[i] != actualHash[i])
                    return false;
            }
            return true;
        }

        // ================================================================
        // МЕТОДЫ УСТАНОВКИ
        // ================================================================

        /// <summary>
        /// Установка хеша данных
        /// </summary>
        public void SetDataHash(byte[] hash)
        {
            if (hash == null || hash.Length != 32)
                throw new ArgumentException("Хеш должен быть 32 байта");

            Array.Copy(hash, _dataHash, 32);
        }

        /// <summary>
        /// Установка хеша ядра
        /// </summary>
        public void SetCoreHash(byte[] hash)
        {
            if (hash == null || hash.Length != 32)
                throw new ArgumentException("Хеш должен быть 32 байта");

            Array.Copy(hash, _coreHash, 32);
        }

        /// <summary>
        /// Установка версии ПО
        /// </summary>
        public void SetSoftwareVersion(uint version)
        {
            if (version == 0)
                throw new ArgumentException("Версия не может быть 0");
            _softwareVersion = version;
        }

        // ================================================================
        // МЕТОДЫ РАБОТЫ С ДАТОЙ
        // ================================================================

        /// <summary>
        /// Установка времени создания
        /// </summary>
        public void SetTimestamp(DateTime time)
        {
            string dateStr = time.ToString("yyyyMMddHHmmss");
            byte[] bytes = Encoding.ASCII.GetBytes(dateStr);

            Array.Clear(_timestamp, 0, _timestamp.Length);
            Array.Copy(bytes, _timestamp, Math.Min(bytes.Length, _timestamp.Length));
        }

        /// <summary>
        /// Получение времени создания
        /// </summary>
        public DateTime GetTimestamp()
        {
            int length = 0;
            while (length < _timestamp.Length && _timestamp[length] != 0)
                length++;

            if (length == 0)
                return DateTime.MinValue;

            string dateStr = Encoding.ASCII.GetString(_timestamp, 0, length);

            if (dateStr.Length < 14)
                return DateTime.MinValue;

            try
            {
                int year = int.Parse(dateStr.Substring(0, 4));
                int month = int.Parse(dateStr.Substring(4, 2));
                int day = int.Parse(dateStr.Substring(6, 2));
                int hour = int.Parse(dateStr.Substring(8, 2));
                int minute = int.Parse(dateStr.Substring(10, 2));
                int second = int.Parse(dateStr.Substring(12, 2));

                return new DateTime(year, month, day, hour, minute, second);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }
}