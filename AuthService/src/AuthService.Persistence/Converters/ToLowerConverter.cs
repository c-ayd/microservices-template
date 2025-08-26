using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuthService.Persistence.Converters
{
    public class ToLowerConverter : ValueConverter<string?, string?>
    {
        public ToLowerConverter()
            : base(app => app != null ? app.ToLower() : null, 
                  db => db)
        {
        }
    }
}
