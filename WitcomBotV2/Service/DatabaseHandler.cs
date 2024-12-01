using System.Data;
using System.Reflection;
using Discord.WebSocket;

namespace WitcomBotV2.Service;

using System.Globalization;
using System.Net;
using Microsoft.Data.Sqlite;
using Discord;

public class DatabaseHandler
{
    private static string _connectionString = $"Data Source={Program.DatabaseFile}";

    public static async Task Init(bool updateTables = false)
    {
        Log.Info(nameof(Init), $"Initializing database at {_connectionString}..");
        if (!File.Exists(Program.DatabaseFile) || updateTables)
        {
            Log.Info(nameof(Init), "Database file not found, creating..");
            using SqliteConnection connection = new(_connectionString);
            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating table 'ping'..");
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS Pings(Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT, Message TEXT)";
                cmd.ExecuteNonQuery();
            }
            
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating table 'roleReaction'..");
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS RoleReaction(Id INTEGER PRIMARY KEY AUTOINCREMENT, RoleId TEXT ,MessageId TEXT, EmoteId TEXT, ChannelId Text)";
                cmd.ExecuteNonQuery();
            }
        }
        
    }

    public static void AddEntry(ulong id, string description, DatabaseType type, ulong staffId = 0)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = type switch
            {
                DatabaseType.Ping => "INSERT INTO Pings(UserId, Message) VALUES(@id, @string)",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            cmd.Parameters.AddWithValue("@id", id.ToString());

            if (type is not DatabaseType.SelfRole)
            {
                if (type is DatabaseType.Tags)
                {
                    string[] tagArray = description.Split('|');
                    cmd.Parameters.AddWithValue("@name", tagArray[0]);
                    cmd.Parameters.AddWithValue("@tag", tagArray[1]);
                }
                else
                    cmd.Parameters.AddWithValue("@string", description);

                if (type is not DatabaseType.Ping && type is not DatabaseType.BugReport)
                {
                    cmd.Parameters.AddWithValue("@staff", staffId.ToString());
                    cmd.Parameters.AddWithValue("@issued", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                }
            }

            cmd.ExecuteNonQuery();
        }
        
        connection.Close();
    }
    
    public static void RemoveEntry(int id, DatabaseType type)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = type switch
            {
                DatabaseType.ReactRole => "DELETE FROM RoleReaction WHERE Id=@id",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        
        connection.Close();
    }
    
    public static void RemoveEntry(ulong userId, DatabaseType type)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = type switch
            {
                DatabaseType.Ping => "DELETE FROM Pings WHERE UserId=@id",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            cmd.Parameters.AddWithValue("@id", userId.ToString());
            cmd.ExecuteNonQuery();
        }
        
        connection.Close();
    }
    
    public static string GetPingTrigger(ulong userId)
    {
        string message = string.Empty;
        using (SqliteConnection connection = new(_connectionString))
        {
            connection.Open();
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Pings WHERE UserId=@id";
                cmd.Parameters.AddWithValue("@id", userId.ToString());

                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        message = reader.GetString(2);
                        break;
                    }
                }
            }
            
            connection.Close();
        }
        
        if (string.IsNullOrEmpty(message))
            Log.Debug(nameof(GetPingTrigger), $"Returning null ping trigger message for {userId}.");
        return message;
    }

    public static List<ReactionRoleData> GetReactionRoleData()
    {
        List<ReactionRoleData> reactionRoleData = new();
        using (SqliteConnection connection = new(_connectionString))
        {
            connection.Open();
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM RoleReaction";

                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reactionRoleData.Add(new ReactionRoleData
                        {
                            Id = reader.GetInt32(0),
                            RoleId = reader.GetString(1),
                            MessageId = reader.GetString(2),
                            Emote = reader.GetString(3),
                            ChannelId = reader.GetString(4)
                        });
                    }
                }
            }
            
            connection.Close();
        }
        
        return reactionRoleData;
    }

    public static void AddReactionRoleData(ulong roleId, ulong messageId, string emote, ulong channelId)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = "INSERT INTO RoleReaction(RoleId, MessageId, EmoteId, ChannelId) VALUES(@roleId, @messageId, @emote, @channelId)";
            cmd.Parameters.AddWithValue("@roleId", roleId.ToString());
            cmd.Parameters.AddWithValue("@messageId", messageId.ToString());
            cmd.Parameters.AddWithValue("@emote", emote);
            cmd.Parameters.AddWithValue("@channelId", channelId.ToString());
            cmd.ExecuteNonQuery();
        }
        
        connection.Close();
    }
}