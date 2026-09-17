using Tycho.Identity.Structure;

namespace Tycho.UnitTests.Identity.Structure;

public class InstanceIdentityTests
{
    [Fact]
    public void CreateRoot_UsesTheDefinitionValue()
    {
        // Arrange
        DefinitionIdentity definition = DefinitionIdentity.Parse("App+abc");

        // Act
        InstanceIdentity root = InstanceIdentity.CreateRoot(definition);

        // Assert
        Assert.Equal(definition.Value, root.Value);
        Assert.Equal(root.Value, root.ToString());
    }

    [Fact]
    public void CreateRoot_WhenDefinitionIsNull_Throws()
    {
        // Act & Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => InstanceIdentity.CreateRoot(null!));
        Assert.Equal("definitionIdentity", exception.ParamName);
    }

    [Fact]
    public void CreateChild_AppendsTheDefinitionToTheCompleteParentPath()
    {
        // Arrange
        InstanceIdentity root = InstanceIdentity.CreateRoot(DefinitionIdentity.Parse("App"));

        // Act
        InstanceIdentity module = root.CreateChild(DefinitionIdentity.Parse("Module"));
        InstanceIdentity child = module.CreateChild(DefinitionIdentity.Parse("Leaf"));

        // Act
        // Assert
        Assert.Equal("App/Module/Leaf", child.Value);
    }

    [Fact]
    public void CreateChild_PreservesTheParentAndExistingChildren()
    {
        // Arrange
        InstanceIdentity root = InstanceIdentity.CreateRoot(DefinitionIdentity.Parse("App"));

        // Act
        InstanceIdentity first = root.CreateChild(DefinitionIdentity.Parse("First"));
        InstanceIdentity second = root.CreateChild(DefinitionIdentity.Parse("Second"));

        // Act
        // Assert
        Assert.Equal("App", root.Value);
        Assert.Equal("App/First", first.Value);
        Assert.Equal("App/Second", second.Value);
    }

    [Fact]
    public void CreateChild_WhenDefinitionIsNull_Throws()
    {
        // Arrange
        InstanceIdentity root = InstanceIdentity.CreateRoot(DefinitionIdentity.Parse("App"));

        // Act & Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => root.CreateChild(null!));
        Assert.Equal("definitionIdentity", exception.ParamName);
        Assert.Equal("App", root.Value);
    }

    [Fact]
    public void SameDefinitionInDifferentBranches_HasDifferentIdentity()
    {
        // Arrange
        InstanceIdentity root = InstanceIdentity.CreateRoot(DefinitionIdentity.Parse("App"));
        DefinitionIdentity leaf = DefinitionIdentity.Parse("Leaf");

        // Act
        InstanceIdentity sales = root.CreateChild(DefinitionIdentity.Parse("Sales")).CreateChild(leaf);
        InstanceIdentity support = root.CreateChild(DefinitionIdentity.Parse("Support")).CreateChild(leaf);

        // Assert
        Assert.False(sales == support);
    }

    [Fact]
    public void SameModulePathInDifferentApps_HasDifferentIdentity()
    {
        // Arrange
        DefinitionIdentity module = DefinitionIdentity.Parse("Module");

        // Act
        InstanceIdentity first = InstanceIdentity.CreateRoot(DefinitionIdentity.Parse("FirstApp")).CreateChild(module);
        InstanceIdentity second = InstanceIdentity.CreateRoot(DefinitionIdentity.Parse("SecondApp")).CreateChild(module);

        // Assert
        Assert.False(first == second);
    }

    [Fact]
    public void IndependentlyConstructedInstancesWithTheSamePath_AreEqual()
    {
        // Act
        InstanceIdentity first = CreateLeaf();
        InstanceIdentity replica = CreateLeaf();

        // Assert
        Assert.True(first == replica);
        Assert.NotSame(first, replica);
    }

    [Fact]
    public void Equals_WithDifferentPathOrUnrelatedObject_ReturnsFalse()
    {
        // Arrange
        InstanceIdentity leaf = CreateLeaf();
        InstanceIdentity root = InstanceIdentity.CreateRoot(DefinitionIdentity.Parse("Leaf"));

        // Act & Assert
        Assert.False(leaf.Equals(root));
        Assert.False(leaf.Equals((object)root));
        Assert.False(leaf.Equals(new object()));
        Assert.False(leaf.Equals("App/Module/Leaf"));
    }

    [Fact]
    public void Equality_WithNullReferences_EvaluatesCorrectly()
    {
        // Arrange
        InstanceIdentity identity = CreateLeaf();
        InstanceIdentity? missing = null;

        // Act & Assert
        Assert.False(identity.Equals((InstanceIdentity)null!));
        Assert.False(identity.Equals((object?)null));
        Assert.False(identity == missing);
        Assert.False(missing == identity);
        Assert.True(identity != missing);
        Assert.True(missing != identity);
        Assert.True(missing == (InstanceIdentity?)null);
        Assert.False(missing != (InstanceIdentity?)null);
    }

    [Fact]
    public void Equality_ComparesTheBaseValueAcrossIdentityTypes()
    {
        // Arrange
        DefinitionIdentity definition = DefinitionIdentity.Parse("App");
        InstanceIdentity root = InstanceIdentity.CreateRoot(definition);
        Tycho.Identity.Identity identity = root;

        // Act & Assert
        Assert.True(identity.Equals(definition));
        Assert.True(identity.Equals((object)definition));
        Assert.True(identity == definition);
        Assert.Equal(identity.GetHashCode(), definition.GetHashCode());
    }

    [Fact]
    public void Equality_IsCaseSensitive()
    {
        // Arrange
        InstanceIdentity first = InstanceIdentity.CreateRoot(DefinitionIdentity.Parse("App"));
        InstanceIdentity second = InstanceIdentity.CreateRoot(DefinitionIdentity.Parse("app"));

        // Act & Assert
        Assert.False(first.Equals(second));
        Assert.False(first == second);
    }

    private static InstanceIdentity CreateLeaf() => InstanceIdentity
        .CreateRoot(DefinitionIdentity.Parse("App"))
        .CreateChild(DefinitionIdentity.Parse("Module"))
        .CreateChild(DefinitionIdentity.Parse("Leaf"));
}
