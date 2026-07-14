using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.IO.Compression;

namespace WpfApp1.Protocol
{
    public class ProtocolService
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public ProtocolService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,  // Чтобы JSON был читаемый
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        /// <summary>
        /// Сохранение протокола в файл
        /// </summary>
        public void Save(string filename, ProtocolData data)
        {
            // 1. Сериализуем в JSON
            string json = JsonSerializer.Serialize(data, _jsonOptions);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            // 2. Сжимаем
            byte[] compressed = Compress(jsonBytes);

            // 3. Шифруем (пока пропускаем, просто копируем)
            byte[] encrypted = compressed; // TODO: добавить шифрование

            // 4. Создаем заголовок
            ProtocolHead header = new ProtocolHead();
            header.DataSize = encrypted.Length;
            header.SetTimestamp(DateTime.Now);
            header.SetSoftwareVersion(1);

            // 5. Считаем хеш
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(encrypted);
                header.SetDataHash(hash);
            }

            // 6. Записываем в файл
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                header.Write(writer);      // 96 байт
                writer.Write(encrypted);   // данные
            }
        }

        /// <summary>
        /// Загрузка протокола из файла
        /// </summary>
        public ProtocolData Load(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // 1. Читаем заголовок
                ProtocolHead header = new ProtocolHead();
                header.Read(reader);

                // 2. Проверяем валидность
                if (!header.IsValid())
                    throw new Exception("Неверный формат файла");

                // 3. Читаем данные
                byte[] encrypted = reader.ReadBytes((int)header.DataSize);

                // 4. Проверяем хеш
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(encrypted);
                    if (!header.VerifyDataHash(hash))
                        throw new Exception("Нарушена целостность данных");
                }

                // 5. Расшифровываем (пока пропускаем)
                byte[] compressed = encrypted;

                // 6. Распаковываем
                byte[] jsonBytes = Decompress(compressed);

                // 7. Десериализуем
                string json = Encoding.UTF8.GetString(jsonBytes);
                ProtocolData data = JsonSerializer.Deserialize<ProtocolData>(json, _jsonOptions);

                return data;
            }
        }

        // ================================================================
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        // ================================================================

        private byte[] Compress(byte[] data)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
                {
                    gzipStream.Write(data, 0, data.Length);
                }
                return memoryStream.ToArray();
            }
        }

        private byte[] Decompress(byte[] compressed)
        {
            using (var memoryStream = new MemoryStream(compressed))
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                gzipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}