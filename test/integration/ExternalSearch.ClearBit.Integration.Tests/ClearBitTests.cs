using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Messages.Processing;
using CluedIn.Core.Processing;
using CluedIn.Core.Serialization;
using CluedIn.Core.Workflows;
using CluedIn.ExternalSearch;
using CluedIn.ExternalSearch.Providers.ClearBit;
using CluedIn.Testing.Base.Context;
using CluedIn.Testing.Base.ExternalSearch;
using CluedIn.Testing.Base.Processing.Actors;
using Moq;
using Xunit;

namespace ExternalSearch.ClearBit.Integration.Tests
{
    public class ClearBitTests : BaseExternalSearchTest<ClearBitExternalSearchProvider>
    {
        [Fact(Skip = "GitHub Issue 829 - ref https://github.com/CluedIn-io/CluedIn/issues/829")]
        public void Test()
        {
            // Arrange
            this.testContext = new TestContext();
            IEntityMetadata entityMetadata = new EntityMetadataPart()
                {
                    Name        = "Nordea",
                    EntityType  = EntityType.Organization
                };

            var externalSearchProvider  = new Mock<ClearBitExternalSearchProvider>(MockBehavior.Loose);
            var clues                   = new List<CompressedClue>();

            externalSearchProvider.CallBase = true;

            this.testContext.ProcessingHub.Setup(h => h.SendCommand(It.IsAny<ProcessClueCommand>())).Callback<IProcessingCommand>(c => clues.Add(((ProcessClueCommand)c).Clue));

            this.testContext.Container.Register(Component.For<IExternalSearchProvider>().UsingFactoryMethod(() => externalSearchProvider.Object));

            var context         = this.testContext.Context.ToProcessingContext();
            var command         = new ExternalSearchCommand();
            var actor           = new ExternalSearchProcessingAccessor(context.ApplicationContext);
            var workflow        = new Mock<Workflow>(MockBehavior.Loose, context, new EmptyWorkflowTemplate<ExternalSearchCommand>());
            
            workflow.CallBase = true;

            command.With(context);
            command.OrganizationId  = context.Organization.Id;
            command.EntityMetaData  = entityMetadata;
            command.Workflow        = workflow.Object;
            context.Workflow        = command.Workflow;

            // Act
            var result = actor.ProcessWorkflowStep(context, command);
            Assert.Equal(WorkflowStepResult.Repeat.SaveResult, result.SaveResult);

            result = actor.ProcessWorkflowStep(context, command);
            Assert.Equal(WorkflowStepResult.Success.SaveResult, result.SaveResult);
            context.Workflow.AddStepResult(result);
            
            context.Workflow.ProcessStepResult(context, command);

            // Assert
            this.testContext.ProcessingHub.Verify(h => h.SendCommand(It.IsAny<ProcessClueCommand>()), Times.AtLeastOnce);

            Assert.True(clues.Count > 0);
        }

        [Theory(Skip = "Failed Mock exception. GitHub Issue 829 - ref https://github.com/CluedIn-io/CluedIn/issues/829")]
        [InlineData("Nordea")]
        [InlineData("Sitecore")]
        [InlineData("Pfa pension")]
        public void TestClueProduction(string name)
        {
            IEntityMetadata entityMetadata = new EntityMetadataPart()
            {
                Name = name,
                EntityType = EntityType.Organization
            };

            Setup(null, entityMetadata);

            testContext.ProcessingHub.Verify(h => h.SendCommand(It.IsAny<ProcessClueCommand>()), Times.AtLeastOnce);
            Assert.True(clues.Count > 0);
        }

        [Theory]
        [InlineData("asdasdasd")]
        [InlineData("")]
        [InlineData(null)]
        [Trait("Category", "slow")]
        public void TestNoClueProduction(string name)
        {
            IEntityMetadata entityMetadata = new EntityMetadataPart()
            {
                Name = name,
                EntityType = EntityType.Organization
            };

            Setup(null, entityMetadata);

            testContext.ProcessingHub.Verify(h => h.SendCommand(It.IsAny<ProcessClueCommand>()), Times.Never);
            Assert.True(clues.Count == 0);
        }
    }
}
