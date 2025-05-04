using System;
using SimpleMapper.Interface;

namespace SimpleMapper.Configuration;

public class SimpleMapperConfigurationBuilder
{
    private readonly List<ISimpleMapperProfile> _profiles = new();

    public void AddProfile<T>() where T : ISimpleMapperProfile, new()
    {
        _profiles.Add(new T());
    }

    public SimpleMapperConfiguration Build()
    {
        var config = new SimpleMapperConfiguration();
        foreach (var profile in _profiles)
            profile.Configure(config);
        return config;
    }
}
