using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis_Project.ViewModels;
using Tesis_Project.Models;

namespace Tesis_Project.Tests.Tests.ViewModelsTests
{
    public class AddItemModalTests
    {
        [Fact]
        public void AddItem_Should_Add_Expert_When_ModelType_Is_ExpertModel()
        {
            // Arrange
            var marcoDecision = MarcoDecisionModel.Instance;
            marcoDecision.Experts.Clear();
            var viewModel = new AddItemModalViewModel("expertModel");

            // Act
            viewModel.LabelName = "Expert 1";
            viewModel.AddItem();

            // Assert
            Assert.Single(marcoDecision.Experts);
            Assert.Equal("Expert 1", marcoDecision.Experts.First().Name);
        }
    }
}
