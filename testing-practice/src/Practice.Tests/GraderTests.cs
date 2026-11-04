using System;
using System.Collections.Generic;
using System.Text;

namespace Practice.Tests
{
    public class GraderTests
    {
        [Theory]
        [InlineData(100,"A")]
        [InlineData(60, "B")]
        [InlineData(50, "C")]
        [InlineData(0, "Fail")]

        public void TestGradeLimits(int min, string expectedgrade)
        {
            var grade = Grader.Grade(min);

            Assert.Equal(grade, expectedgrade);
        }

        [Fact]
        public void Add_AboveLimits_ReturnsOutOfRange()
        {
            // Arrange
            int score = 101;
            // Asset
            Assert.Throws<ArgumentOutOfRangeException>(()=>Grader.Grade(score));
        }


        public void Add_BelowLimits_ReturnsOutOfRange()
        {
            // Arrange
            int score = 0;

            // Asset
            Assert.Throws<ArgumentOutOfRangeException>(() => Grader.Grade(score));
        }
    }
}
