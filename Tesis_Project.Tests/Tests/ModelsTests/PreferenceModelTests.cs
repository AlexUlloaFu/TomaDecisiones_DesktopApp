using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis_Project.Models.Domains;
using Tesis_Project.Models;
using Xunit;

namespace Tesis_Project.Tests.Models
{
    public class PreferenceModelTests
    {
        [Fact]
        public void SettingDomain_ShouldRaisePropertyChangedEvent()
        {
            // Arrange
            var preference = new PreferenceModel();
            var domain = new DomainModel { Type = DomainType.Linguistico };
            string changedProperty = null;

            preference.PropertyChanged += (sender, args) =>
            {
                changedProperty = args.PropertyName;
            };

            // Act
            preference.Domain = domain;

            // Assert
            Assert.Equal(nameof(preference.Domain), changedProperty);
        }

        [Fact]
        public void SettingDomain_ShouldTriggerComputedPropertyNotifications()
        {
            // Arrange
            var preference = new PreferenceModel();
            var domain = new DomainModel { Type = DomainType.Linguistico };
            bool linguisticNotified = false;
            bool intervalarNotified = false;
            bool realNotified = false;

            preference.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(preference.IsLinguisticPreference)) linguisticNotified = true;
                if (args.PropertyName == nameof(preference.IsIntervalarPreference)) intervalarNotified = true;
                if (args.PropertyName == nameof(preference.IsRealPreference)) realNotified = true;
            };

            // Act
            preference.Domain = domain;

            // Assert
            Assert.True(linguisticNotified);
            Assert.True(intervalarNotified);
            Assert.True(realNotified);
        }

        [Fact]
        public void SettingDomain_ShouldInvokeDomainChangedEvent()
        {
            // Arrange
            var preference = new PreferenceModel();
            var domain = new DomainModel { Type = DomainType.Real };
            bool domainChangedInvoked = false;

            preference.DomainChanged += () => domainChangedInvoked = true;

            // Act
            preference.Domain = domain;

            // Assert
            Assert.True(domainChangedInvoked);
        }

        [Fact]
        public void SettingRealValue_ShouldRaisePropertyChangedEvent()
        {
            // Arrange
            var preference = new PreferenceModel();
            string changedProperty = null;

            preference.PropertyChanged += (sender, args) =>
            {
                changedProperty = args.PropertyName;
            };

            // Act
            preference.RealValue = 10.5f;

            // Assert
            Assert.Equal(nameof(preference.RealValue), changedProperty);
        }

        [Fact]
        public void SettingLowerOrUpperLimit_ShouldUpdateIntervalarValue()
        {
            // Arrange
            var preference = new PreferenceModel();
            string changedProperty = null;

            preference.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(preference.IntervalarValue))
                {
                    changedProperty = args.PropertyName;
                }
            };

            // Act
            preference.LowerLimit = 1.5f;
            preference.UpperLimit = 5.0f;

            // Assert
            Assert.Equal(nameof(preference.IntervalarValue), changedProperty);
            Assert.Equal("[1.5,5]", preference.IntervalarValue);
        }

        [Fact]
        public void IsLinguisticPreference_ShouldReturnTrue_WhenDomainIsLinguistico()
        {
            // Arrange
            var preference = new PreferenceModel
            {
                Domain = new DomainModel { Type = DomainType.Linguistico }
            };

            // Act & Assert
            Assert.True(preference.IsLinguisticPreference);
            Assert.False(preference.IsIntervalarPreference);
            Assert.False(preference.IsRealPreference);
        }
    }
}
