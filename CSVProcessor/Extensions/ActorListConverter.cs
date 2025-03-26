using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace CSVProcessor.Helpers;

public class ActorListConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        return text.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToList();
    }
}