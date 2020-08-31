using Cotorra.Core.Utils;
using System;
using System.Linq;
using Xunit;
using Cotorra.Schema;
using System.Collections.Generic;

namespace Cotorra.UnitTest.Util
{

    public class CloneExtensionUT
    {
        [Fact]
        public void SholudCloneAnOverdraft()
        {
            var instance = Guid.NewGuid();
            var company = Guid.NewGuid();
            var overdraft = OverdraftManagerUT.GenerateObject(company, instance).FirstOrDefault();
            var cloneExtensionManager   = new CloneExtensionManager<Overdraft>();

            var overCloned = cloneExtensionManager.DeepClone(overdraft);

            Assert.NotNull(overCloned);
            Assert.Equal(overdraft.ID, overCloned.ID);
            Assert.Equal(overdraft.company, overCloned.company);

        }

        [Fact]
        public void SholudCloneAnOverdraftCheckTheyAreDifferntObjects()
        {
            var instance = Guid.NewGuid();
            var company = Guid.NewGuid();

            var overdraft = OverdraftManagerUT.GenerateObject(company, instance).FirstOrDefault();
            var cloneExtensionManager = new CloneExtensionManager<Overdraft>();

            var overCloned = cloneExtensionManager.DeepClone(overdraft);

            Assert.NotNull(overCloned);
            Assert.Equal(overdraft.ID, overCloned.ID);

            overCloned.ID = Guid.NewGuid();

            Assert.NotEqual(overdraft.ID, overCloned.ID);


        }

    }
}
