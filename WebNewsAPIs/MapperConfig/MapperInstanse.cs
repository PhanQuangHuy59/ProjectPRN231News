using AutoMapper;

namespace ProjectAPIAss.MapperConfig
{
    public class MapperInstanse 
    {
        private static MapperConfiguration configuration;

        public static IMapper GetMapper()
        {
            configuration = new MapperConfiguration(cf =>
            {
                cf.AddProfile(new MapperProfile());
            });
            return configuration.CreateMapper();
        }




    }
}
