using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Locations.Models;
using SReader.Domains.Locations.Repositories;
using UnityEngine;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>
    /// PostgREST-backed location repository over {SupabaseUrl}/rest/v1.
    /// Table: user_locations, keyed by user_id. Calls carry the current
    /// user's access token so Supabase Row-Level Security applies.
    /// </summary>
    public sealed class SupabaseUserLocationRepository : SupabaseRepositoryBase, IUserLocationRepository
    {
        readonly CurrentSessionHolder session;

        string RestUrl => Settings.SupabaseUrl.TrimEnd('/') + "/rest/v1";
        string Token => session?.Session?.AccessToken;

        public SupabaseUserLocationRepository(AppSettings settings, CurrentSessionHolder session) : base(settings)
        {
            this.session = Guard.NotNull(session, nameof(session));
        }

        public async Task<Result<UserLocation>> GetForUserAsync(string userId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<UserLocation>(configured.Error);

            var url = $"{RestUrl}/user_locations?user_id=eq.{Uri.EscapeDataString(userId)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<UserLocation>(response.Error);

            var rows = ParseArray<LocationDto>(response.Value);
            // No saved location yet is not an error — hand back an empty one to edit.
            return Result.Ok(rows.Length == 0 ? new UserLocation { UserId = userId } : rows[0].ToModel());
        }

        public async Task<Result> UpsertAsync(UserLocation location, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(LocationDto.From(location));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/user_locations",
                Settings.SupabaseAnonKey, body, Token, ct, "resolution=merge-duplicates,return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> DeleteForUserAsync(string userId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var url = $"{RestUrl}/user_locations?user_id=eq.{Uri.EscapeDataString(userId)}";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Delete, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        [Serializable]
        class LocationDto
        {
            public string user_id;
            public string country;
            public string province;
            public string city;
            public string address;
            public double latitude;
            public double longitude;

            public UserLocation ToModel() => new UserLocation
            {
                UserId = user_id,
                Country = country,
                Province = province,
                City = city,
                Address = address,
                Latitude = latitude,
                Longitude = longitude
            };

            public static LocationDto From(UserLocation l) => new LocationDto
            {
                user_id = l.UserId,
                country = l.Country,
                province = l.Province,
                city = l.City,
                address = l.Address,
                latitude = l.Latitude,
                longitude = l.Longitude
            };
        }

        // JsonUtility cannot parse a top-level JSON array, so wrap it first.
        [Serializable] class Wrapper<T> { public T[] items; }

        static T[] ParseArray<T>(string json)
        {
            if (string.IsNullOrEmpty(json) || json == "[]") return Array.Empty<T>();
            try
            {
                var wrapped = JsonUtility.FromJson<Wrapper<T>>("{\"items\":" + json + "}");
                return wrapped?.items ?? Array.Empty<T>();
            }
            catch
            {
                return Array.Empty<T>();
            }
        }
    }
}
