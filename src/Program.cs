using BenchmarkDotNet.Running;
using src;
using System.Collections.Concurrent;
using System.Linq.Expressions;

var summary = BenchmarkRunner.Run<MyBenchmark>();

static class Mapper
{
    public static Tout Mapping<Tin, Tout>(Tin obj)
    {
        var Tinype = typeof(Tin);
        var Toutype = typeof(Tout);

        //var funcValue = MapperCache<Tin, Tout>.ToLambda($"{Tinype.Name}:{Toutype.Name}");
        //if (funcValue != null)
        //{
        //    return funcValue.Invoke(obj);
        //}
        var parameter = Expression.Parameter(Tinype, "source");
        var variable = Expression.Variable(Toutype, "mapper");

        var mapperProperties = Toutype.GetProperties();

        var expressions = new List<Expression>
    {
        Expression.Assign(variable, Expression.New(Toutype))
    };

        foreach (var mapperProperty in mapperProperties)
        {
            expressions.Add(Expression.Assign
                (
                    Expression.Property(variable, mapperProperty.Name),
                    Expression.Property(parameter, mapperProperty.Name)
                ));
        }

        expressions.Add(variable);

        var block = Expression.Block(new[] { variable }, expressions);

        var lambda = Expression.Lambda<Func<Tin, Tout>>(block, parameter);

        var func = lambda.Compile();

        //MapperCache<Tin, Tout>.AddValue($"{Tinype.Name}:{Toutype.Name}", func);

        var dto = func.Invoke(obj);
        return dto;
    }

    public static Tout CacheMapping<Tin, Tout>(Tin obj)
    {
        var Tinype = typeof(Tin);
        var Toutype = typeof(Tout);

        var funcValue = MapperCache<Tin, Tout>.ToLambda($"{Tinype.Name}:{Toutype.Name}");
        if (funcValue != null)
        {
            return funcValue.Invoke(obj);
        }

        var parameter = Expression.Parameter(Tinype, "source");
        var variable = Expression.Variable(Toutype, "mapper");

        var mapperProperties = Toutype.GetProperties();

        var expressions = new List<Expression>
    {
        Expression.Assign(variable, Expression.New(Toutype))
    };

        foreach (var mapperProperty in mapperProperties)
        {
            expressions.Add(Expression.Assign
                (
                    Expression.Property(variable, mapperProperty.Name),
                    Expression.Property(parameter, mapperProperty.Name)
                ));
        }

        expressions.Add(variable);

        var block = Expression.Block(new[] { variable }, expressions);

        var lambda = Expression.Lambda<Func<Tin, Tout>>(block, parameter);

        var func = lambda.Compile();

        MapperCache<Tin, Tout>.AddValue($"{Tinype.Name}:{Toutype.Name}", func);

        var dto = func.Invoke(obj);
        return dto;
    }

}


public static class MapperCache<Tin, Tout>
{
    private readonly static ConcurrentDictionary<string, Func<Tin, Tout>> _cache = new();

    public static Func<Tin, Tout>? ToLambda(string exp)
    {
        return _cache.TryGetValue(exp, out var value) ? value : null;
    }

    public static void AddValue(string exp, Func<Tin, Tout> func)
    {
        _cache.TryAdd(exp, func);
    }
}

class BasicClass
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public List<int> Ints { get; set; }
    public int Age { get; set; }
}



class DTOClass
{
    public string Name { get; set; }
    public int Age { get; set; }
    public List<int> Ints { get; set; }
}



class DTOClass2
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public List<int> Ints { get; set; }
}