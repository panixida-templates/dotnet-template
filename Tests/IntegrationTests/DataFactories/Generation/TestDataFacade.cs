using AutoFixture;
using AutoFixture.Dsl;

using Bogus;

using Common.Helpers;

using IntegrationTests.DataFactories.Generation.Configuration;
using IntegrationTests.DataFactories.Generation.Core;

namespace IntegrationTests.DataFactories.Generation;

public sealed class TestDataFacade
{
    private IFixture Fixture { get; }

    public TestDataFacade(IFixture fixture)
    {
        Fixture = fixture;
    }

    public TestDataFacade(
        string locale = "ru",
        string? scope = null,
        int recursionDepth = 3,
        int? seed = null,
        Action<Faker>? configureFaker = null)
    {
        var scopeHash = string.IsNullOrEmpty(scope) ? 0 : CryptoHelper.GetHash(scope);
        var usedSeed = (seed ?? FixtureFactory.DefaultSeed) ^ scopeHash;

        Fixture = FixtureFactory.Create(locale, usedSeed, recursionDepth, configureFaker);
    }

    public T Create<T>()
    {
        return Fixture.Create<T>();
    }

    public ICustomizationComposer<T> Build<T>()
    {
        return Fixture.Build<T>();
    }

    public T Mutate<T>(
        T source,
        int minChanges = 1,
        int maxChanges = 3,
        IEnumerable<string>? excludeProperties = null) where T : class
    {
        return DataMutator.Mutate(source, Fixture, minChanges, maxChanges, excludeProperties);
    }
}
