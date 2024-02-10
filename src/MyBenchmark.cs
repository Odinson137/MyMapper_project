using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace src
{
    public class MyBenchmark
    {
        BasicClass basicClass = new BasicClass()
        {
            Name = "Foo",
            LastName = "Bar",
            Age = 1,
            Ints = new List<int> { 1, 2, 3 },
        };

        [Benchmark]
        public void AutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BasicClass, DTOClass2>();
            });

            IMapper mapper = config.CreateMapper();
            var dtoClass2 = mapper.Map<DTOClass2>(basicClass);
        }

        [Benchmark]
        public void MyMapper()
        {
            Mapper.Mapping<BasicClass, DTOClass2>(basicClass);
        }


        [Benchmark]
        public void MyMapperWithCache()
        {
            Mapper.CacheMapping<BasicClass, DTOClass2>(basicClass);
        }
    }
}
