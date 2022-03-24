using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using Diagraph.Infrastructure.Models;

namespace Diagraph.Infrastructure.Parsing;

public class LibreViewCsvGlucoseDataParser : IGlucoseDataParser
{
    private static CsvConfiguration _configuration = new(CultureInfo.InvariantCulture)
    {
        Delimiter = ",",
        HasHeaderRecord = true
    };
    
    public IEnumerable<GlucoseMeasurement> Parse(string data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Stream dataStream         = new MemoryStream(Encoding.UTF8.GetBytes(data));
        using StreamReader reader = new StreamReader(dataStream);
        using CsvReader csv       = new CsvReader(reader, _configuration);

        // Skip one line because of double header.
        // TODO: find a way to avoid through configuration.
        csv.Read();
        
       return csv.GetRecords<LibreViewRow>()
            .Select
            (
                record => new GlucoseMeasurement
                {
                    // TODO: add scan type?
                    Level = record.StripGlucose 
                            ?? record.ScanGlucose 
                            ?? record.HistoricGlucose.GetValueOrDefault(),
                    TakenAt = record.DeviceTimestamp.GetValueOrDefault()
                }
            ).ToList();
    }

    private class LibreViewDateTimeConverter : TypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow _, MemberMapData __)
        {
            DateTime parsed = DateTime.ParseExact(text, "dd-MM-yyyy HH:mm", null);
            return DateTime.SpecifyKind(parsed, DateTimeKind.Utc); // So EntityFramework stops complaining.
        }
    }

    private class LibreViewRow
    {
        [Name("Device")]
        public string Device { get; set; }
        
        [Name("Serial Number")]
        public string SerialNumber { get; set; }
        
        [Name("Device Timestamp")]
        [TypeConverter(typeof(LibreViewDateTimeConverter))]
        public DateTime? DeviceTimestamp { get; set; }
        
        [Name("Record Type")]
        public int? RecordType { get; set; }
        
        [Name("Historic Glucose mmol/L")]
        public float? HistoricGlucose { get; set; }
        
        [Name("Scan Glucose mmol/L")]
        public float? ScanGlucose { get; set; }
        
        [Name("Non-numeric Rapid-Acting Insulin")]
        public string NonNumericRapidActingInsulin { get; set; }
        
        [Name("Non-numeric Food")]
        public string NonNumericFood { get; set; }
        
        [Name("Carbohydrates (grams)")]
        public string CarbohydratesGrams { get; set; }
        
        [Name("Carbohydrates (servings)")]
        public string CarbohydratesServings { get; set; }
        
        [Name("Non-numeric Long-Acting Insulin")]
        public string NonNumericLongActingInsulin { get; set; }
        
        [Name("Long-Acting Insulin (units)")]
        public string LongActingInsulinUnits { get; set; }
        
        [Name("Notes")]
        public string Notes { get; set; }
        
        [Name("Strip Glucose mmol/L")]
        public float? StripGlucose { get; set; }
        
        [Name("Ketone mmol/L")]
        public float? Ketone { get; set; }
        
        [Name("Meal Insulin (units)")]
        public int? MealInsulinUnits { get; set; }
        
        [Name("Correction Insulin (units)")]
        public int? CorrectionInsulinUnits { get; set; }
        
        [Name("User Change Insulin (units)")]
        public int? UserChangeInsulinUnits { get; set; }
    }
}