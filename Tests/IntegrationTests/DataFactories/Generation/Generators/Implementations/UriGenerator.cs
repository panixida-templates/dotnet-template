using AutoFixture.Kernel;

using Bogus;

using IntegrationTests.DataFactories.Generation.Generators.Interfaces;

namespace IntegrationTests.DataFactories.Generation.Generators.Implementations;

internal sealed class UriGenerator(Faker faker) : ITypeDataGenerator
{
    public bool TryGenerate(Type type, string? name, ISpecimenContext context, out object? value)
    {
        if (type == typeof(Uri))
        {
            value = new Uri(faker.Internet.Url());
            return true;
        }

        value = null;

        return false;
    }
}
