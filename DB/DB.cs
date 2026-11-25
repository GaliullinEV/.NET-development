using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;

namespace database;
class DB{
    private const string HOST = "localhost";
    private const string PORT = "3306";
    private const string DATABASE = "zhenya";
    private const string USER = "root";
    private const string PASS = "";
    private string connect;
    public DB(){
        connect = $"Server={HOST};User={USER};Password={PASS};Database={DATABASE};Port={PORT};";
    }
    public async Task CreateTable(){
        string sql = "CREATE TABLE IF NOT EXISTS `users` (" +
            "id  INT(11) AUTO_INCREMENT PRIMARY KEY," +
            "login VARCHAR(50)," +
            "password VARCHAR(50)" +
            ") ENGINE=MYISAM";
        await ExecuteQuery(sql);
    }
    public async Task InsertData(string? title, string? text, string? date, string? avtor){

        string sql = "INSERT INTO `articles` (title, text, date, avtor) VALUES (@title, @text, @date, @avtor)";
        Dictionary<string, string?> parametrs = new Dictionary<string, string?>();
        parametrs.Add("title", title);
        parametrs.Add("text", text);
        parametrs.Add("date", date);
        parametrs.Add("avtor", avtor);
        await ExecuteQuery(sql, parametrs);
    }
    private async Task ExecuteQuery(string sql, Dictionary<string, string?>? parametrs = null){
        using(MySqlConnection conn = new MySqlConnection(connect)){

            await conn.OpenAsync();
            MySqlCommand command = new MySqlCommand(sql, conn);

            if(parametrs != null){
                foreach(var el in parametrs){
                    MySqlParameter param = new MySqlParameter($"@{el.Key}", el.Value);
                    command.Parameters.Add(param);
                }
            }
            await command.ExecuteNonQueryAsync();
        }
    }
    public async Task GetData(int number){
        using(MySqlConnection conn = new MySqlConnection(connect)){

            await conn.OpenAsync();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `articles` WHERE title LIKE '%oo%' OR id >= @number  ORDER BY id DESK LIMIT 2 OFFSET 1", conn);
            MySqlParameter param = new MySqlParameter($"@number", number);
            command.Parameters.Add(param);

            MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync();

            if(reader.HasRows){ 
                while(await reader.ReadAsync()){
                    object id = reader["id"];
                    object title = reader["title"];
                    object text = reader["text"];
                    object date = reader["date"];
                    object avtor = reader["avtor"];
                    
                    System.Console.WriteLine($"{id}. {title} - {avtor}");
                } 
            }
           await reader.CloseAsync();
        }
    }

    public async Task UpdateData(string? title, string? text, string? date, string? avtor){

        string sql = "UPDATE `articles` SET title = @title, text = @text, date = @date, avtor = @avtor WHERE id = 2";
        Dictionary<string, string?> parametrs = new Dictionary<string, string?>();
        parametrs.Add("title", title);
        parametrs.Add("text", text);
        parametrs.Add("date", date);
        parametrs.Add("avtor", avtor);
        await ExecuteQuery(sql, parametrs);
    }
    public async Task DeleteData(){
        string sql = "DELETE FROM `articles` WHERE title = 'Tesla'";
        await ExecuteQuery(sql);
    }

    public async Task CountData(){
        string sql = "SELECT COUNT(id) FROM `articles`";

         using(MySqlConnection conn = new MySqlConnection(connect)){

            await conn.OpenAsync();

            MySqlCommand command = new MySqlCommand(sql, conn);
            object? num = await command.ExecuteScalarAsync(); 
            System.Console.WriteLine(num);
         }
    }
}