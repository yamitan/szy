using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Microsoft.Extensions.Logging;

namespace szy.Services
{
    /// <summary>
    /// SQLite 数据库服务
    /// </summary>
    public class DatabaseService
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseService> _logger;
        private readonly string _dbPath;

        public DatabaseService(ILogger<DatabaseService> logger, ConfigService configService)
        {
            _logger = logger;
            
            // 从配置中获取数据库路径
            _dbPath = configService.GetDatabasePath();
            
            // 确保数据库文件存在
            if (!File.Exists(_dbPath))
            {
                _logger.LogWarning($"数据库文件不存在: {_dbPath}");
            }
            
            // 设置连接字符串
            _connectionString = $"Data Source={_dbPath};Version=3;";
        }

        /// <summary>
        /// 执行查询并返回 DataTable
        /// </summary>
        public DataTable ExecuteQuery(string sql, params SQLiteParameter[] parameters)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();
                
                using var command = new SQLiteCommand(sql, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                
                using var adapter = new SQLiteDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                
                return dataTable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"执行查询失败: {sql}");
                throw;
            }
        }

        /// <summary>
        /// 执行非查询SQL语句
        /// </summary>
        public int ExecuteNonQuery(string sql, params SQLiteParameter[] parameters)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();
                
                using var command = new SQLiteCommand(sql, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"执行非查询语句失败: {sql}");
                throw;
            }
        }

        /// <summary>
        /// 执行查询并返回第一行第一列的值
        /// </summary>
        public object ExecuteScalar(string sql, params SQLiteParameter[] parameters)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();
                
                using var command = new SQLiteCommand(sql, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                
                return command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"执行标量查询失败: {sql}");
                throw;
            }
        }

        /// <summary>
        /// 获取数据库状态信息
        /// </summary>
        public object GetDatabaseInfo()
        {
            try
            {
                var fileInfo = new FileInfo(_dbPath);
                
                // 获取表信息
                var tables = ExecuteQuery("SELECT name FROM sqlite_master WHERE type='table'");
                var tableList = new List<string>();
                
                foreach (DataRow row in tables.Rows)
                {
                    tableList.Add(row["name"].ToString());
                }

                return new
                {
                    DatabasePath = _dbPath,
                    DatabaseSize = $"{fileInfo.Length / 1024.0:F2} KB",
                    LastModified = fileInfo.LastWriteTime,
                    Tables = tableList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取数据库信息失败");
                throw;
            }
        }
    }
} 