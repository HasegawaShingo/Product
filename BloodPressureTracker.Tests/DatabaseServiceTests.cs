using BloodPressureTracker.Models;
using BloodPressureTracker.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BloodPressureTracker.Tests
{
    public class DatabaseServiceTests : IDisposable
    {
        private readonly string _dbPath;
        private readonly DatabaseService _db;

        public DatabaseServiceTests()
        {
            _dbPath = Path.Combine(Path.GetTempPath(), $"bp_test_{Guid.NewGuid()}.db3");
            _db = new DatabaseService(_dbPath);
        }

        [Fact]
        public async Task SaveAndRetrieveMorningAndEveningRecords_ByDateRange()
        {
            var date = DateTime.Today;

            var morning = new BloodPressureRecord
            {
                RecordDate = date.AddHours(8),
                TimeCategory = TimeCategory.Morning,
                Systolic1 = 120,
                Diastolic1 = 80,
                Pulse1 = 70,
                Systolic2 = 118,
                Diastolic2 = 78,
                Pulse2 = 69,
                Notes = "morning"
            };

            var evening = new BloodPressureRecord
            {
                RecordDate = date.AddHours(20),
                TimeCategory = TimeCategory.Evening,
                Systolic1 = 125,
                Diastolic1 = 82,
                Pulse1 = 72,
                Systolic2 = 124,
                Diastolic2 = 81,
                Pulse2 = 71,
                Notes = "evening"
            };

            await _db.SaveRecordAsync(morning);
            await _db.SaveRecordAsync(evening);

            var records = await _db.GetRecordsByDateRangeAsync(date.Date, date.Date.AddDays(1).AddTicks(-1));

            Assert.Contains(records, r => r.TimeCategory == TimeCategory.Morning && r.Notes == "morning");
            Assert.Contains(records, r => r.TimeCategory == TimeCategory.Evening && r.Notes == "evening");
        }

        public void Dispose()
        {
            try { File.Delete(_dbPath); } catch { }
        }
    }
}
