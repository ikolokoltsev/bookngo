using server.Activities.Models;

namespace server.Activities.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly Config _config;

    public ActivityRepository(Config config)
    {
        _config = config;
    }

    public async Task<IEnumerable<ActivityData>> GetAllActivities(ActivityFilterQuery filter)
    {
        List<ActivityData> activities = new();
        List<string> queryParts = new()
        {
            "SELECT Id, Name, Country, City, Address, start_time, end_time, Price, activity_type, Status FROM activities WHERE 1=1"
        };
        List<MySqlParameter> parameters = new();

        if (filter.MinPrice.HasValue)
        {
            queryParts.Add("AND Price >= @MinPrice");
            parameters.Add(new MySqlParameter("@MinPrice", filter.MinPrice.Value));
        }

        if (filter.MaxPrice.HasValue)
        {
            queryParts.Add("AND Price <= @MaxPrice");
            parameters.Add(new MySqlParameter("@MaxPrice", filter.MaxPrice.Value));
        }

        if (filter.Type.HasValue)
        {
            queryParts.Add("AND activity_type = @Type");
            parameters.Add(new MySqlParameter("@Type", filter.Type.Value.ToString()));
        }

        if (filter.Status.HasValue)
        {
            queryParts.Add("AND Status = @Status");
            parameters.Add(new MySqlParameter("@Status", filter.Status.Value.ToString()));
        }

        if (!string.IsNullOrWhiteSpace(filter.Country))
        {
            queryParts.Add("AND Country = @Country");
            parameters.Add(new MySqlParameter("@Country", filter.Country));
        }

        if (!string.IsNullOrWhiteSpace(filter.City))
        {
            queryParts.Add("AND City = @City");
            parameters.Add(new MySqlParameter("@City", filter.City));
        }

        if (!string.IsNullOrWhiteSpace(filter.Address))
        {
            queryParts.Add("AND Address = @Address");
            parameters.Add(new MySqlParameter("@Address", filter.Address));
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            queryParts.Add("AND (Name LIKE @SearchTerm OR Country LIKE @SearchTerm OR City LIKE @SearchTerm OR Address LIKE @SearchTerm)");
            parameters.Add(new MySqlParameter("@SearchTerm", $"%{filter.SearchTerm}%"));
        }

        string query = string.Join(" ", queryParts);

        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters.ToArray());
        while (await reader.ReadAsync())
        {
            activities.Add(new ActivityData
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Country = reader.GetString(2),
                City = reader.GetString(3),
                Address = reader.GetString(4),
                StartTime = reader.GetDateTime(5),
                EndTime = reader.GetDateTime(6),
                Price = reader.GetDouble(7),
                Type = Enum.Parse<ActivityType>(reader.GetString(8)),
                Status = Enum.Parse<ActivityStatus>(reader.GetString(9))
            });
        }

        return activities;
    }

    public async Task<ActivityDetail?> GetActivityById(int id)
    {
        const string query = """
                             SELECT Id, Name, Country, City, Address, start_time, end_time, Price, activity_type, Status,
                                    has_equipment, has_instructor, is_indoor, description
                             FROM activities
                             WHERE Id = @id
                             """;
        var parameters = new MySqlParameter[] { new("@id", id) };

        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters);
        if (await reader.ReadAsync())
        {
            return new ActivityDetail
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Country = reader.GetString(2),
                City = reader.GetString(3),
                Address = reader.GetString(4),
                StartTime = reader.GetDateTime(5),
                EndTime = reader.GetDateTime(6),
                Price = reader.GetDouble(7),
                Type = Enum.Parse<ActivityType>(reader.GetString(8)),
                Status = Enum.Parse<ActivityStatus>(reader.GetString(9)),
                AdditionalInfo = new ActivityAdditionalInfo
                {
                    HasEquipment = reader.GetBoolean(10),
                    HasInstructor = reader.GetBoolean(11),
                    IsIndoor = reader.GetBoolean(12)
                },
                Description = reader.IsDBNull(13) ? null : reader.GetString(13)
            };
        }

        return null;
    }

    public async Task CreateActivity(Activity activity)
    {
        const string query = """
                             INSERT INTO activities
                             (Name, Country, City, Address, start_time, end_time, Price, activity_type, Status,
                              has_equipment, has_instructor, is_indoor, description)
                             VALUES(@name, @country, @city, @address, @startTime, @endTime, @price, @type, @status,
                                    @hasEquipment, @hasInstructor, @isIndoor, @description)
                             """;
        var additionalInfo = activity.AdditionalInfo ?? new ActivityAdditionalInfo();
        var parameters = new MySqlParameter[]
        {
            new("@name", activity.Name),
            new("@country", activity.Country),
            new("@city", activity.City),
            new("@address", activity.Address),
            new("@startTime", activity.StartTime),
            new("@endTime", activity.EndTime),
            new("@price", activity.Price),
            new("@type", activity.Type.ToString()),
            new("@status", activity.Status.ToString()),
            new("@hasEquipment", additionalInfo.HasEquipment),
            new("@hasInstructor", additionalInfo.HasInstructor),
            new("@isIndoor", additionalInfo.IsIndoor),
            new("@description", activity.Description ?? (object)DBNull.Value)
        };

        await MySqlHelper.ExecuteNonQueryAsync(_config.db, query, parameters);
    }

    public async Task<bool> UpdateActivity(int id, ActivityUpdateRequest update)
    {
        List<string> setParts = new();
        List<MySqlParameter> parameters = new();

        if (update.Name != null)
        {
            setParts.Add("Name = @name");
            parameters.Add(new MySqlParameter("@name", update.Name));
        }

        if (update.Country != null)
        {
            setParts.Add("Country = @country");
            parameters.Add(new MySqlParameter("@country", update.Country));
        }

        if (update.City != null)
        {
            setParts.Add("City = @city");
            parameters.Add(new MySqlParameter("@city", update.City));
        }

        if (update.Address != null)
        {
            setParts.Add("Address = @address");
            parameters.Add(new MySqlParameter("@address", update.Address));
        }

        if (update.StartTime.HasValue)
        {
            setParts.Add("start_time = @startTime");
            parameters.Add(new MySqlParameter("@startTime", update.StartTime.Value));
        }

        if (update.EndTime.HasValue)
        {
            setParts.Add("end_time = @endTime");
            parameters.Add(new MySqlParameter("@endTime", update.EndTime.Value));
        }

        if (update.Price.HasValue)
        {
            setParts.Add("Price = @price");
            parameters.Add(new MySqlParameter("@price", update.Price.Value));
        }

        if (update.Type.HasValue)
        {
            setParts.Add("activity_type = @type");
            parameters.Add(new MySqlParameter("@type", update.Type.Value.ToString()));
        }

        if (update.Status.HasValue)
        {
            setParts.Add("Status = @status");
            parameters.Add(new MySqlParameter("@status", update.Status.Value.ToString()));
        }

        if (update.Description != null)
        {
            setParts.Add("description = @description");
            parameters.Add(new MySqlParameter("@description", update.Description));
        }

        if (update.AdditionalInfo?.HasEquipment is bool hasEquipment)
        {
            setParts.Add("has_equipment = @hasEquipment");
            parameters.Add(new MySqlParameter("@hasEquipment", hasEquipment));
        }

        if (update.AdditionalInfo?.HasInstructor is bool hasInstructor)
        {
            setParts.Add("has_instructor = @hasInstructor");
            parameters.Add(new MySqlParameter("@hasInstructor", hasInstructor));
        }

        if (update.AdditionalInfo?.IsIndoor is bool isIndoor)
        {
            setParts.Add("is_indoor = @isIndoor");
            parameters.Add(new MySqlParameter("@isIndoor", isIndoor));
        }

        if (setParts.Count == 0)
        {
            return false;
        }

        parameters.Add(new MySqlParameter("@id", id));
        string query = $"UPDATE activities SET {string.Join(", ", setParts)} WHERE Id = @id";
        int affected = await MySqlHelper.ExecuteNonQueryAsync(_config.db, query, parameters.ToArray());
        return affected > 0;
    }

    public async Task DeleteActivity(int id)
    {
        const string query = "DELETE FROM activities WHERE Id = @id";
        var parameters = new MySqlParameter[] { new("@id", id) };
        await MySqlHelper.ExecuteNonQueryAsync(_config.db, query, parameters);
    }
}
