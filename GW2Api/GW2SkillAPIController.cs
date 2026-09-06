using System.Text.Json;
using GW2EIGW2API.GW2API;

namespace GW2EIGW2API;

internal class GW2SkillAPIController
{
    private const string APIPath = "/v2/skills";

    private GW2APIUtilities.APIItems<GW2APISkill> _apiSkills = new();
    private static List<GW2APISkill> GetGW2APISkills()
    {
        Console.WriteLine("Getting skills from API");

        return GW2APIUtilities.GetGW2APIItems<GW2APISkill>(APIPath);
    }
    internal GW2APIUtilities.APIItems<GW2APISkill> GetAPISkills(string cachePath)
    {
        if (_apiSkills.Items.Count == 0)
        {
            SetAPISkills(cachePath);
        }
        return _apiSkills;
    }

    internal void WriteAPISkillsToFile(string filePath)
    {
        FileStream fcreate = File.Open(filePath, FileMode.Create);
        fcreate.Close();

        List<GW2APISkill> skills = GetGW2APISkills();
        using(var writer = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
            JsonSerializer.Serialize(writer, skills, GW2APIUtilities.SerializerSettings);
        }

        // refresh API cache
        _apiSkills = new GW2APIUtilities.APIItems<GW2APISkill>(skills);
    }
    // FORK: writes the in-memory cache without re-downloading; used to persist the startup download.
    internal void WriteCachedAPISkillsToFile(string filePath)
    {
        // Never persist an empty result: it usually means the API was unreachable, and an empty file
        // would be read back as a valid cache, so the download would never be retried.
        if (_apiSkills.Items.Count == 0)
        {
            return;
        }
        using var writer = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        JsonSerializer.Serialize(writer, _apiSkills.Items.Values.ToList(), GW2APIUtilities.SerializerSettings);
    }

    private void SetAPISkills(string filePath)
    {
        var fi = new FileInfo(filePath);
        if (fi.Exists && fi.Length != 0)
        {
            Console.WriteLine("Reading SkillList");
            using (var reader = fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var skillList = JsonSerializer.Deserialize<List<GW2APISkill>>(reader, GW2APIUtilities.DeserializerSettings);
                _apiSkills = new GW2APIUtilities.APIItems<GW2APISkill>(skillList);
            }
        }
        else
        {
            _apiSkills = new GW2APIUtilities.APIItems<GW2APISkill>(GetGW2APISkills());
        }
    }
}

