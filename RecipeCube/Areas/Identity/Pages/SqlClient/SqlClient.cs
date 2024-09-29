using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RecipeCube.Models;
using System.Security.Claims;

namespace RecipeCube.Areas.Identity.Pages.SqlClient
{
    // 連線字串
    public static class DatabaseConfig
    {
        public static string ConnectionString => "Data Source=.;Initial Catalog=RecipeCube;TrustServerCertificate=True;Integrated Security=true;MultipleActiveResultSets=true";
    }

    // 更新指定資料表的指定欄位(一到多個)
    public class UpdateSql
    {
        //連線字串
        private readonly string _connectionString = DatabaseConfig.ConnectionString;
        // 更新資料表欄位內容
        public async Task<int> UpdateTableAsync(string tableName, Dictionary<string, object> fieldData, string primaryKeyField, object primaryKeyValue)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // 正確生成 SET 子句的參數化語句，將每個欄位名對應到一個參數
                var setClauses = new List<string>();
                foreach (var key in fieldData.Keys)
                {
                    setClauses.Add($"{key} = @{key}");
                }

                var commandText = $"UPDATE [{tableName}] SET {string.Join(", ", setClauses)} WHERE {primaryKeyField} = @PrimaryKeyValue";

                var command = new SqlCommand(commandText, connection);

                // 添加參數
                foreach (var item in fieldData)
                {
                    command.Parameters.AddWithValue($"@{item.Key}", item.Value);
                }
                command.Parameters.AddWithValue("@PrimaryKeyValue", primaryKeyValue);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected;
            }
        }
    }

    // 查詢使用者偏好 or 不可食用食材
    public class JOINSql
    {
        //連線字串
        private readonly string _connectionString = DatabaseConfig.ConnectionString;
        // 更新資料表欄位內容
        public async Task<List<string>> SelectJoinAsync(string tableName, object UserId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var commandText = $@"
                SELECT STRING_AGG([Ingredients].ingredient_name, ', ') AS ingredient_names 
                FROM [{tableName}] 
                JOIN [Ingredients] ON [{tableName}].ingredient_id = [Ingredients].ingredient_id
                WHERE [{tableName}].user_id = @UserId";

                var command = new SqlCommand(commandText, connection);

                // 添加參數
                
                command.Parameters.AddWithValue("@UserId", UserId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var results = new List<string>();

                    // 讀取查詢結果
                    while (await reader.ReadAsync())
                    {
                        var ingredientNames = reader["ingredient_names"].ToString();
                        results.Add(ingredientNames);
                    }

                    return results;
                }
            }
        }
    }


    // 查詢所有食材&依照食材id和userid寫入使用者偏好 or 不可食用食材
    public class FoodService
    {
        // 連線字串
        private readonly string _connectionString = DatabaseConfig.ConnectionString;
        // 定義 Ingredient 類別
        public class Food
        {
            public int IngredientId { get; set; }
            public string IngredientName { get; set; }
            public string Category { get; set; }
        }

        // 查詢所有食材
        public async Task<List<Food>> GetAllIngredientsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var commandText = @"SELECT ingredient_id, ingredient_name, category FROM Ingredients";
                var command = new SqlCommand(commandText, connection);

                var ingredients = new List<Food>();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ingredients.Add(new Food
                        {
                            IngredientId = reader.GetInt32(0),
                            IngredientName = reader.GetString(1),
                            Category = reader.GetString(2)
                        });
                    }
                }
                return ingredients;
            }
        }

        public async Task<int> SaveIngredientsAsync(string tableName, string userId, int ingredientId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var commandText = $@"INSERT INTO [{tableName}] ([user_id], [ingredient_id]) 
                             VALUES (@user_id, @ingredientId)";
                var command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@user_id", userId);
                command.Parameters.AddWithValue("@ingredientId", ingredientId);
                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected;
            }
        }
    }

    // 依據使用者選取 偏好 or 不可食用食材 刪除
    public class DeleteFoodService
    {
        // 連線字串
        private readonly string _connectionString = DatabaseConfig.ConnectionString;
        public async Task<int> DeletePreferenceAsync(string tableName, string userId, int ingredientId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var commandText = $@"DELETE FROM [{tableName}] WHERE [user_id] = @userId AND [ingredient_id] = @ingredientId";
                var command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@ingredientId", ingredientId);
                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected;
            }
        }

    }
}
