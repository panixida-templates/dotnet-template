using AutoFixture.Kernel;

using Bogus;

using IntegrationTests.DataFactories.Generation.Generators.Interfaces;

namespace IntegrationTests.DataFactories.Generation.Generators.Implementations;

internal sealed class GuidGenerator(Faker faker) : ITypeDataGenerator
{
    public bool TryGenerate(Type type, string? name, ISpecimenContext context, out object? value)
    {
        if (type == typeof(Guid))
        {
            value = new Guid(faker.Random.Bytes(16));
            return true;
        }

        value = null;

        return false;
    }
}
